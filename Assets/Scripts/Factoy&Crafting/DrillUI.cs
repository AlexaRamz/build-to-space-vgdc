using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using TMPro;

public class DrillUI : MonoBehaviour
{
	public Drill activeDrill;
	public FactoryManager factoryManager;
	public TMP_InputField inputField;
	public TMP_InputField inputField1;
	public Slider slider;
	private List<string> options;
	
    public void OnEnable()
    {
		factoryManager.openDrillEvent.AddListener(open);
    }

    public void OnDisable()
    {
		factoryManager.openDrillEvent.RemoveListener(open);
    }
	
	void Update()
	{
		if (activeDrill != null)
		{
			slider.maxValue = activeDrill.dropFrequency;
			slider.value = activeDrill.timeTo;
		}
	}

    public void open(Drill drill)
    {
		activeDrill = drill;
		inputField.text = (activeDrill.oreRichness*activeDrill.GetComponent<Power>().power).ToString();
		inputField1.text = activeDrill.resouceInfo.name;
	}
}
