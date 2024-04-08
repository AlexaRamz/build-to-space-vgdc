using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.SceneManagement;

public class ShipBuilding : MonoBehaviour
{
    public static List<BuildGrid> savedShips;
    [SerializeField] private GameObject shipPrefab;

    [SerializeField] private GameObject HangarActionButton;
    [SerializeField] private TMPro.TextMeshProUGUI HangarActionText;
    [SerializeField] private TMPro.TextMeshProUGUI HangarTransportText;

    public static bool InVirtualHangar => SceneManager.GetActiveScene().name == "VirtualHangar";
    BuildingSystem buildSys;

    private void Start()
    {
        buildSys = BuildingSystem.Instance;
        if (savedShips == null)
        {
            Debug.Log("Reset saved ships");
            savedShips = new List<BuildGrid>();
        }
    }
    public void SaveOrPasteShip()
    {
        if (InVirtualHangar) //The scene is currently the virtual hangar
        {
            SaveCurrentShip();
        }
        else
        {
            PasteSavedShip(false);
        }
    }
    public void EnterOrExitHangar()
    {
        if (InVirtualHangar) //The scene is currently the virtual hangar
        {
            LoadMainScene();
        }
        else
        {
            LoadVirtualHangar();
        }
    }
    private void SaveCurrentShip()
    {
        //Debug.Log("Saved a ship");
        savedShips.Add(buildSys.buildGrid.Clone(false));
    }
    private void PasteSavedShip(bool onMouse = true)
    {
        if (savedShips == null || savedShips.Count == 0) return;

        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector3 spawnPos = mousePos;
        if (!onMouse)
        {
            spawnPos = Camera.main.transform.position;
        }
        GameObject newShip = Instantiate(shipPrefab, (Vector2)spawnPos, Quaternion.identity);
        Ship ship = newShip.GetComponent<Ship>();
        BuildGrid save = savedShips.Last();
        ship.ship = save.Clone(false);
        ship.ship.ClampBounds(); //Clamp the ship size to the size of the cloned blocks

        newShip.transform.position -= new Vector3(ship.ship.width / 2, 0);
        //newShip.transform.position -= (Vector3)offset.RotatedBy(gameObject.transform.eulerAngles.z * Mathf.Deg2Rad); //this is a system for readjusting the position of the ship when new blocks are added. Right now it is very finicky
        //ship.ship.ShiftObjects(offset);

        //ship.ship.AddSize(1, 1); //Expand the ship size by 1 in each direction to allow placing around the ship
        //ship.ship.AddSize(-1, -1);

        buildSys.SpawnObjects(ship.ship, newShip.transform);
    }
    private void LoadVirtualHangar()
    {
        SceneManager.LoadScene("VirtualHangar"); //Go to the virtual hangar
    }
    private void LoadMainScene()
    {
        SceneManager.LoadScene("TutorialScene"); //Go to the main scene
    }
    private void LoadCharacterSelect()
    {
        SceneManager.LoadScene(0);
    }

    void Update()
    {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        int totalShips = Ship.LoadedShips.Count;
        // Debug.Log(totalShips);
        //bool PlaceOnFlatFloor = true;
        BuildGrid selectedBuildArray = buildSys.buildGrid;
        Ship selectedShip = null;
        ///This searches for ships that might be where the cursor is located, allowing the player to build on them instead.
        ///This should be reworked to let the play place on the closest ship to the mouse, just in case too many ships/builds are competing for player placement attention
        for (int i = 0; i < totalShips; i++)
        {
            Ship ship = Ship.LoadedShips[i];
            Vector2Int shipGridPos = ship.ConvertPositionToShipCoordinates(mousePos);
            bool withinBounds = ship.PositionInBounds(shipGridPos);
            if (withinBounds)
            {
                if (ship.ship.HasAdjacentFromGridPos(shipGridPos) || ship.ship.GetValue(shipGridPos) != null)
                {
                    //PlaceOnFlatFloor = false;
                    selectedBuildArray = ship.ship;
                    //gridPos = shipGridPos;
                    Vector2 shipWorldPos = ship.ConvertShipCoordinatesToPosition(shipGridPos);
                    //placeholder.transform.position = new Vector3(shipWorldPos.x, shipWorldPos.y, placeholder.transform.position.z);
                    //placeholder.transform.rotation = Quaternion.Euler(0, 0, ship.transform.rotation.eulerAngles.z + placeholderRotation);
                    selectedShip = ship;
                    break;
                }
            }
        }

        if (Input.GetKeyDown(KeyCode.C) && Input.GetKey(KeyCode.LeftControl)) //CTRL C to clone a ship
        {
            SaveCurrentShip();
        }
        if (Input.GetKeyDown(KeyCode.V) && Input.GetKey(KeyCode.LeftControl)) //CTRL V to paste a ship
        {
            PasteSavedShip(true);
        }
        if (Input.GetKey(KeyCode.G) && Input.GetKey(KeyCode.LeftControl)) //CTRL G to swap scenes to the shipbuilding scene
        {
            LoadMainScene();
        }
        if (Input.GetKey(KeyCode.H) && Input.GetKey(KeyCode.LeftControl))
        {
            LoadVirtualHangar();
        }
        if (Input.GetKey(KeyCode.L) && Input.GetKey(KeyCode.LeftControl))
        {
            LoadCharacterSelect();
        }
        if (InVirtualHangar)
        {
            HangarActionText.text = "Save Ship";
            HangarTransportText.text = "Exit Hangar";
        }
        else
        {
            HangarActionText.text = "Paste Ship";
            HangarTransportText.text = "Enter Hangar";
        }
    }
}
