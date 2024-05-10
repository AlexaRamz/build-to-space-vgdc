using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloseMenu : MonoBehaviour
{
    [SerializeField] private MenuManager menuManager;
    public void Close()
    {
        menuManager.CloseCurrentMenu();
    }
}
