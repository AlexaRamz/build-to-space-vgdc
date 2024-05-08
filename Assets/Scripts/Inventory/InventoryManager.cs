using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "InventoryManager", menuName = "Scriptable Objects/Managers/Inventory Manager")]
public class InventoryManager : ScriptableObject
{
    public List<ItemAmountInfo> items = new List<ItemAmountInfo>();
    public Item currentItem;

    public List<ToolData> tools = new List<ToolData>();
    public Tool currentTool;

    public int money;
    [SerializeField] GameObject collectablePrefab;

    public List<Resource> resources;

    public event UnityAction holdEvent;
    public event UnityAction cancelHoldEvent;

    private Transform holdOrigin;

    public void AddItem(Item item, int amount)
    {
        ItemAmountInfo info = GetItemInfo(item);
        if (info != null)
        {
            info.amount += amount;
        }
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
    public bool DepleteItem(Item item, int amount)
    {
        ItemAmountInfo info = GetItemInfo(item);
        if (info != null && info.amount >= amount)
        {
            info.amount -= amount;
            return true;
        }
        return false;
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
            holdOrigin.GetComponent<SpriteRenderer>().sprite = info.item.image;
            holdEvent?.Invoke();
        }
    }
    public void DeselectItem()
    {
        currentItem = null;
        holdOrigin.GetComponent<SpriteRenderer>().sprite = null;
        cancelHoldEvent?.Invoke();
    }
    public void SetHoldOrigin(Transform holdOrigin)
    {
        this.holdOrigin = holdOrigin;
    }
    public void Drop()
    {
        if (currentItem != null && holdOrigin != null && DepleteItem(currentItem, 1))
        {
            GameObject obj = Instantiate(collectablePrefab, holdOrigin.transform.position, Quaternion.identity);
            obj.GetComponent<Collectable>().SetItem(currentItem);
        }
    }

    /* TOOLS-WEAPONS */
    public void Equip(int index)
    {
        if (holdOrigin == null) return;
        ClearEquip();
        if (index >= 0 && index < tools.Count && tools[index] != null)
        {
            ToolData data = tools[index];
            //Debug.Log(data.Name);
            if (InitializeTool(data, holdOrigin))
            {
                holdEvent?.Invoke();
            }
            else
            {
                Debug.Log("Failed to equip!");
            }
        }
        else
        {
            cancelHoldEvent?.Invoke();
        }
    }
    void ClearEquip()
    {
        if (holdOrigin == null) return;
        foreach (Transform child in holdOrigin.transform)
        {
            Destroy(child.gameObject);
        }
        currentTool = null;
    }
    bool InitializeTool(ToolData data, Transform origin)
    {
        if (data.prefab != null)
        {
            GameObject obj = Instantiate(data.prefab, origin.position, Quaternion.identity);
            obj.transform.parent = origin;
            obj.name = data.name;
            obj.transform.localScale = new Vector3(1, 1, 1);
            Tool tool = obj.GetComponent<Tool>();
            if (tool != null)
            {
                tool.data = data;
                currentTool = tool;
            }
            return true;
        }
        return false;
    }
    void AddTool(ToolData tool)
    {
        tools.Add(tool);
    }

    /* SHOP ITEMS */
    public void ApplyShopItem(ShopItem item)
    {
        Debug.Log(item.GetType());
        if (item is ToolData)
        {
            AddTool((ToolData)item);
        }
        // Future: powerups
    }
}
