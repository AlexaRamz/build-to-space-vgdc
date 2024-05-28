using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum QuestState
{
    Active,
    Fulfilled, // All requirements fulfilled and reward available to collect
    Completed, // Reward collected and moved out to completed quest list
}

public class Quest
{
    public QuestData questData;
    public Transform targetLocation; //The idea behind this is that if a quest requires mutliple characters to be visited, then that quest chain will function as multiple individual quest objects
    public GameObject[] targets;

    public QuestState questState;
    public float progress = 0; //Used to determine progress bar percentage
    public int destroyedSoFar = 0; //This variable is used to track how many enemies have been destroyed so far

    public Quest(QuestData questData)
    {
        this.questData = questData;
    }
}
