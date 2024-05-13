using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using TMPro;

public class FurnaceUI : MonoBehaviour
{
	public Furnace activeFurnace;
	public InventoryManager inventoryManager;
	public RecipeManager recipeManager;
	public FactoryManager factoryManager;
	public TMP_Dropdown dropdown;
	public TMP_Text Components;
	public Slider slider;
	private List<string> options;
	
    public void OnEnable()
    {
		factoryManager.openFurnaceEvent.AddListener(open);
    }

    public void OnDisable()
    {
		factoryManager.openFurnaceEvent.RemoveListener(open);
    }
	
	void Update()
	{
		Components.text = "";
		if (activeFurnace != null)
		{
			slider.maxValue = activeFurnace.dropFrequency;
			slider.value = activeFurnace.nextDrop;
		}
		foreach (ItemAmountInfo item in activeFurnace.recipe.components)
		{
			Components.text += item.item.name + ": " + activeFurnace.reserve[item.item].ToString() + " / " + item.amount.ToString() + System.Environment.NewLine;
		}
	}

    public void open(Furnace furnace)
    {
		activeFurnace = furnace;
		options = new List<string>();
		foreach (Recipe recipe in recipeManager.FurnaceRecipes)
		{
			if (recipe.tier <= activeFurnace.tier) { options.Add(recipe.name); }
		}
		dropdown.AddOptions(options);
		if (activeFurnace.recipe != null) { dropdown.value = options.IndexOf(activeFurnace.recipe.name)+1; }
	}
	
	public void changedDD()
	{
		Empty();
		foreach (Recipe recipe in recipeManager.FurnaceRecipes)
		{
			if (recipe.name == options[dropdown.value-1])
			{
				activeFurnace.recipe = recipe;
			}
		}
		activeFurnace.reserve = new Dictionary<Item, int>();
		foreach (ItemAmountInfo component in activeFurnace.recipe.components)
		{
			activeFurnace.reserve[component.item] = 0;
		}
	}
	
	public void Empty()
	{
		foreach (ItemAmountInfo item in activeFurnace.recipe.components)
		{
			transform.parent.parent.parent.GetComponent<PlayerManager>().AddToInventory(item.item, activeFurnace.reserve[item.item]);
			activeFurnace.reserve[item.item] = 0;
		}
	}
	
	//broken
	// public void Add()
	// {
		// foreach (ItemAmountInfo item in activeFurnace.recipe.components)
		// {
			// int addAmount = item.amount - activeFurnace.reserve[item.item]
			// if (addAmount > 0 && inventoryManager.DepleteItem(item.item, addAmount))
			// {
				// transform.parent.parent.parent.GetComponent<PlayerManager>().AddToInventory(item.item, -1*activeFurnace.reserve[item.item]);
				// activeFurnace.reserve[item.item] = item.amount;
			// }
		// }
	// }
}
