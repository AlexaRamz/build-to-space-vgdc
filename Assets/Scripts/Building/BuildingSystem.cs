using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using Unity.PlasticSCM.Editor.WebApi;
using UnityEditor.Build;
using UnityEngine;
using UnityEngine.SceneManagement;

[System.Serializable]
public class Category
{
    public string name;
    public Sprite image;
    public List<Build> builds = new List<Build>();
}
public struct Tile
{
    public Transform transform
    { 
        get 
        {
            if (obj == null)
                throw new System.Exception("Cannot get transform of an empty tiles");
            return obj.transform; 
        }
    }
    public Tile(bool n = false)
    {
        obj = null;
        info = null;
    }
    public bool HasTile => obj != null;
    public bool HasInfo => info != null;
    public GameObject obj;
    public BuildInfo info;
}
public class BuildArray
{
    public BuildArray Copy()
    {
        BuildArray ba = new BuildArray(parent, Width, Height, 0, 0);
        for (int i = 0; i < Width; i++)
        {
            for (int j = 0; j < Height; j++)
            {
                if (tile[i, j].HasInfo)
                {
                    ba.tile[i, j].info = new BuildInfo()
                    {
                        build = tile[i, j].info.build,
                        rot = tile[i, j].info.rot
                    };
                }
            }
        }
        return ba;
    }
    public BuildArray Clone(GameObject parent, out Vector2Int offset, out int width, out int height)
    {
        BuildArray ba = new BuildArray(parent, Width, Height, 0, 0);
        int startX = Width;
        int startY = Height;
        int endX = 0;
        int endY = 0;
        for (int i = 0; i < Width; i++)
        {
            for (int j = 0; j < Height; j++)
            {
                if (tile[i,j].HasInfo)
                {
                    if(i < startX)
                    {
                        startX = i;
                    }
                    if (j < startY)
                    {
                        startY = j;
                    }
                    if(i > endX)
                    {
                        endX = i;
                    }
                    if (j > endY)
                    {
                        endY = j;
                    }
                    ba.PlaceBlock(i, j, tile[i, j].info.build, tile[i, j].info.GetRotation());
                }
                else
                    ba.tile[i, j] = tile[i, j];
            }
        }
        offset = new Vector2Int(startX, startY);
        width = endX - startX + 1;
        height = endY - startY + 1;
        return ba;
    }
    public BuildArray(GameObject parent, int width, int height, int x, int y)
    {
        this.parent = parent;
        position = new Vector3(x, y);
        tile = new Tile[width, height];
    }
    public BuildArray(GameObject parent, Vector2Int size, Vector3 position)
    {
        this.parent = parent;
        this.position = position;
        tile = new Tile[size.x, size.y];
    }
    public GameObject parent;
    public Vector3 position;
    public int Width => tile.GetLength(0);
    public int Height => tile.GetLength(1);
    public Tile[,] tile; //for getting object
    public Vector3 GetWorldPos(Vector2 mousePos)
    {
        return new Vector3(Mathf.FloorToInt(mousePos.x) + 0.5f, Mathf.FloorToInt(mousePos.y) + 0.5f, 0);
    }
    public Vector2Int GetGridPos(Vector2 mousePos)
    {
        return new Vector2Int(Mathf.FloorToInt(mousePos.x - position.x), Mathf.FloorToInt(mousePos.y - position.y));
    }
    public GameObject GetGridObject(Vector2Int gridPos)
    {
        return tile[gridPos.x, gridPos.y].obj;
    }
    public BuildInfo GetGridInfo(Vector2Int gridPos)
    {
        return tile[gridPos.x, gridPos.y].info;
    }
    public bool HasTile(Vector2Int gridPos)
    {
        return tile[gridPos.x, gridPos.y].HasTile;
    }
    public bool IsWithinGrid(Vector2Int gridPos)
    {
        return gridPos.x >= 0 && gridPos.x < Width && gridPos.y >= 0 && gridPos.y < Height;
    }
    public bool HasAdjacent(Vector2Int gridPos)
    {
        Vector2Int[] adjShifts = {
            new Vector2Int(-1, 0), new Vector2Int(1, 0), new Vector2Int(0, -1), new Vector2Int(0, 1)
        };
        foreach (Vector2Int shift in adjShifts)
        {
            Vector2Int adjPos = gridPos + shift;
            if (IsWithinGrid(adjPos) && HasTile(adjPos))
            {
                return true;
            }
        }
        return false;
    }
    public void SetGrid(GameObject Object, Vector2Int gridPos, BuildInfo info)
    {
        tile[gridPos.x, gridPos.y].obj = Object;
        tile[gridPos.x, gridPos.y].info = info;
    }
    public bool PlaceBlock(ref Tile[,] tile, int i, int j, Build build, Rotation rotation, Ship ship = null)
    {
        BuildingSystem bs = BuildingSystem.Instance;
        Vector3 pos = new Vector3(i, j) + position; //This block placing system needs to be unified with the placement system in BuildingSystem. But to do that would require a TON of refactoring...
        if (!tile[i, j].HasTile)
        {
            GameObject clone = rotation.Object;
            GameObject obj;
            if (clone == null)
            {
                if (build.depth == Build.DepthLevel.MidGround)
                    clone = bs.buildTemplate;
                else
                    clone = bs.backBuildTemplate;
            }
            obj = GameObject.Instantiate(clone, Vector3.zero, parent.transform.rotation, parent.transform);
            obj.transform.localPosition = pos;
            if (rotation.sprite != null)
            {
                SpriteRenderer renderer = obj.GetComponent<SpriteRenderer>();
                renderer.sprite = rotation.sprite;
            }
            tile[i, j].obj = obj;
            int myRot = 0;
            for(int a = 0; a < build.rotations.Length; a++)
            {
                if (build.rotations[a] == rotation)
                {
                    myRot = a;
                }
            }
            tile[i, j].info = new BuildInfo()
            {
                build = build, rot = myRot
            };
            

            if (bs.categories[0].builds[0].depth == Build.DepthLevel.MidGround)
            {
                PolygonCollider2D collider = obj.AddComponent<PolygonCollider2D>();
                collider.usedByComposite = true;
            }

            if (rotation.flipX)
            {
                obj.transform.localScale = new Vector3(-1, obj.transform.localScale.y, 1);
            }
            if (rotation.flipY)
            {
                obj.transform.localScale = new Vector3(obj.transform.localScale.x, -1, 1);
            }

            BuildTrigger info = obj.AddComponent<BuildTrigger>();
            info.build = build;
            info.gridPos = new Vector2Int(i, j);
            
            if(ship != null)
            {
                ship.AddSize(
                    i == 0 ? -1 : 0, 
                    j == 0 ? -1 : 0);
                ship.AddSize(
                    i >= ship.Width - 1 ? 1 : 0, 
                    j >= ship.Height - 1 ?  1 : 0);
            }
            return true;
        }
        return false;
    }
    public bool PlaceBlock(int i, int j, Build build, Rotation rotation, Ship ship = null)
    {
        return PlaceBlock(ref this.tile, i, j, build, rotation, ship);
    }
}
public class BuildingSystem : MonoBehaviour
{
    public static List<BuildArray> savedShips;
    public int width = 40; //This is the size of the world/placeable block area
    public int height = 40;
    public Vector2Int startCorner = new Vector2Int(-10, -1);

