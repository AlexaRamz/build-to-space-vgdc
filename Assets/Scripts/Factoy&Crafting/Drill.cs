using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Drill : Interactable
{
	public Item resouceInfo;
	public bool outRight;
	public bool onOre=false;
	public float oreRichness;
	public float dropFrequency;
	public float timeTo;
	public GameObject dropTemplate;
	private Vector2 dropPosition;
	public FactoryManager factoryManager;
	
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
			resouceInfo = source.GetComponent<OreSource>().resouceDepositInfo.resouceInfo;
			oreRichness = source.GetComponent<OreSource>().resouceDepositInfo.richness;
		}
		dropPosition = transform.position;
		dropPosition.y += 1f; 
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
			timeTo -= Time.deltaTime*oreRichness*GetComponent<Power>().power;
			if (timeTo <= 0 && onOre)
			{
				GameObject obj = Instantiate(dropTemplate, dropPosition, Quaternion.identity);
				obj.GetComponent<Collectable>().SetItem(resouceInfo);
				timeTo = dropFrequency;
			}
		}
    }
	
	public override void Interact()
	{
		factoryManager.DrillMenu(this);
	}
}
