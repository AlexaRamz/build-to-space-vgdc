using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuManagerUI : MonoBehaviour
{
    [System.Serializable]
    public class Menu
    {
        public string name;
        public GameObject menu;
    }
    public List<Menu> menus;
    private Menu currentMenu;

    [SerializeField] MenuManager menuManager;

    private void Start()
    {
        menuManager.ShowMenu("HUD");
        menuManager.isInMenu = false;
    }
    private void OnEnable()
    {
        menuManager.openMenuEvent.AddListener(ShowMenu);
        menuManager.closeCurrentMenuEvent.AddListener(CloseCurrentMenu);
    }
    private void OnDisable()
    {
        menuManager.openMenuEvent.RemoveListener(ShowMenu);
        menuManager.closeCurrentMenuEvent.RemoveListener(CloseCurrentMenu);
    }

    private void ShowMenu(string menuName)
    {
        Menu menu = FindMenuFromName(menuName);
        ShowMenu(menu);
    }

    private Menu FindMenuFromName(string menuName)
    {
        foreach (Menu m in menus)
        {
            if (m.name == menuName)
            {
                return m;
            }
        }
        return null;
    }

    private void ShowMenu(Menu menu)
    {
        if (menu == null) return;
        if (currentMenu != null)
        {
            currentMenu.menu.SetActive(false);
            menuManager.menuClosedEvent.Invoke(currentMenu.name);
        }
        if (menu.name == "HUD" || currentMenu != menu)
        {
            currentMenu = menu;
            currentMenu.menu.SetActive(true);
            menuManager.isInMenu = currentMenu.name != "HUD";

            menuManager.menuOpenedEvent.Invoke(currentMenu.name);
        }
        else
        {
            CloseCurrentMenu();
        }
    }

    private void CloseCurrentMenu()
    {
        ShowMenu(FindMenuFromName("HUD"));
    }
}