    public BuildArray world;
    public static BuildingSystem Instance;
    public BuildingUI buildUI;
    public List<Category> categories = new List<Category>(); //list of categories of craftable builds
    private Category currentCategory;

    private Inventory inv;
    [HideInInspector] public BuildInfo currentInfo;
    public GameObject placeholder;
    [HideInInspector] public bool building = false;
   
    public Transform buildObjectContainer;
    public GameObject buildTemplate;
    public GameObject backBuildTemplate;
    public GameObject destroyParticles;
    public GameObject shipPrefab;
    private void Start()
    {
        if(savedShips == null)
        {
            Debug.Log("Reset saved ships");
            savedShips = new List<BuildArray>();
        }
        Instance = this;
        inv = FindObjectOfType<Inventory>();
        world = new BuildArray(gameObject, new Vector2Int(width, height), (Vector2)startCorner);
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
    void Place(BuildArray bArray, Vector2Int gridPos, Ship ship)
    {
        Rotation rotation = currentInfo.GetRotation();
        if(bArray.PlaceBlock(gridPos.x, gridPos.y, currentInfo.build, rotation, ship))
            inv.DepleteMaterials(currentInfo.build.materials);
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

        world.SetGrid(null, info.gridPos, null);
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
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
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
            Vector2Int gridPos = world.GetGridPos(mousePos);
            Vector3 worldPos = world.GetWorldPos(mousePos);
            placeholder.transform.position = worldPos;
            placeholder.transform.rotation = Quaternion.identity;
            int totalShips = Ship.LoadedShips.Count;
            // Debug.Log(totalShips);
            bool PlaceOnFlatFloor = true;
            BuildArray selectedBuildArray = world;
            Ship selectedShip = null;
            for(int i = 0; i < totalShips; i++)
            {
                Ship ship = Ship.LoadedShips[i];
                Vector2Int shipGridPos = ship.ConvertPositionToShipCoordinates(mousePos);
                bool withinBounds = ship.PositionInBounds(shipGridPos);
                if(withinBounds)
                {
                    if(ship.ship.HasAdjacent(shipGridPos) || ship.ship.HasTile(shipGridPos))
                    {
                        PlaceOnFlatFloor = false;
                        selectedBuildArray = ship.ship;
                        gridPos = shipGridPos;
                        Vector2 shipWorldPos = ship.ConvertShipCoordinatesToPosition(shipGridPos);
                        placeholder.transform.position = new Vector3(shipWorldPos.x, shipWorldPos.y, placeholder.transform.position.z);
                        placeholder.transform.rotation = ship.transform.rotation;
                        selectedShip = ship;
                        break;
                    }
                }
            }
            if (selectedBuildArray.IsWithinGrid(gridPos) && !buildUI.IsOnUI())
            {
                if (canBuild && currentInfo != null && currentInfo.build != null)
                {
                    if (Input.GetKeyUp("r"))
                    {
                        RotateBuild();
                    }
                    if (!selectedBuildArray.HasTile(gridPos) && (selectedBuildArray.HasAdjacent(gridPos) || (PlaceOnFlatFloor && gridPos.y == 0)))
                    {
                        if (mouseDown && inv.CheckMaterials(currentInfo.build.materials))
                        {
                            PlaceHolderOff();
                            canDelete = false;
                            Place(selectedBuildArray, gridPos, selectedShip);
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
                    if (selectedBuildArray.HasTile(gridPos))
                    {
                        obj = selectedBuildArray.GetGridObject(gridPos);
                    }
                    else
                    {
                        Vector2 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                        RaycastHit2D hit = Physics2D.Raycast(mouseWorldPos, Vector2.zero);
                        if (hit)
                            obj = hit.collider.gameObject;
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
        if (Input.GetKeyDown(KeyCode.C) && Input.GetKey(KeyCode.LeftControl))
        {
            Debug.Log("Saved a ship");
            savedShips.Add(world.Copy());
        }
        if (Input.GetKeyDown(KeyCode.V) && Input.GetKey(KeyCode.LeftControl))
        {
            Debug.Log("Cloned a ship");
            GameObject newShip = Instantiate(shipPrefab, (Vector2)mousePos, Quaternion.identity);
            Ship ship = newShip.GetComponent<Ship>();
            BuildArray save = savedShips.Last();
            int minWidth;
            int minHeight;
            Vector2Int offset;
            ship.ship = save.Clone(newShip, out offset, out minWidth, out minHeight);
            ship.RB.mass = ship.Width * ship.Height;
            ship.SetBounds(minWidth, minHeight, -offset.x, -offset.y); //Clamp the ship size to the size of the cloned blocks
            ship.AddSize(1, 1); //Expand the ship size by 1 in each direction to allow placing around the ship
            ship.AddSize(-1, -1);
            //Debug.Log(save.tile[0, 0]);
        }
        if (Input.GetKey(KeyCode.G) && Input.GetKey(KeyCode.LeftControl))
        {
            SceneManager.LoadScene(4);
        }
    }
}
