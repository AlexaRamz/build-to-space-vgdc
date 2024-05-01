using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToolData : Item
{
    // A tool is an item with a specific use, or requiring input, that may be added to the tool wheel
    public float activationCooldown = 0.25f;
    public GameObject prefab;
}