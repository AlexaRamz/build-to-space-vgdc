using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    [SerializeField] private InventoryManager inventory;
    public Transform holdOrigin;
    PlayerInteraction plrInt;
    [SerializeField] private MenuManager menuManager;
    [SerializeField] GameObject collectablePrefab;
    PlayerMovement plrMovement;

    private Tool activeTool;

    private void Start()
    {
        plrInt = GetComponent<PlayerInteraction>();
        plrMovement = GetComponent<PlayerMovement>();
    }
    private void OnEnable()
    {
        plrInt = GetComponent<PlayerInteraction>();
        inventory.holdItemEvent += HoldItem;
        inventory.equipToolEvent += EquipTool;
        inventory.cancelHoldEvent += CancelHold;
    }
    private void OnDisable()
    {
        inventory.holdItemEvent -= HoldItem;
        inventory.equipToolEvent -= EquipTool;
        inventory.cancelHoldEvent -= CancelHold;
    }
    public void AddToInventory(Item item, int amount)
    {
        inventory.AddItem(item, amount);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            DropItem();
        }

        /* TOOL INPUT */
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            NumberInput(1);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            NumberInput(2);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            NumberInput(3);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            NumberInput(4);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            NumberInput(5);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha6))
        {
            NumberInput(6);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha7))
        {
            NumberInput(7);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha8))
        {
            NumberInput(8);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha9))
        {
            NumberInput(9);
        }

        if (plrInt.canInteract && activeTool != null && Input.GetMouseButton(0))
        {
            activeTool.Use();
        }

        /* MENU INPUT */
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            menuManager.ShowMenu("ToolWheel");
        }
        else if (Input.GetKeyDown(KeyCode.P))
        {
            menuManager.ShowMenu("Shop");
        }
        else if (Input.GetKeyDown(KeyCode.B))
        {
            menuManager.ShowMenu("Building");
        }
        else if (Input.GetKeyDown(KeyCode.M))
        {
            menuManager.ShowMenu("Tablet");
        }
        else if (Input.GetKeyDown(KeyCode.N))
        {
            menuManager.ShowMenu("Inventory");
        }
        else if (Input.GetKeyDown(KeyCode.Q))
        {
            menuManager.ShowMenu("Quests");
        }
        else if (Input.GetKeyDown(KeyCode.RightShift) || Input.GetKeyDown(KeyCode.LeftShift))
        {
            menuManager.CloseCurrentMenu();
        }

        if (Input.GetKeyDown("up") || Input.GetKey("w"))
        {
            FlyTowardInput(0);
        }
        if (Input.GetKeyDown("right") || Input.GetKey("d"))
        {
            FlyTowardInput(270);
        }
        if (Input.GetKeyDown("down") || Input.GetKey("s"))
        {
            FlyTowardInput(180);
        }
        if (Input.GetKeyDown("left") || Input.GetKey("a"))
        {
            FlyTowardInput(90);
        }

        if (Input.GetKeyUp("up") || Input.GetKey("w"))
        {
            FlyTowardInputOff(0);
        }
        if (Input.GetKeyUp("right") || Input.GetKey("d"))
        {
            FlyTowardInputOff(270);
        }
        if (Input.GetKeyUp("down") || Input.GetKey("s"))
        {
            FlyTowardInputOff(180);
        }
        if (Input.GetKeyUp("left") || Input.GetKey("a"))
        {
            FlyTowardInputOff(90);
        }
    }
    void NumberInput(int input)
    {
        if (!plrMovement.sitting)
        {
            inventory.SelectTool(input - 1);
        }
        else
        {
            Ship ship = BuildingSystem.Instance.GetShipAtPosition(transform.position);
            if (ship != null)
            {
                ship.ToggleThrust(input - 1);
            }
        }
    }

    void FlyTowardInput(float degDirection)
    {
        if (plrMovement.sitting)
        {
            Ship ship = BuildingSystem.Instance.GetShipAtPosition(transform.position);
            if (ship != null)
            {
                ship.ApplyThrustTowards(degDirection, true);
            }
        }
    }
    void FlyTowardInputOff(float degDirection)
    {
        if (plrMovement.sitting)
        {
            Ship ship = BuildingSystem.Instance.GetShipAtPosition(transform.position);
            if (ship != null)
            {
                ship.ApplyThrustTowards(degDirection, false);
            }
        }
    }

    /* ITEM AND TOOL EQUIPPING */
    public void HoldItem(Item item)
    {
        holdOrigin.GetComponent<SpriteRenderer>().sprite = item.image;
    }
    void ClearItem()
    {
        holdOrigin.GetComponent<SpriteRenderer>().sprite = null;
    }
    public void CancelHold()
    {
        ClearItem();
        ClearEquip();
    }
    public void DropItem()
    {
        if (inventory.currentItem != null && inventory.DepleteItem(inventory.currentItem, 1))
        {
            GameObject obj = Instantiate(collectablePrefab, holdOrigin.transform.position, Quaternion.identity);
            obj.GetComponent<Collectable>().SetItem(inventory.currentItem);
        }
    }
    public void EquipTool(ToolData tool)
    {
        if (!InitializeTool(tool, holdOrigin))
        {
            Debug.Log("Failed to equip!");
        }
    }
    void ClearEquip()
    {
        activeTool = null;
        foreach (Transform child in holdOrigin.transform)
        {
            Destroy(child.gameObject);
        }
    }
    bool InitializeTool(ToolData data, Transform origin)
    {
        if (data != null && data.prefab != null)
        {
            GameObject obj = Instantiate(data.prefab, origin.position, Quaternion.identity);
            obj.transform.parent = origin;
            obj.name = data.name;
            obj.transform.localScale = new Vector3(1, 1, 1);
            Tool tool = obj.GetComponent<Tool>();
            if (tool != null)
            {
                tool.data = data;
                activeTool = tool;
            }
            return true;
        }
        return false;
    }
}
