using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IPurchasable
{
    public string Name { get; }
    public int cost { get;}
    public Sprite image { get; }
    public string description { get;}
}
