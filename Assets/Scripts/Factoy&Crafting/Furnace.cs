using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Furnace : Interactable
{
	public int tier;
	public Dictionary<Item, int> reserve;
	public bool outRight;
	private bool Debounce;
	public float maxReserve;
	public float nextDrop;
	public float dropFrequency;
	public GameObject dropTemplate;
	private Vector2 dropPosition;
	public RecipeManager recipes;
	public Recipe recipe;
	public FactoryManager factoryManager;
	
	void Start()
	{
		dropPosition = transform.position;
		dropPosition.y += 0.5f;
		if (outRight)
		{
			dropPosition.x += 2.5f; 
		} else {
			dropPosition.x -= 2.5f;
		}
		recipe = recipes.FurnaceRecipes[0];
		reserve = new Dictionary<Item, int>();
		foreach (ItemAmountInfo component in recipe.components)
		{
			reserve[component.item] = 0;
		}
	}

	void OnTriggerEnter2D(Collider2D col)
    {
        if (!Debounce && col.gameObject.CompareTag("Collectable") && col.GetType() == typeof(CircleCollider2D))
        {
			Debounce = true;
			if (reserve.ContainsKey(col.gameObject.GetComponent<Collectable>().itemInfo) && reserve[col.gameObject.GetComponent<Collectable>().itemInfo] < maxReserve)
			{
				reserve[col.gameObject.GetComponent<Collectable>().itemInfo] += 1;
				Destroy(col.gameObject);
			}
        }
		Debounce = false;
    }
	
    // Update is called once per frame
    void Update()
    {
		if (can())
		{
			nextDrop -= Time.deltaTime;
			if (nextDrop <= 0)
			{
				nextDrop = dropFrequency;
				foreach (ItemAmountInfo product in recipe.products)
				{
					for (int i = 0; i < product.amount; i++)
					{
						GameObject obj = Instantiate(dropTemplate, dropPosition, Quaternion.identity);
						obj.GetComponent<Collectable>().SetItem(product.item);
					}
				}
				foreach (ItemAmountInfo component in recipe.components)
				{
					reserve[component.item] -= component.amount;
				}
			}
		}
    }
	
	bool can()
	{
		foreach (ItemAmountInfo component in recipe.components)
		{
			if (reserve[component.item] < component.amount)
			{
				nextDrop = dropFrequency;
				return false;
			}
		}
		return true;
	}
	
	public override void Interact()
	{
		factoryManager.FurnaceMenu(this);
	}
}
