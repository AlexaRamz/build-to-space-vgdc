using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Container : Interactable
{
	public Item itemInfo;
	public bool outRight;
	private bool Debounce;
	public bool lockResource = false;
	public int reserve;
	public int maxReserve;
	private float nextDrop;
	public float dropFrequency;
	public GameObject dropTemplate;
	private Vector2 dropPosition;
	public FactoryManager factoryManager;
	
	void OnTriggerEnter2D(Collider2D col)
    {
        if (!Debounce && col.gameObject.CompareTag("Collectable") && col.GetType() == typeof(CircleCollider2D) && reserve < maxReserve)
        {
			Debounce = true;
			if (reserve == 0 && !lockResource)
			{
				itemInfo = col.gameObject.GetComponent<Collectable>().itemInfo;
				transform.GetChild(0).GetComponent<SpriteRenderer>().sprite = itemInfo.image;
			}
			if (col.gameObject.GetComponent<Collectable>().itemInfo == itemInfo)
			{
				reserve += 1;
				Destroy(col.gameObject);
			}
        }
		Debounce = false;
    }

	void Start()
	{
		dropPosition = transform.position;
		if (outRight)
		{
			dropPosition.x += 1.5f; 
		} else {
			dropPosition.x -= 1.5f;
		}
	}

    void Update()
    {
		if (reserve > 0)
		{
			nextDrop -= Time.deltaTime;
			if (nextDrop <= 0)
			{
				GameObject obj = Instantiate(dropTemplate, dropPosition, Quaternion.identity);
				obj.GetComponent<Collectable>().SetItem(itemInfo);
				reserve -= 1;
				nextDrop = dropFrequency;
				if (reserve == 0)
				{
					transform.GetChild(0).GetComponent<SpriteRenderer>().sprite = null;
				}
			}	
		}
    }
	
	//add a openable menu to lock a certain material, and set drop rate
	public override void Interact()
	{
		factoryManager.ContainerMenu(this);
	}
}
