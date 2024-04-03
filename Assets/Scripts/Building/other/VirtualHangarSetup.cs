using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VirtualHangarSetup : MonoBehaviour
{
    MenuManager menuManager;
    void Start()
    {
        // Open the building menu on start
        menuManager = MenuManager.Instance;
        menuManager.ShowMenu(menuManager.buildMenu);
    }
}
