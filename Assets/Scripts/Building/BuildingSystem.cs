using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Category
{
    public string name;
    public Sprite image;
    public List<Build> builds = new List<Build>();
}
public class BuildingSystem : MonoBehaviour
{
    public static BuildingSystem Instance;
    public BuildingUI buildUI;
    public GameObject[,] gridArray; //for getting object
    public BuildInfo[,] infoArray; //for saving
    public List<Category> categories = new List<Category>(); //list of categories of craftable builds
    private Category currentCategory;

    private Inventory inv;
    private BuildInfo currentInfo;
    public GameObject placeholder;
    [HideInInspector] public bool building = false;
   
    public Transform buildObjectContainer;
    public GameObject buildTemplate;
    public GameObject backBuildTemplate;
    private const int width = 40;
    private const int height = 40;
    public GameObject destroyParticles;
    Vector2Int startCorner = new Vector2Int(-10, -1);

    private void Start()
    {
        Instance = this;
        inv = FindObjectOfType<Inventory>();
        gridArray = new GameObject[width, height];
        infoArray = new BuildInfo[width, height];
        currentInfo = new BuildInfo();
        SetupBuilding();
    }
    private void SetupBuilding()
    {
        buildUI.SetCategories(categories);
        buildUI.ChangeCategory(0);
        StartBuilding();
    }
    public void StartBuilding()
    {
        buildUI.StartBuilding();
        buildUI.OpenMenu();
        building = true;
    }
    public void EndBuilding()
    {
        PlaceHolderOff();
        building = false;
    }

    //***********MATERIALS************
    public void UpdateMaterials()
    {
        if (building)
        {
            buildUI.UpdateMaterials();
        }
    }

    //***********PLACING************
    public void StartPlacing(Build build)
    {
        currentInfo.build = build;
        currentInfo.rot = 0;
        PlaceHolderOn();
    }
    void EndPlacing()
    {
        currentInfo.build = null;
        currentInfo.rot = 0;
        PlaceHolderOff();
    }
    public Build GetBuild(int i)
    {
        return currentCategory.builds[i];
    }
    public Build GetBuild()
    {
        return currentInfo.build;
    }
    public void ChangeCategory(int i)
    {
        currentCategory = categories[i];
        buildUI.SetBuilds(currentCategory);
        EndPlacing();
    }
    void Place(Vector2Int gridPos, Vector3 worldPos)
    {
        GameObject Object;
        Rotation rotation = currentInfo.GetRotation();
        Build build = currentInfo.build;
        int currentRot = currentInfo.rot;

        if (rotation.Object == null)
        {
            if (build.depth == Build.DepthLevel.MidGround)
            {
                Object = Instantiate(buildTemplate, worldPos, Quaternion.identity);
            }
            else // build.depth == Build.DepthLevel.Background
            {
                Object = Instantiate(backBuildTemplate, worldPos, Quaternion.identity);
            }

            SpriteRenderer renderer = Object.GetComponent<SpriteRenderer>();
            renderer.sprite = rotation.sprite;
        }
        else
        {
            Object = Instantiate(rotation.Object, worldPos, Quaternion.identity);

        }
        Object.name = build.name;
        Object.transform.SetParent(buildObjectContainer);
        SetGrid(Object, gridPos, new BuildInfo { build = build, rot = currentRot });

        if (build.depth == Build.DepthLevel.MidGround)
        {
            PolygonCollider2D collider = Object.AddComponent<PolygonCollider2D>();
            collider.usedByComposite = true;
        }

        if (rotation.flipX)
        {
            Object.transform.localScale = new Vector3(-1, Object.transform.localScale.y, 1);
        }
        if (rotation.flipY)
        {
            Object.transform.localScale = new Vector3(Object.transform.localScale.x, -1, 1);
        }
        inv.DepleteMaterials(currentInfo.build.materials);

        BuildTrigger info = Object.AddComponent<BuildTrigger>();
        info.build = build;
        info.gridPos = gridPos;
    }


    //***********GRID************
    Vector3 GetWorldPos(Vector2 mousePos)
    {
        return new Vector3(Mathf.FloorToInt(mousePos.x) + 0.5f, Mathf.FloorToInt(mousePos.y) + 0.5f, 0);
    }
    Vector2Int GetGridPos(Vector2 mousePos)
    {
        return new Vector2Int(Mathf.FloorToInt(mousePos.x) - startCorner.x, Mathf.FloorToInt(mousePos.y) - startCorner.y);
    }
    GameObject GetGridObject(Vector2Int gridPos)
    {
        return gridArray[gridPos.x, gridPos.y];
    }
    BuildInfo GetGridInfo(Vector2Int gridPos)
    {
        return infoArray[gridPos.x, gridPos.y];
    }
    bool CheckGrid(Vector2Int gridPos)
    {
        return infoArray[gridPos.x, gridPos.y] != null;
    }
    bool IsWithinGrid(Vector2Int gridPos)
    {
        return gridPos.x >= 0 && gridPos.x < width && gridPos.y >= 0 && gridPos.y < height;
    }
    bool HasAdjacent(Vector2Int gridPos)
    {
        if (gridPos.y == 0) return true;
        Vector2Int[] adjShifts = { new Vector2Int(-1, 0), new Vector2Int(1, 0), new Vector2Int(0, -1), new Vector2Int(0, 1) };
        foreach (Vector2Int shift in adjShifts)
        {
            Vector2Int adjPos = gridPos + shift;
            if (IsWithinGrid(adjPos) && GetGridObject(adjPos) != null)
            {
                return true;
            }
        }
        return false;
    }
    void SetGrid(GameObject Object, Vector2Int gridPos, BuildInfo info)
    {
        gridArray[gridPos.x, gridPos.y] = Object;
        infoArray[gridPos.x, gridPos.y] = info;
    }


