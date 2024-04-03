using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingSystem : MonoBehaviour
{
    public BuildGrid buildGrid;
    [SerializeField] Transform objectsContainer;
    [SerializeField] private BuildObject currentBuildObject;
    [SerializeField] private GameObject buildTemplate;
    [SerializeField] private GameObject backBuildTemplate;
    public BuildCatalog buildCatalog;
    [SerializeField] private GameObject placeholderPrefab;
    private GameObject placeholder;
    [SerializeField] private GameObject destroyParticlesPrefab;

    float holdToDestroyTime = 0.2f;

    MenuManager menuManager;
    BuildingUI buildUI;
    public static BuildingSystem Instance;

    private void Awake()
    {
        Instance = this;
        menuManager = MenuManager.Instance;
        menuManager.OnMenuClosed.AddListener(EndBuilding);
        buildUI = GetComponent<BuildingUI>();
    }
    private void Start()
    {
        buildGrid = new BuildGrid(100, 100, new Vector2Int(-25, -1));

        buildUI.SetCatalog(buildCatalog);

        if (objectsContainer == null)
        {
            Debug.Log("Building system error: Please assign the objects container");
        }
        placeholder = Instantiate(placeholderPrefab);
    }

    public void StartBuilding()
    {
        menuManager.ShowMenu(menuManager.buildMenu);
    }
    public void EndBuilding()
    {

    }

    public Build SetBuildObject(int index)
    {
        Build newBuild = buildCatalog.GetBuild(index);
        currentBuildObject.build = newBuild;
        currentBuildObject.rot = 0;
        UpdatePlaceholder();

        return newBuild;
    }
    public Category SetCategory(int index)
    {
        return buildCatalog.SetCategory(index);
    }

    Vector3 TileToWorldPos(Vector2Int pos)
    {
        return new Vector3(pos.x + 0.5f, pos.y + 0.5f, 0);
    }
    void PlaceObject(Vector2Int pos)
    {
        if (!buildGrid.IsWithinGrid(pos) || buildGrid.GetValue(pos) != null) return;

        // Determine the object template to use
        Build thisBuild = currentBuildObject.build;
        Rotation thisRotation = currentBuildObject.GetRotation();
        GameObject clone = thisRotation.Object;
        if (clone == null)
        {
            if (thisBuild.depth == Build.DepthLevel.MidGround)
                clone = buildTemplate;
            else
                clone = backBuildTemplate;
        }

        // Place the object in world
        Vector3 worldPos = TileToWorldPos(pos);
        GameObject obj = Instantiate(clone, Vector3.zero, objectsContainer.rotation, objectsContainer);
        obj.transform.localPosition = worldPos;
        obj.transform.localEulerAngles = new Vector3(0, 0, thisRotation.DegRotation);

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

        // Save object placement in the grid
        BuildObject buildObjectCopy = currentBuildObject.Clone();
        buildObjectCopy.gridObject = obj;
        buildGrid.SetValue(pos, buildObjectCopy);
    }
    void DeleteObject(Vector2Int pos)
    {
        BuildObject buildObj = buildGrid.GetValue(pos);
        if (!buildGrid.IsWithinGrid(pos) || !buildGrid.RemoveValue(pos)) return;

        // Delete object from world
        Destroy(buildObj.gridObject);

        // Particles
        Instantiate(destroyParticlesPrefab, TileToWorldPos(pos), Quaternion.identity);
    }
    void RotateObject()
    {
        currentBuildObject.AdvanceRotation();
        UpdatePlaceholder();
    }
    void UpdatePlaceholder()
    {
        Rotation thisRotation = currentBuildObject.GetRotation();
        SpriteRenderer renderer = placeholder.GetComponent<SpriteRenderer>();
        renderer.sprite = thisRotation.sprite;
        renderer.flipX = thisRotation.flipX;
        renderer.flipY = thisRotation.flipY;
        placeholder.transform.localRotation = Quaternion.Euler(0, 0, thisRotation.DegRotation);
    }

    IEnumerator deleteTimer;
    bool isPlacing;
    bool isDeleting;
    Vector2Int lastTilePos;

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
        if (objectsContainer == null) return;

        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2Int tilePos = new Vector2Int(Mathf.FloorToInt(mousePos.x), Mathf.FloorToInt(mousePos.y));

        bool canPlace = buildGrid.GetValue(tilePos) == null;
        bool onNewTile = tilePos != lastTilePos;
        lastTilePos = tilePos;

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

        if (canPlace && !isDeleting)
        {
            if (onNewTile)
            {
                placeholder.SetActive(true);
                placeholder.transform.position = TileToWorldPos(tilePos);
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
                PlaceObject(tilePos);
            }
            else if (isDeleting)
            {
                DeleteObject(tilePos);
            }
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            RotateObject();
        }

        // TO DO:
        // 3. Check has adjacent/no destroy separate from ground
        // 4. Update materials
    }
}
