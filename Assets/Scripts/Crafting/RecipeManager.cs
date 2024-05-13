using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.Events;

[System.Serializable]
public class Recipe
{
	public int tier;
    public string name;
    public Sprite image;
	[SerializeField] public List<ItemAmountInfo> components;
	[SerializeField] public List<ItemAmountInfo> products;
}

[System.Serializable]
[CreateAssetMenu(fileName = "RecipeManager", menuName = "Scriptable Objects/Managers/Recipe Manager")]
public class RecipeManager : ScriptableObject
{
    [SerializeField] public List<Recipe> FurnaceRecipes;
	[SerializeField] public List<Recipe> AsembelerRecipes;
	[SerializeField] public List<Recipe> RefinaryRecipes;
	
	//add thing for UI and machine
}
