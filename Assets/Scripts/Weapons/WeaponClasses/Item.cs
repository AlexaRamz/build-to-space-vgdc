using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Item", menuName = "Scriptable Objects/Item")]
public class Item : ScriptableObject, IHoldable, IPurchasable
{
    public string Name
    {
        get
        {
            return name;
        }
    }
    [SerializeField] private int _cost;
    public int cost
    {
        get
        {
            return _cost;
        }
    }
    [SerializeField] private string _description;
    public string description
    {
        get
        {
            return _description;
        }
    }
    [SerializeField] private Sprite _image;
    public Sprite image
    {
        get
        {
            return _image;
        }
    }
    [SerializeField] private Vector2 _holdPosition;
    public Vector2 holdPosition
    {
        get
        {
            return _holdPosition;
        }
    }
    public enum ItemCategory
    {
        None,
        Resource,
    }
    public ItemCategory category;
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

