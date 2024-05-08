using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Item", menuName = "Scriptable Objects/Item")]
public class Item : ShopItem
{
    // An item is an object that can be held
    public Vector2 holdPosition;
}
[System.Serializable]
public class ItemAmountInfo
{
    public Item item;
    public int amount = 1;

    public ItemAmountInfo(Item item, int amount)
    {
        this.item = item;
        this.amount = amount;
    }
    public ItemAmountInfo Clone()
    {
        return new ItemAmountInfo(item, amount);
    }
}

