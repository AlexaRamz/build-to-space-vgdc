using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuManager : MonoBehaviour
{
    // Handles button inputs when within a menu
    IMenu currentMenu;

    public bool IsInMenu()
    {
        return currentMenu != null;
    }
    public void OpenMenu(IMenu newMenu)
    {
        if (newMenu != currentMenu)
        {
            if (currentMenu != null)
            {
                currentMenu.CloseMenu();
            }
            currentMenu = newMenu;
            currentMenu.OpenMenu();
        }
    }
    public void CloseMenu()
    {
        if (IsInMenu())
        {
            currentMenu.CloseMenu();
            currentMenu = null;
        }
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.RightShift)) // Back button
        {
            CloseMenu();
        }
    }
}
