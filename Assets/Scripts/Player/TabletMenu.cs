using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TabletMenu : MonoBehaviour
{
    private Canvas canvas;
    [SerializeField] private MenuManager menuManager;
    [SerializeField] private TextMeshProUGUI descriptionText;

    private void Start()
    {
        canvas = GetComponent<Canvas>();
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
        menuManager.ShowMenu("Building");
    }
    public void InventoryApp()
    {
        menuManager.ShowMenu("Inventory");
    }
}
