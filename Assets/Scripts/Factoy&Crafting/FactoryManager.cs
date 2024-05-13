using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

[CreateAssetMenu(fileName = "MenuManager", menuName = "Scriptable Objects/Managers/Factory Manager")]
public class FactoryManager : ScriptableObject
{
	public MenuManager menuManager;
    public UnityEvent<Container> openContainerEvent;
    public UnityEvent<Generator> openGeneratorEvent;
    public UnityEvent<Furnace> openFurnaceEvent;
    public UnityEvent<Drill> openDrillEvent;
	
    public void ContainerMenu(Container container)
	{
		menuManager.ShowMenu("Container");
		openContainerEvent?.Invoke(container);
	}
	
    public void GeneratorMenu(Generator generator)
	{
		menuManager.ShowMenu("Generator");
		openGeneratorEvent?.Invoke(generator);
	}
	
    public void FurnaceMenu(Furnace furnace)
	{
		menuManager.ShowMenu("Furnace");
		openFurnaceEvent?.Invoke(furnace);
	}
	
    public void DrillMenu(Drill drill)
	{
		menuManager.ShowMenu("Drill");
		openDrillEvent?.Invoke(drill);
	}
}
