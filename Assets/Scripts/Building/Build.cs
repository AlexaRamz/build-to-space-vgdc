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
    public float DegRotation;
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
[CreateAssetMenu(fileName = "New Build", menuName = "Scriptable Objects/Build")]
public class Build : ScriptableObject
{
    public string description;
    public Rotation[] rotations;
    public List<ItemAmountInfo> materials = new List<ItemAmountInfo>();
    public enum DepthLevel
    {
        MidGround,
        Background,
        Foreground,
    }
    public DepthLevel depth;

    public Rotation GetRotation(int rot)
    {
        return rotations[rot];
    }
}