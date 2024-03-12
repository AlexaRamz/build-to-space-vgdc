using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ShopItem : ScriptableObject
{
    public Sprite image;
    public string description;
    public int cost;
}
