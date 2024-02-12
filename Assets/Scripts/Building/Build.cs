using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Rotation //essential a variant of sprite for the object
{
    public Sprite sprite;
    public GameObject Object;
    public bool flipX = false;
    public bool flipY = false;
    //public Vector2Int size = new Vector2Int(1, 1);

    public Sprite GetSprite()
    {
        if (sprite != null)
        {
            return sprite;
        }
        else if (Object != null)
        {
            return Object.GetComponent<SpriteRenderer>().sprite;
        }
        return null;
    }
}
[CreateAssetMenu(fileName = "New Build", menuName = "Build")]
public class Build : ScriptableObject
{
    public string description;
    public Rotation[] rotations;
    public List<ResourceAmount> materials = new List<ResourceAmount>();
    public enum DepthLevel
    {
        Background,
        MidGround,
        Foreground,
    }
    public DepthLevel depth;

    public Rotation GetRotation(int rot)
    {
        return rotations[rot];
    }
}
[System.Serializable]
public class ResourceAmount
{
    public ResourceType resource;
    public int amount = 1;
}

[System.Serializable]
public class BuildInfo
{
    public Build build;
    public int rot = 0;
    public Rotation GetRotation()
    {
        return build.rotations[rot];
    }
    public void AdvanceRotation()
    {
        rot = (rot + 1) % build.rotations.Length;
    }
}