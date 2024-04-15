using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.SceneManagement;

public class ShipBuilding : MonoBehaviour
{
    public static List<BuildGrid> savedShips;
    [SerializeField] private GameObject shipPrefab;
    public static List<Ship> loadedShips = new List<Ship>();

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
        savedShips.Add(buildSys.worldGrid.Clone(false));
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
        Ship newShip = Instantiate(shipPrefab, (Vector2)spawnPos, Quaternion.identity).GetComponent<Ship>();
        BuildGrid save = savedShips.Last();
        newShip.SetUpShip(save.Clone(false));
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
