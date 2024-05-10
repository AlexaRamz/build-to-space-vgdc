using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

[CreateAssetMenu(fileName = "MenuManager", menuName = "Scriptable Objects/Managers/Menu Manager")]
public class MenuManager : ScriptableObject
{
    public UnityEvent<string> openMenuEvent;
    public UnityEvent closeCurrentMenuEvent;

    public UnityEvent<string> menuOpenedEvent;
    public UnityEvent<string> menuClosedEvent;

    [HideInInspector] public bool isInMenu;

    public bool IsOnUI()
    {
        return EventSystem.current.IsPointerOverGameObject();
    }

    public void ShowMenu(string menuName)
    {
        openMenuEvent?.Invoke(menuName);
    }

    public void CloseCurrentMenu()
    {
        closeCurrentMenuEvent?.Invoke();
    }
}
