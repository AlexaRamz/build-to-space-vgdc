using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    [SerializeField] private InventoryManager inventory;
    public Transform holdOrigin;
    PlayerInteraction plrInt;
    [SerializeField] private MenuManager menuManager;

    private void Start()
    {
        inventory.SetHoldOrigin(holdOrigin);
        plrInt = GetComponent<PlayerInteraction>();
    }
    public void AddToInventory(Item item, int amount)
    {
        inventory.AddItem(item, amount);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            inventory.Drop();
        }

        /* TOOLS-WEAPONS INPUT */
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            inventory.Equip(0);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            inventory.Equip(1);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            inventory.Equip(2);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            inventory.Equip(3);
        }

        if (plrInt.canInteract && inventory.currentTool != null && Input.GetMouseButton(0))
        {
            inventory.currentTool.Use();
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
}
