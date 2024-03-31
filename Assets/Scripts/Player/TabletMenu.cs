using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TabletMenu : MonoBehaviour
{
    Canvas canvas;
    MenuManager menuManager;
    [SerializeField]
    private TextMeshProUGUI descriptionText;

    private void Start()
    {
        canvas = GetComponent<Canvas>();
        menuManager = MenuManager.Instance;
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
        menuManager.ShowMenu(menuManager.buildMenu);
    }
}
