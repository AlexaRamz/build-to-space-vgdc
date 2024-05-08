using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VirtualHangarSetup : MonoBehaviour
{
    [SerializeField] private MenuManager menuManager;
    void Start()
    {
        // Open the building menu on start
        menuManager.ShowMenu("Building");
    }
}
