using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryUI : MonoBehaviour
{
    int currentSelection = -1; // display list index
    int resourceSelection = 0; // true resources index

    bool open = false;
    [HideInInspector] public Inventory plrInv;
    public List<ResourceDisplay> resourceDisplays = new List<ResourceDisplay>();
    public Canvas menu;
    public Canvas interfaceMenu;

    void OpenSelecting()
    {
        open = true;
        resourceSelection = plrInv.currentResourceIndex;
        if (resourceSelection < 0 || resourceSelection >= plrInv.resources.Count)
        {
            resourceSelection = 0;
        }
        SetDisplays();
        ClearArrows();
        StartCoroutine(WaitOneFrame());
    }
    void FinishOpen()
    {
        currentSelection = 0;
        resourceDisplays[currentSelection].ArrowOn();
        InterfaceOff();
        menu.enabled = true;
    }
    public void InterfaceOn()
    {
        interfaceMenu.enabled = true;
    }
    public void InterfaceOff()
    {
        interfaceMenu.enabled = false;
    }
    IEnumerator WaitOneFrame() // to allow layout groups to update after setting displays
    {
        yield return 0;
        FinishOpen();
    }
    void CloseSelecting()
    {
        open = false;
        menu.enabled = false;
        interfaceMenu.enabled = true;
    }
    void ClearArrows()
    {
        foreach (ResourceDisplay display in resourceDisplays)
        {
            display.ArrowOff();
        }
    }
    int GetDisplayIndex(int resourceIndex) // returns the corresponding display index for the resource index
    {
        int i = resourceIndex - resourceSelection;
        if (i < 0)
        {
            i += plrInv.resources.Count;
        }
        return i;
    }
    int GetResourceIndex(int displayIndex) // returns the corresponding resource index for the display index
    {
        int i = resourceSelection + displayIndex;
        if (i >= plrInv.resources.Count)
        {
            i -= plrInv.resources.Count;
        }
        return i;
    }
    public void SetDisplays()
    {
        for (int i = 0; i < resourceDisplays.Count - 1; i++)
        {
            resourceDisplays[i].SetResource(plrInv.resources[GetResourceIndex(i)]);
        }
    }
    void CycleSelectionDown()
    {
        currentSelection++;
        if (currentSelection >= resourceDisplays.Count)
        {
            currentSelection = 0;
        }
    }
    void CycleSelectionUp()
    {
        currentSelection--;
        if (currentSelection < 0)
        {
            currentSelection = resourceDisplays.Count - 1;
        }
    }
    int GetResourceSelection()
    {
        int selection = resourceSelection + currentSelection;
        if (selection >= plrInv.resources.Count)
        {
            selection -= plrInv.resources.Count;
        }
        return selection;
    }
    void SetResource()
    {
        if (currentSelection >= resourceDisplays.Count - 1) // Cancel
        {
            plrInv.Hold(-1);
        }
        else
        {
            plrInv.Hold(GetResourceSelection());
        }
    }
    private void Update()
    {
        if (!open)
        {
            if (Input.GetKeyDown("c")) // Open menu
            {
                OpenSelecting();
            }
            else if (Input.GetKeyDown("z")) // Cycle list down
            {
                CycleSelectionUp();
                SetResource();
            }
            else if (Input.GetKeyDown("x")) // Cycle list up
            {
                CycleSelectionDown();
                SetResource();
            }
        }
        else if (open)
        {
            if (Input.GetKeyDown("c")) // Close menu
            {
                CloseSelecting();
            }
            if (Input.GetKeyDown("down")) // Cycle list down
            {
                ClearArrows();
                CycleSelectionDown();
                resourceDisplays[currentSelection].ArrowOn();
            }
            else if (Input.GetKeyDown("up")) // Cycle list up
            {
                ClearArrows();
                CycleSelectionUp();
                resourceDisplays[currentSelection].ArrowOn();
            }
            else if (Input.GetKeyDown(KeyCode.Return)) // Select
            {
                CloseSelecting();
                SetResource();
                resourceSelection = GetResourceSelection();
                currentSelection = 0;
            }
        }
    }
}
