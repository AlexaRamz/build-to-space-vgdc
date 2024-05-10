using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum QuestType //Include various quest type options here
{
    Discover,
    Hunt,
    Fetch,
    Talk
}

[CreateAssetMenu(fileName = "New Quest", menuName = "Scriptable Objects/Quest")]
public class QuestData: ScriptableObject //There is currently no need to include a constructor, since I assume all quests will be written in the editor for this
{
    public string targetName;
    [HideInInspector] public Transform TargetLocation; //The idea behind this is that if a quest requires mutliple characters to be visited, then that quest chain will function as multiple individual quest objects
    public int moneyReward;
    public int researchReward;
    public string informationText; //Can be displayed somewhere within the UI
    public string victoryText; //While information Text is displayed for the initial description, victory text is the description when a quest is finished
    public QuestType questType;
    public int requirement; //Stores index of quest required for this to be completed, set to -1 if there are no requirements
    public int nextQuestIndex; //Stores index of next quest for auto activation, -1 for no next quest
    public bool available; //Determines whether requirements have been completed
    //public int index; //Used to reference index from quest board perspective - if necessary (commented out until this needs to be used)
    [HideInInspector] public bool active; //Used as a reference to whether a quest is active, initially set to false for all (May not be needed but Im leaving it in because I already implemented the setting feature)
    [HideInInspector] public bool completed; //Used as a reference to whether a quest is finished, but not yet moved to the other list (this process involves fulfilling requirements)
    public ItemAmountInfo[] requiredItems;

    private void OnEnable()
    {
        active = completed = false;
    }
}
