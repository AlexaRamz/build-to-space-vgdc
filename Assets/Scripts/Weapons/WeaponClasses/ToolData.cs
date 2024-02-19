using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ToolData : ScriptableObject
{
    public string Name;
    public Sprite sprite;
    public Vector2 holdPosition;
    public GameObject prefab;
}