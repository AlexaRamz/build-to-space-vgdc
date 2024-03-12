using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Gun", menuName = "Scriptable Objects/Tool/Gun")]
public class GunData : ToolData
{
    public float fireCoolDown = 0.25f;
    public int bulletDamage = 1;
    [Range(1, 10)]
    public float bulletSpeed = 10f;
    [Range(1, 10)]
    public float bulletLifetime = 3f;
    public GameObject bulletPrefab;
}