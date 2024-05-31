using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BuildingSystem : MonoBehaviour
{
    public BuildGrid worldGrid;
    [SerializeField] Transform objectsContainer;
    public BuildObject currentBuildObject { get; private set; }
    [SerializeField] private GameObject buildTemplate;
    [SerializeField] private GameObject backBuildTemplate;
    public BuildCatalog buildCatalog;
    [SerializeField] private GameObject placeholderPrefab;
    private GameObject placeholder;
    [SerializeField] private GameObject destroyParticlesPrefab;

    float holdToDestroyTime = 0.2f;

    [SerializeField] private MenuManager menuManager;
    [SerializeField] private InventoryManager plrInv;
    public static BuildingSystem Instance;
    bool building;
    [SerializeField] private GameObject shipPrefab;

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

    void PlaceObject(Vector3 worldPos, BuildGrid thisGrid, Transform parent)
    {
        GameObject obj = PlaceBlock(worldPos, currentBuildObject, parent);
        // Save object placement in the grid
        BuildObject buildObjectCopy = currentBuildObject.Clone();
        buildObjectCopy.gridObject = obj;
        thisGrid.SetValueAtPosition(worldPos, buildObjectCopy);
        // Deplete materials
        plrInv.DepleteAll(currentBuildObject.build.materials);
    }
    GameObject PlaceBlock(Vector3 worldPos, BuildObject thisBuildObject, Transform parent)
    {
        if (thisBuildObject.build == null) return null;
        // Determine the object template to use
        Build thisBuild = thisBuildObject.build;
        Rotation thisRotation = thisBuildObject.GetRotation();
        GameObject clone = thisRotation.Object;
        bool isObject = clone != null;
        if (!isObject)
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

        if (!isObject)
        {
            obj.GetComponent<SpriteRenderer>().sprite = thisRotation.sprite;
            if (thisBuild.depth == Build.DepthLevel.MidGround)
            {
                PolygonCollider2D collider = obj.AddComponent<PolygonCollider2D>();
                collider.usedByComposite = true;
            }
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

        if (buildObj == null || (!deleteTerrain && buildObj.build == null) || !thisGrid.RemoveValueAtPosition(worldPos))
        {
            return false;
        }

        if (buildObj.gridObject != null)
        {
            // Delete object from world
            Destroy(buildObj.gridObject);
        }
        if (buildObj.build != null)
        {
            // Recover materials
            plrInv.AddAll(buildObj.build.materials);
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

    public Ship GetShipAtPosition(Vector2 pos)
    {
        foreach (Ship ship in ShipBuilding.loadedShips)
        {
            BuildGrid shipGrid = ship.ship;
            if (shipGrid.PositionIsWithinGrid(pos))
            {
                return ship;
            }
        }
        return null;
    }

    public Ship SpawnShip(BuildGrid ship, Vector2 spawnPos)
    {
        Ship newShip = Instantiate(shipPrefab, spawnPos, Quaternion.identity).GetComponent<Ship>();
        newShip.SetUpShip(ship.Clone(false));
        return newShip;
    }
    public Ship RespawnShipAtPlayer(BuildGrid ship, Vector2 lastPlayerPos)
    {
        // Recalculate ship position
        GameObject plr = GameObject.FindGameObjectWithTag("Player");
        Vector2 spawnPos = (Vector2)plr.transform.position + (ship.bottomLeft - lastPlayerPos);

        // Spawn the ship objects
        Ship newShip = SpawnShip(ship, spawnPos);

        // Sit the player if was seating before
        BuildObject objectAtPlayer = newShip.ship.GetValueAtPosition(plr.transform.position);
        if (objectAtPlayer != null && objectAtPlayer.build.name == "Control Seat")
        {
            Seat seat = objectAtPlayer.gridObject?.GetComponent<Seat>();
            if (seat != null) seat.Interact();
        }
        return newShip;
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
    private void LateUpdate()
    {
        if (!building) return;

        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        BuildGrid selectedGrid = worldGrid;
        Transform selectedParent = objectsContainer;

        Ship thisShip = GetShipAtPosition(mousePos);
        if (thisShip != null)
        {
            selectedParent = thisShip.transform;
            selectedGrid = thisShip.ship;
        }

        Vector3 alignedPos = selectedGrid.WorldtoAligned(mousePos);

        bool hasBuild = currentBuildObject.build != null;
        bool spaceAvailable = selectedGrid.GetValueAtPosition(alignedPos) == null;
        bool hasAdjacent = true;
        bool hasMaterials = hasBuild && plrInv.HasAll(currentBuildObject.build.materials);
        if (!InVirtualHangar)
        {
            hasAdjacent = selectedGrid.PositionHasAdjacent(mousePos);
        }
        bool canPlace = hasBuild && spaceAvailable && hasAdjacent && hasMaterials;

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
                if (canPlace)
                {
                    PlaceObject(alignedPos, selectedGrid, selectedParent);
                    if (thisShip != null)
                        thisShip.UpdateShip(alignedPos);
                }
            }
            else if (isDeleting)
            {
                //bool willCollapse = selectedGrid.CheckCollapseOnDelete(alignedPos);
                bool willCollapse = false;
                if (!willCollapse && DeleteObject(alignedPos, selectedGrid))
                {
                    if (thisShip != null)
                        thisShip.UpdateShip(alignedPos);
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
