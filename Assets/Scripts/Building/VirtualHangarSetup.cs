using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VirtualHangarSetup : MonoBehaviour
{
    BuildingSystem buildManager;
    void Start()
    {
        // Open the building menu on start
        buildManager = transform.parent.parent.Find("BuildingSystem").GetComponent<BuildingSystem>();
        buildManager.StartBuilding();
    }
}
