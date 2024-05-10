using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BuildingSystem : MonoBehaviour
{
    public BuildGrid worldGrid;
    [SerializeField] Transform objectsContainer;
    private BuildObject currentBuildObject;
    [SerializeField] private GameObject buildTemplate;
    [SerializeField] private GameObject backBuildTemplate;
    public BuildCatalog buildCatalog;
    [SerializeField] private GameObject placeholderPrefab;
    private GameObject placeholder;
    [SerializeField] private GameObject destroyParticlesPrefab;

    float holdToDestroyTime = 0.2f;

    [SerializeField] private MenuManager menuManager;
    public static BuildingSystem Instance;
    bool building;

    public static bool InVirtualHangar => SceneManager.GetActiveScene().name == "VirtualHangar";

    private void Awake()
    {
        Instance = this;
    }
    private void Start()
    {
        if (InVirtualHangar)
        {
            worldGrid = new BuildGrid(new Vector2Int(-10, -10), 20, 20);
        }
        else
        {
            worldGrid = new BuildGrid(new Vector2Int(-100, -100));
            TerrainManager.Instance?.AddGroundTiles();
        }
        if (objectsContainer == null)
        {
            Debug.Log("Building system error: Please assign objects container");
        }

        placeholder = Instantiate(placeholderPrefab);

        currentBuildObject = new BuildObject(null);
    }

    public void StartBuilding()
    {
        building = true;
    }
    public void EndBuilding()
    {
        if (placeholder != null) placeholder.SetActive(false);
        building = isPlacing = isDeleting = false;
        InterruptDeleteTimer();
    }

    public Build SetBuildObject(int index)
    {
        Build newBuild = buildCatalog.GetBuild(index);
        currentBuildObject.build = newBuild;
        currentBuildObject.rot = 0;
        UpdatePlaceholder();

        return newBuild;
    }
    public BuildCategory SetCategory(int index)
    {
        return buildCatalog.SetCategory(index);
    }

    bool PlaceObject(Vector3 worldPos, BuildGrid thisGrid, Transform parent)
    {
        if (!thisGrid.PositionIsWithinGrid(worldPos) || thisGrid.GetValueAtPosition(worldPos) != null) return false;

        GameObject obj = PlaceBlock(worldPos, currentBuildObject, parent);
        // Save object placement in the grid
        BuildObject buildObjectCopy = currentBuildObject.Clone();
        buildObjectCopy.gridObject = obj;
        thisGrid.SetValueAtPosition(worldPos, buildObjectCopy);
        return true;
    }
    GameObject PlaceBlock(Vector3 worldPos, BuildObject thisBuildObject, Transform parent)
    {
        if (thisBuildObject.build == null) return null;
        // Determine the object template to use
        Build thisBuild = thisBuildObject.build;
        Rotation thisRotation = thisBuildObject.GetRotation();
        GameObject clone = thisRotation.Object;
        if (clone == null)
        {
            if (thisBuild.depth == Build.DepthLevel.MidGround)
                clone = buildTemplate;
            else
                clone = backBuildTemplate;
        }

        // Place the object in world
        GameObject obj = Instantiate(clone, Vector2.zero, Quaternion.identity, parent);
        obj.name = thisBuild.name;
        obj.transform.position = worldPos;
        obj.transform.localRotation = Quaternion.Euler(0, 0, thisRotation.DegRotation);

        if (thisRotation.sprite != null)
        {
            obj.GetComponent<SpriteRenderer>().sprite = thisRotation.sprite;
        }
        if (thisBuild.depth == Build.DepthLevel.MidGround)
        {
            PolygonCollider2D collider = obj.AddComponent<PolygonCollider2D>();
            collider.usedByComposite = true;
        }
        if (thisRotation.flipX)
        {
            obj.transform.localScale = new Vector3(-1, obj.transform.localScale.y, 1);
        }
        if (thisRotation.flipY)
        {
            obj.transform.localScale = new Vector3(obj.transform.localScale.x, -1, 1);
        }
        return obj;
    }
    public bool DeleteObject(Vector3 worldPos, BuildGrid thisGrid, bool deleteTerrain = false)
    {
        BuildObject buildObj = thisGrid.GetValueAtPosition(worldPos);
        if (buildObj == null || (!deleteTerrain && buildObj.gridObject == null) || !thisGrid.RemoveValueAtPosition(worldPos)) return false;

        if (buildObj.gridObject != null)
        {
            // Delete object from world
            Destroy(buildObj.gridObject);
        }

        // Particles
        CreateParticles(thisGrid.WorldtoAligned(worldPos));
        return true;
    }
    void CreateParticles(Vector3 worldPos)
    {
        Instantiate(destroyParticlesPrefab, worldPos, Quaternion.identity);
    }
    void RotateObject()
    {
        currentBuildObject.AdvanceRotation();
        UpdatePlaceholder();
    }
    public void SpawnObjects(BuildGrid thisGrid, Transform parent)
    {
        foreach (KeyValuePair<Vector2Int, BuildObject> p in thisGrid.gridObjects)
        {
            GameObject obj = PlaceBlock(thisGrid.GridtoWorldAligned(p.Key), p.Value, parent);
            p.Value.gridObject = obj;
        }
    }
    public void RepositionObjects(BuildGrid thisGrid, Transform parent)
    {
        foreach (KeyValuePair<Vector2Int, BuildObject> p in thisGrid.gridObjects)
        {
            p.Value.gridObject.transform.position = thisGrid.GridtoWorldAligned(p.Key);
        }
    }
    void UpdatePlaceholder()
    {
        Rotation thisRotation = currentBuildObject.GetRotation();
        SpriteRenderer renderer = placeholder.GetComponent<SpriteRenderer>();
        renderer.sprite = thisRotation.sprite;
        renderer.flipX = thisRotation.flipX;
        renderer.flipY = thisRotation.flipY;
    }

    IEnumerator deleteTimer;
    bool isPlacing;
    bool isDeleting;

    IEnumerator DeleteDelay()
    {
        yield return new WaitForSeconds(holdToDestroyTime);
        isDeleting = true;
    }
    void InterruptDeleteTimer()
    {
        if (deleteTimer != null)
            StopCoroutine(deleteTimer);
    }
    private void Update()
    {
        if (!building || objectsContainer == null) return;

        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        BuildGrid selectedGrid = worldGrid;
        Transform selectedParent = objectsContainer;

        Ship thisShip = null;
        ///This searches for ships that might be where the cursor is located, allowing the player to build on them instead.
        ///This should be reworked to let the play place on the closest ship to the mouse, just in case too many ships/builds are competing for player placement attention
        foreach (Ship ship in ShipBuilding.loadedShips)
        {
            BuildGrid shipGrid = ship.ship;
            if (shipGrid.PositionIsWithinGrid(mousePos))
            {
                thisShip = ship;
                selectedParent = ship.transform;
                selectedGrid = ship.ship;
                break;
            }
        }
        Vector3 alignedPos = selectedGrid.WorldtoAligned(mousePos);
        Vector3Int cellPos = TerrainManager.Instance.ground.WorldToCell(mousePos);
        bool hasBuild = currentBuildObject.build != null;
        bool spaceAvailable = selectedGrid.GetValueAtPosition(alignedPos) == null && !TerrainManager.Instance.ground.HasTile(cellPos);
        bool hasAdjacent = true;
        if (!InVirtualHangar)
        {
            hasAdjacent = selectedGrid.PositionHasAdjacent(mousePos) || TerrainManager.HasAdjacentTile(cellPos, TerrainManager.Instance.ground);
        }
        bool canPlace = hasBuild && spaceAvailable && hasAdjacent;

        if (Input.GetMouseButtonDown(0))
        {
            if (canPlace)
            {
                isPlacing = true;
            }
            else
            {
                deleteTimer = DeleteDelay();
                StartCoroutine(deleteTimer);
            }
        }
        if (Input.GetMouseButtonUp(0))
        {
            isPlacing = isDeleting = false;
            InterruptDeleteTimer();
        }

        if (!isDeleting && hasBuild)
        {
            placeholder.transform.position = alignedPos;
            placeholder.SetActive(spaceAvailable);
            Rotation thisRotation = currentBuildObject.GetRotation();
            placeholder.transform.rotation = Quaternion.Euler(0, 0, thisRotation.DegRotation + selectedGrid.rotation);

            if (canPlace)
            {
                
                placeholder.GetComponent<SpriteRenderer>().color = new Color32(255, 255, 255, 127);
            }
            else
            {
                placeholder.GetComponent<SpriteRenderer>().color = new Color32(255, 0, 0, 127);
            }
        }
        else
        {
            placeholder.SetActive(false);
        }

        if (!menuManager.IsOnUI())
        {
            if (isPlacing)
            {
                if (PlaceObject(alignedPos, selectedGrid, selectedParent))
                {
                    if (thisShip != null && selectedGrid.PositionIsAtEdge(alignedPos))
                        thisShip.UpdateShip();
                }
            }
            else if (isDeleting)
            {
                //bool willCollapse = selectedGrid.CheckCollapseOnDelete(alignedPos);
                bool willCollapse = false;
                if (!willCollapse && DeleteObject(alignedPos, selectedGrid))
                {
                    if (thisShip != null && selectedGrid.PositionIsAtEdge(alignedPos))
                        thisShip.UpdateShip();
                }
            }
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            RotateObject();
        }
    }
    private void OnDisable()
    {
        if (placeholder != null) placeholder.SetActive(false);
        isPlacing = isDeleting = false;
        InterruptDeleteTimer();
    }
}