    //***********TEMPLATE************
    void SetTemplate()
    {
        Rotation thisRotation = currentInfo.GetRotation();
        SpriteRenderer renderer = placeholder.GetComponent<SpriteRenderer>();
        if (thisRotation.sprite == null && thisRotation.Object != null)
        {
            SpriteRenderer objRenderer = thisRotation.Object.GetComponent<SpriteRenderer>();
            renderer.sprite = objRenderer.sprite;
            renderer.flipX = objRenderer.flipX;
            renderer.flipY = objRenderer.flipY;
        }
        else
        {
            renderer.sprite = thisRotation.sprite;
            renderer.flipX = thisRotation.flipX;
            renderer.flipY = thisRotation.flipY;
        }
    }
    void PlaceHolderOn()
    {
        SpriteRenderer renderer = placeholder.GetComponent<SpriteRenderer>();
        SetTemplate();
        renderer.color = new Color32(255, 255, 255, 127);
        renderer.enabled = true;
    }
    void PlaceHolderOff()
    {
        SpriteRenderer renderer = placeholder.GetComponent<SpriteRenderer>();
        renderer.enabled = false;
        renderer.sprite = null;
    }


    //***********DESTROYING************
    IEnumerator DestroyParticles(GameObject particles)
    {
        yield return new WaitForSeconds(0.4f);
        Destroy(particles);
    }
    void DestroyObject(GameObject obj)
    {
        BuildTrigger info = obj.GetComponent<BuildTrigger>();
        inv.AddMaterials(info.build.materials);

        GameObject particles = Instantiate(destroyParticles, transform);
        particles.transform.position = obj.transform.position;
        particles.GetComponent<ParticleSystem>().Play();
        StartCoroutine(DestroyParticles(particles));

        SetGrid(null, info.gridPos, null);
        Destroy(obj);
    }


    //***********ROTATING************
    void RotateBuild()
    {
        currentInfo.AdvanceRotation();
        SetTemplate();
    }
    
    bool mouseDown;
    float counter;
    float minTime = 0.15f;
    bool canBuild = true;
    bool canDelete = true;

    void Update()
    {
        if (building)
        {
            if (Input.GetMouseButtonDown(0))
            {
                mouseDown = true;
            }
            if (Input.GetMouseButtonUp(0))
            {
                mouseDown = false;
                counter = 0;
                canBuild = true;
                canDelete = true;
            }

            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector2Int gridPos = GetGridPos(mousePos);
            Vector3 worldPos = GetWorldPos(mousePos);
            placeholder.transform.position = worldPos;
            if (IsWithinGrid(gridPos) && !buildUI.IsOnUI())
            {
                if (canBuild && currentInfo != null && currentInfo.build != null)
                {
                    if (Input.GetKeyUp("r"))
                    {
                        RotateBuild();
                    }
                    if (!CheckGrid(gridPos) && HasAdjacent(gridPos))
                    {
                        if (mouseDown && inv.CheckMaterials(currentInfo.build.materials))
                        {
                            PlaceHolderOff();
                            canDelete = false;
                            Place(gridPos, worldPos);
                        }
                        else
                        {
                            PlaceHolderOn();
                        }
                    }
                    else
                    {
                        PlaceHolderOff();
                    }
                }
                if (canDelete && mouseDown)
                {
                    GameObject obj = null;
                    if (CheckGrid(gridPos))
                    {
                        obj = GetGridObject(gridPos);
                    }
                    else
                    {
                        Vector2 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                        RaycastHit2D hit = Physics2D.Raycast(mouseWorldPos, Vector2.zero);
                        if (hit) obj = hit.collider.gameObject;
                    }
                    if (obj != null && obj.GetComponent<BuildTrigger>())
                    {
                        canBuild = false;
                        PlaceHolderOff();

                        counter += Time.deltaTime;
                        if (counter > minTime)
                        {
                            counter = 0;
                            DestroyObject(obj);
                            canBuild = false;
                        }
                    }
                }
            }
            else
            {
                PlaceHolderOff();
            }
        }
    }
}
