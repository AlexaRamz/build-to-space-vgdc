using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToolData : ScriptableObject, IHoldable, IPurchasable
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
    public float activationCooldown = 0.25f;
    public GameObject prefab;
}