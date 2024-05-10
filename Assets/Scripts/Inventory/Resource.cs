using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu(fileName = "New Resource", menuName = "Scriptable Objects/Items/Resource")]
public class Resource : Item
{
    public TileBase tile;
}
