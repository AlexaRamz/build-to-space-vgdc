using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using TMPro;

public class ContainerUI : MonoBehaviour
{
	public Container activeContainer;
	public InventoryManager inventoryManager;
	public FactoryManager factoryManager;
	public TMP_Dropdown dropdown;
	public TMP_InputField inputField;
	public Toggle lockResource;
	public Slider slider;
	private List<string> options;
	
    public void OnEnable()
    {
		factoryManager.openContainerEvent.AddListener(open);
    }

    public void OnDisable()
    {
		factoryManager.openContainerEvent.RemoveListener(open);
    }
	
	void Update()
	{
		if (activeContainer != null)
		{
			slider.maxValue = activeContainer.maxReserve;
			slider.value = activeContainer.reserve;
		}
	}

    public void open(Container container)
    {
		activeContainer = container;
		options = new List<string>();
		foreach (Item item in inventoryManager.resources)
		{
			options.Add(item.name);
		}
		dropdown.AddOptions(options);
		inputField.text = activeContainer.dropFrequency.ToString();
		lockResource.isOn = activeContainer.lockResource;
		if (activeContainer.itemInfo != null) {	dropdown.value = options.IndexOf(activeContainer.itemInfo.name)+1; }
	}
	
	public void changedDF()
	{
		activeContainer.dropFrequency = float.Parse(inputField.text);
	}
	
	public void changedLR()
	{
		activeContainer.lockResource = lockResource.isOn;
	}
	
	public void changedDD()
	{
		Empty();
		foreach (Item item in inventoryManager.resources)
		{
			if (item.name == options[dropdown.value-1])
			{
				activeContainer.itemInfo = item;
			}
		}
		activeContainer.transform.GetChild(0).GetComponent<SpriteRenderer>().sprite = activeContainer.itemInfo.image;
	}
	
	public void Empty()
	{
		transform.parent.parent.parent.GetComponent<PlayerManager>().AddToInventory(activeContainer.itemInfo, activeContainer.reserve);
		activeContainer.reserve = 0;
	}
}
