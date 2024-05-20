using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TabletMenu : MonoBehaviour
{
    [SerializeField] private MenuManager menuManager;
    [SerializeField] private TextMeshProUGUI descriptionText;

    public void ButtonHoverEnter(string appDesc)
    {
        descriptionText.text = appDesc;
    }
    public void ButtonHoverExit()
    {
        descriptionText.text = "";
    }
    public void OpenApp(string appName)
    {
        menuManager.ShowMenu(appName);
    }
}
