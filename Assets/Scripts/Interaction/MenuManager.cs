using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class MenuManager : MonoBehaviour
{
    public GameObject shopMenu;
    public GameObject weaponWheel;
    public GameObject buildMenu;
    public GameObject tabletMenu;
    public GameObject HUD;
    public GameObject dialogueBox;
    public GameObject questBox;

    private GameObject currentMenu;
    private Canvas canvas;
    
    public static MenuManager Instance;
    public bool isInMenu  = false;

    public UnityEvent OnMenuOpened;
    public UnityEvent OnMenuClosed;

    void Awake()
    {
        Instance = this;
        canvas = GetComponent<Canvas>();
        currentMenu = HUD;
        currentMenu.SetActive(true);
    }

    public void ShowMenu(GameObject menu)
    {
        currentMenu.SetActive(false);
        if (currentMenu != menu)
        {
            currentMenu = menu;
            currentMenu.SetActive(true);
            isInMenu = true;
            OnMenuOpened?.Invoke();
        }
        else
        {
            CloseCurrentMenu();
        }
    }

    public void CloseCurrentMenu()
    {
        currentMenu.SetActive(false);
        currentMenu = HUD;
        currentMenu.SetActive(true);
        isInMenu = false;
        OnMenuClosed?.Invoke();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
            ShowMenu(weaponWheel);
        if (Input.GetKeyDown(KeyCode.P))
            ShowMenu(shopMenu);
        if (Input.GetKeyDown(KeyCode.B))
            ShowMenu(buildMenu);
        if (Input.GetKeyDown(KeyCode.M))
            ShowMenu(tabletMenu);
        if (Input.GetKeyDown(KeyCode.O))
            ShowMenu(questBox);
        if (Input.GetKeyDown(KeyCode.RightShift))
            CloseCurrentMenu();
    }

    public bool IsOnUI()
    {
        return EventSystem.current.IsPointerOverGameObject();
    }
}
