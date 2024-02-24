using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ToolUI : MonoBehaviour, IMenu
{
    Canvas canvas;
    bool open;
    List<Transform> slots = new List<Transform>();
    Inventory plrInv;
    MenuManager menuManager;
    public static ToolUI Instance;

    void Awake()
    {
        Instance = this;
        foreach (Transform child in transform)
        {
            slots.Add(child);
        }
    }
    void Start()
    {
        canvas = GetComponent<Canvas>();
        plrInv = GameObject.Find("Player").GetComponent<Inventory>();
        menuManager = GameObject.Find("MenuManager").GetComponent<MenuManager>();
    }

    public void OpenMenu()
    {
        open = canvas.enabled = true;
    }

    public void CloseMenu()
    {
        open = canvas.enabled = false;
    }
    
    public void DisplayTools(List<ToolData> tools)
    {
        for (int i = 0; i < slots.Count; i++)
        {
            if (i < tools.Count)
            {
                Image img = slots[i].transform.Find("Image").GetComponent<Image>();
                img.sprite = tools[i].sprite;
                img.color = new Color32(255, 255, 255, 255);
            }
            else return;
        }
    }

    void ClearHover()
    {
        for (int i = 0; i < slots.Count; i++)
        {
            slots[i].GetComponent<Image>().color = new Color32(90, 90, 90, 255); 
        }
    }
    void HoverOn(int i)
    {
        hoveringOn = i;
        slots[hoveringOn].GetComponent<Image>().color = new Color32(255, 255, 255, 255);
    }
    int hoveringOn = -1;
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            menuManager.OpenMenu(this);
        }
        else if (open)
        {
            if (Input.GetKeyUp(KeyCode.Tab))
            {
                if (hoveringOn >= 0)
                {
                    plrInv.Equip(hoveringOn);
                }
                menuManager.CloseMenu();
            }
            Vector2 mousePos = Input.mousePosition;

            float smallestDistance = Mathf.Infinity;
            int closest = -1;

            for (int i = 0; i < slots.Count; i++)
            {
                float distance = Vector2.Distance(mousePos, slots[i].position);
                if (distance < smallestDistance)
                {
                    closest = i;
                    smallestDistance = distance;
                }
            }
            ClearHover();
            HoverOn(closest);
        }
    }
}
