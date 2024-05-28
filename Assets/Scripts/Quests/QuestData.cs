using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum QuestType //Include various quest type options here
{
    None,
    Discover,
    Hunt,
    Fetch,
    Talk,
}

[CreateAssetMenu(fileName = "New Quest", menuName = "Scriptable Objects/Quest")]
public class QuestData: ScriptableObject
{
    public string targetName;
    
    public int moneyReward;
    public int researchReward;
    public string informationText; //Can be displayed somewhere within the UI
    public string victoryText; //While information Text is displayed for the initial description, victory text is the description when a quest is finished
    public QuestType questType;

    public int requiredToDestroy; //This variable is used to track how many enemies need to be destroyed for Hunt quests
    public List<ItemAmountInfo> requiredItems = new List<ItemAmountInfo>();
}
