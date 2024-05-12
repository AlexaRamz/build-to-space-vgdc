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

    private Tool activeTool;

    private void Start()
    {
        plrInt = GetComponent<PlayerInteraction>();
        inventory.holdItemEvent += HoldItem;
        inventory.equipToolEvent += EquipTool;
        inventory.cancelHoldEvent += CancelHold;
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

        /* TOOLS-WEAPONS INPUT */
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            inventory.SelectTool(0);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            inventory.SelectTool(1);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            inventory.SelectTool(2);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            inventory.SelectTool(3);
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
        else if (Input.GetKeyDown(KeyCode.RightShift))
        {
            menuManager.CloseCurrentMenu();
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
