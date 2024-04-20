using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Container : MonoBehaviour
{
	private ResourceType resource;
	public bool outRight;
	private bool Debounce;
	public float reserve;
	public float maxReserve;
	private float nextDrop;
	public float dropFrequency;
	private GameObject dropTemplate;
	private Vector2 dropPosition;
	public List<GameObject> templates; //could be optimized
	
	void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.CompareTag("Collectable") && reserve < maxReserve)
        {
			if (reserve == 0)
			{
				resource = col.gameObject.GetComponent<Collectable>().resource;
				transform.GetChild(0).GetComponent<SpriteRenderer>().sprite = col.gameObject.GetComponent<SpriteRenderer>().sprite;
				foreach (GameObject template in templates)
				{
					if (template.GetComponent<Collectable>().resource == resource)
					{
						dropTemplate = template;
					}
				}
			}
			if (!Debounce && col.gameObject.GetComponent<Collectable>().resource == resource)
			{
				Debounce = true;
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
		nextDrop -= Time.deltaTime;
		if (nextDrop <= 0 && reserve > 0)
		{
			GameObject obj = Instantiate(dropTemplate, dropPosition, Quaternion.identity);
			reserve -= 1;
			nextDrop = dropFrequency;
			if (reserve == 0)
			{
				transform.GetChild(0).GetComponent<SpriteRenderer>().sprite = null;
			}
		}	
    }
	
	//add a openable menu to lock a certain material, and set drop rate
}
