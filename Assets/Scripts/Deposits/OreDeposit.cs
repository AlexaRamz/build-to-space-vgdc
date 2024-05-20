using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "RecipeManager", menuName = "Scriptable Objects/Resource Deposit")]
public class ResourceDeposit : ScriptableObject
{
	public Item resouceInfo;
	public float richness;
	public Sprite image;
}
