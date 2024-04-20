using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Drill : MonoBehaviour
{
	public bool outRight;
	private bool onOre=false;
	private float oreRichness;
	public float dropFrequency;
	private float timeTo=2;
	private GameObject dropTemplate;
	private Vector2 dropPosition;
	
    // Start is called before the first frame update
    void Start()
    {
		//location to check for resource source
		GameObject source = null;
		Vector2 cp = transform.position;
		cp.y -= 1.5f;
		Collider2D[] nearbyColliders = Physics2D.OverlapCircleAll(cp, 1.5f);
        foreach (Collider2D collider in nearbyColliders)
        {
			if (collider.tag == "Source")
			{
				onOre = true;
				source = collider.gameObject;
			}
		}
		if (onOre)
		{
			oreRichness = source.GetComponent<OreSource>().richness;
			dropTemplate = source.GetComponent<OreSource>().resouceType;
		}
		dropPosition = transform.position;
		dropPosition.y -= 1f; 
		if (outRight)
		{
			dropPosition.x += 2f; 
		} else {
			dropPosition.x -= 2f;
		}
    }

    // Update is called once per frame
    void Update()
    {
		if (GetComponent<Power>().generator != null)
		{
			timeTo -= (Time.deltaTime/oreRichness)*GetComponent<Power>().power;
			if (timeTo <= 0 && onOre)
			{
				GameObject obj = Instantiate(dropTemplate, dropPosition, Quaternion.identity);
				timeTo = dropFrequency;
			}
		}
    }
}
