using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Generator : Interactable
{
    public float power;
    public float radius;
    public float frquency;
	private float nextTime;
    public int consumption;
    public int reserve;
    public int maxReserve;
	private bool Debounce;
	private bool isFuel=false;
	public bool overdirve;
	private List<Power> machinesPowered; 
	public List<Item> fuels;
	public FactoryManager factoryManager;
	
	//colects fuel
	void OnTriggerEnter2D(Collider2D col)
    {
        if (!Debounce && col.gameObject.CompareTag("Collectable") && reserve < maxReserve)
        {
			Debounce = true;
			isFuel = false;
			foreach (Item fuel in fuels)
			{
				if (col.gameObject.GetComponent<Collectable>().itemInfo == fuel) { isFuel=true; }
			}
			if (isFuel)
			{
				reserve += 1;
				Destroy(col.gameObject);
			}
        }
		Debounce = false;
    }
	
    void Update()
    {
		if (reserve >= consumption && nextTime <= 0)
		{
			//if suficient reserve then check wich machines to power
			machinesPowered = new List<Power>();
			Collider2D[] nearbyColliders = Physics2D.OverlapCircleAll(transform.position, 15f);
			foreach (Collider2D collider in nearbyColliders)
			{
				//check if machine and isn't already powered by a more powerfull generator
				if (collider.tag == "Machine")
				{
					if (collider.GetComponent<Power>().generator == null || collider.GetComponent<Power>().generator == gameObject || collider.GetComponent<Power>().power < power/(machinesPowered.Count+1))
					{
						machinesPowered.Add(collider.GetComponent<Power>());
					}
				}
			}
			//determin how much power to give each machine, given by a square root  of power per machine
			if (machinesPowered.Count > 0)
			{
				float powerPerMachine = (float) System.Math.Sqrt((power*2.5f)/machinesPowered.Count);
				foreach (Power component in machinesPowered)
				{
					component.power = powerPerMachine;
					component.generator = gameObject;
				}
			}
			reserve -= consumption;
			nextTime = frquency;
		}
		if (overdirve) { nextTime -= Time.deltaTime*2.5f; }
		else { nextTime -= Time.deltaTime; } 
    }
	
	public override void Interact()
	{
		factoryManager.GeneratorMenu(this);
	}
}
