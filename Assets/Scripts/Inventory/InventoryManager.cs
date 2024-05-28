using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "InventoryManager", menuName = "Scriptable Objects/Managers/Inventory Manager")]
public class InventoryManager : ScriptableObject
{
    public List<ItemAmountInfo> starterItems = new List<ItemAmountInfo>();
    public List<ItemAmountInfo> items { get; private set; } = new List<ItemAmountInfo>();
    public Item currentItem { get; private set; }

    public List<ToolData> starterTools = new List<ToolData>();
    public List<ToolData> tools { get; private set; } = new List<ToolData>();
    public ToolData currentTool { get; private set; }

    public int money;
    public int researchPoints;

    [SerializeField] GameObject collectablePrefab;

    public List<Resource> resources;

    public event UnityAction<Item> holdItemEvent;
    public event UnityAction holdAnimEvent;
    public event UnityAction<ToolData> equipToolEvent;
    public event UnityAction toolAnimEvent;
    public event UnityAction cancelHoldEvent;

    public event UnityAction inventoryModifiedEvent;


    private void OnEnable()
    {
        items.Clear();
        tools.Clear();
        money = researchPoints = 0;
        currentItem = null;
        currentTool = null;

        foreach (var i in starterItems)
        {
            items.Add(i.Clone());
        }
        foreach (var i in starterTools)
        {
            tools.Add(i);
        }
    }

    public void AddItem(Item item, int amount)
    {
        ItemAmountInfo info = GetItemInfo(item);
        if (info != null)
        {
            info.amount += amount;
        }
        else
        {
            items.Add(new ItemAmountInfo(item, amount));
        }
        inventoryModifiedEvent?.Invoke();
    }
    public void AddAll(List<ItemAmountInfo> items)
    {
        foreach(ItemAmountInfo info in items)
        {
            AddItem(info.item, info.amount);
        }
    }
    public bool DepleteItem(Item item, int amount)
    {
        ItemAmountInfo info = GetItemInfo(item);
        if (info != null && info.amount >= amount)
        {
            info.amount -= amount;
            inventoryModifiedEvent?.Invoke();
            return true;
        }
        return false;
    }
    public bool DepleteAll(List<ItemAmountInfo> items)
    {
        if (!HasAll(items)) return false;
        foreach (ItemAmountInfo info in items)
        {
            DepleteItem(info.item, info.amount);
        }
        return true;
    }
    ItemAmountInfo GetItemInfo(Item item)
    {
        foreach (ItemAmountInfo info in items)
        {
            if (info.item == item)
            {
                return info;
            }
        }
        return null;
    }
    public int GetItemAmount(Item item)
    {
        ItemAmountInfo info = GetItemInfo(item);
        if (info != null)
        {
            return info.amount;
        }
        return 0;
    }

    public bool HasItem(Item item)
    {
        return GetItemInfo(item) != null;
    }
    public bool HasEnough(Item item, int amount)
    {
        ItemAmountInfo info = GetItemInfo(item);
        return info != null && info.amount >= amount;
    }
    public bool HasAll(List<ItemAmountInfo> items)
    {
        foreach (ItemAmountInfo info in items)
        {
            if (!HasEnough(info.item, info.amount)) return false;
        }
        return true;
    }

    public Item GetResourceFromTile(TileBase tile)
    {
        foreach (Resource r in resources)
        {
            if (r.tile == tile)
            {
                return r;
            }
        }
        return null;
    }
    public ItemAmountInfo GetItemFromIndex(int index)
    {
        if (index >= 0 && index < items.Count)
        {
            return items[index];
        }
        return null;
    }

    public void SelectItem(int index)
    {
        ItemAmountInfo info = GetItemFromIndex(index);
        if (info != null)
        {
            currentItem = info.item;
            DeselectTool();
            holdItemEvent?.Invoke(info.item);
            holdAnimEvent?.Invoke();
        }
    }
    public void DeselectItem()
    {
        currentItem = null;
        cancelHoldEvent?.Invoke();
    }

    /* TOOLS-WEAPONS */
    void AddTool(ToolData tool)
    {
        tools.Add(tool);
    }
    ToolData GetToolFromIndex(int index)
    {
        if (index >= 0 && index < tools.Count)
        {
            return tools[index];
        }
        return null;
    }
    public void SelectTool(int index)
    {
        ToolData newTool = GetToolFromIndex(index);
        if (newTool != null && newTool != currentTool)
        {
            currentTool = newTool;
            DeselectItem();
            equipToolEvent?.Invoke(currentTool);
            toolAnimEvent?.Invoke();
        }
        else
        {
            DeselectTool();
        }
    }
    public void DeselectTool()
    {
        currentTool = null;
        cancelHoldEvent?.Invoke();
    }
    /* SHOP ITEMS */
    public void ApplyShopItem(IPurchasable item)
    {
        Debug.Log(item.GetType());
        if (item is ToolData)
        {
            AddTool((ToolData)item);
        }
        // Future: powerups
    }
}
