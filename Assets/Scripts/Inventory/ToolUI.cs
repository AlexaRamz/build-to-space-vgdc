using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ToolUI : MonoBehaviour
{
    public Transform[] slots;
    [SerializeField] private InventoryManager plrInv;
    [SerializeField] private MenuManager menuManager;
    public static ToolUI Instance;

    void Awake()
    {
        Instance = this;
    }
    void OnEnable()
    {
        DisplayTools(plrInv.tools);
    }
    
    public void DisplayTools(List<ToolData> tools)
    {
        for (int i = 0; i < slots.Length; i++)
        {
            Image img = slots[i].transform.Find("Image").GetComponent<Image>();
            if (i < tools.Count && tools[i] != null)
            {
                img.sprite = tools[i].image;
                img.color = new Color32(255, 255, 255, 255);
            }
            else
            {
                img.sprite = null;
                img.color = new Color32(255, 255, 255, 0);
            }
        }
    }

    void ClearHover()
    {
        for (int i = 0; i < slots.Length; i++)
        {
            slots[i].GetComponent<Image>().color = new Color32(90, 90, 90, 255); 
        }
    }
    void HoverOn(int i)
    {
        if (i != hoveringOn)
        {
            ClearHover();
            hoveringOn = i;
            slots[hoveringOn].GetComponent<Image>().color = new Color32(255, 255, 255, 255);
        }
    }
    int hoveringOn = -1;
    void Update()
    {
        if (Input.GetMouseButtonUp(0))
        {
            plrInv.SelectTool(hoveringOn);
            menuManager.CloseCurrentMenu();
        }
        Vector2 mousePos = Input.mousePosition;

        float smallestDistance = Mathf.Infinity;
        int closest = -1;

        for (int i = 0; i < slots.Length; i++)
        {
            float distance = Vector2.Distance(mousePos, slots[i].position);
            if (distance < smallestDistance)
            {
                closest = i;
                smallestDistance = distance;
            }
        }
        HoverOn(closest);
    }
}
