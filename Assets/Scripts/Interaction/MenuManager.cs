using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class MenuManager : MonoBehaviour
{
    // Handles button inputs when within a menu
    private IMenu selectedMenu;
    public bool IsInMenu()
    {
        return selectedMenu != null;
    }
    public void OpenMenu(IMenu newMenu)
    {
        if (newMenu != selectedMenu)
        {
            if (selectedMenu != null)
            {
                selectedMenu.CloseMenu();
            }
            selectedMenu = newMenu;
            selectedMenu.OpenMenu();
        }
    }
    public void CloseMenu()
    {
        if (IsInMenu())
        {
            selectedMenu.CloseMenu();
            selectedMenu = null;
        }
    }
    void Update()
    {
        bool shiftInput = Input.GetKeyDown(KeyCode.RightShift) || Input.GetKeyDown(KeyCode.LeftShift);
        if (shiftInput) // Back button
        {
            CloseMenu();
        }
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            //Debug.Log(BuildingSystem.Instance.building);
            if (BuildingSystem.Instance.building)
            {
                BuildingSystem.Instance.buildUI.EndBuilding();
            }
            else
            {
                BuildingSystem.Instance.StartBuilding();
            }
            //Debug.Log(BuildingSystem.Instance.building);
        }
    }
}
