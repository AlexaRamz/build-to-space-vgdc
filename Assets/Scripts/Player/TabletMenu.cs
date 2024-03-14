using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TabletMenu : MonoBehaviour, IMenu
{
    Canvas canvas;
    MenuManager menuManager;
    [SerializeField]
    private TextMeshProUGUI descriptionText;
    BuildingSystem buildManager;

    private void Start()
    {
        canvas = GetComponent<Canvas>();
        menuManager = transform.parent.GetComponent<MenuManager>();
        buildManager = transform.parent.parent.Find("BuildingSystem").GetComponent<BuildingSystem>();
    }
    public void OpenMenu()
    {
        open = canvas.enabled = true;
    }
    public void CloseMenu()
    {
        open = canvas.enabled = false;
    }
    public void ButtonHoverEnter(string appDesc)
    {
        descriptionText.text = appDesc;
    }
    public void ButtonHoverExit()
    {
        descriptionText.text = "";
    }
    public void BuildApp()
    {
        buildManager.StartBuilding();
    }

    bool open;
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.M))
        {
            if (!open) menuManager.OpenMenu(this);
            else menuManager.CloseMenu();
        }
    }
}
