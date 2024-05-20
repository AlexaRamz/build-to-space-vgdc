using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using TMPro;

public class GeneratorUI : MonoBehaviour
{
	public Generator activeGenerator;
	public FactoryManager factoryManager;
	public TMP_InputField inputField;
	public Toggle overdirve;
	public Slider slider;
	
    public void OnEnable()
    {
		factoryManager.openGeneratorEvent.AddListener(open);
    }

    public void OnDisable()
    {
		factoryManager.openGeneratorEvent.RemoveListener(open);
    }
	
	void Update()
	{
		if (activeGenerator != null)
		{
			slider.maxValue = activeGenerator.maxReserve;
			slider.value = activeGenerator.reserve;
		}
	}

    public void open(Generator generator)
    {
		activeGenerator = generator;
		inputField.text = activeGenerator.power.ToString();
		overdirve.isOn = activeGenerator.overdirve;
	}
	
	public void changedOD()
	{
		activeGenerator.overdirve = overdirve.isOn;
	}
	
	public void Empty()
	{
		transform.parent.parent.parent.GetComponent<PlayerManager>().AddToInventory(activeGenerator.fuels[0], activeGenerator.reserve);
		activeGenerator.reserve = 0;
	}
}
