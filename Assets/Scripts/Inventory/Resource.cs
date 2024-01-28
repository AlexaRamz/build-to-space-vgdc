using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Resource", menuName = "Resource")]
public class Resource : ScriptableObject
{
    public ResourceType type;
    public Sprite image;
}
[System.Serializable]
public class ResourceInfo
{
    public Resource resource;
    public int amount;
}
[System.Serializable]
public enum ResourceType
{
    Copper,
    Aluminum,
    Oil,
}
