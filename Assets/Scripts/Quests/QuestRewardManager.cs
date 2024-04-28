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

[System.Serializable]
public class QuestData //There is currently no need to include a constructor, since I assume all quests will be written in the editor for this
{
    public Transform TargetLocation; //The idea behind this is that if a quest requires mutliple characters to be visited, then that quest chain will function as multiple individual quest objects
    public float moneyReward;
    public float researchReward;
    public string informationText; //Can be displayed somewhere within the UI
    public QuestType questType;
    public QuestData nextQuest; //Opportunity to set a next quest to another object
}

public class QuestRewardManager : MonoBehaviour
{
    public List<QuestData> questDatas = new List<QuestData>(); //Public list to write quest info into
    List<QuestData> completedQuestDatas = new List<QuestData>(); //Transition quest datas into here, upon quest completion
    QuestData currentQuest;
    int currentQuestIndex = 0;

    public QuestFinder questFinder; //Set this association through the editor

    public float researchPointsObtained; //Records research points obtained through quest completion
    public float moneyObtained; //Records money obtained through quest completion

    // Start is called before the first frame update
    void Start()
    {
        researchPointsObtained = 0f;
        moneyObtained = 0f;
        UpdateQuest(questDatas[currentQuestIndex]); //This assumes that the quest at index 0 is expected to be the initial quest
    }

    // Update is called once per frame
    void Update() //Run checks for whether a quest was completed
    {
        switch(currentQuest.questType)
        {
            case QuestType.Discover: //Assumption is this will detect whether you have reached a certain area and/or height
                //Implement IF condition to determine whether quest is complete
                    //Call CompleteCurrentQuest() if so
                break;
            case QuestType.Hunt: //Assumption is this will detect whether you have defeated a certain monster
                //Implement IF condition to determine whether quest is complete
                    //Call CompleteCurrentQuest() if so
                break;
            case QuestType.Fetch: //Assumption is this will detect whether you have collected a certain object
                //Implement IF condition to determine whether quest is complete
                    //Call CompleteCurrentQuest() if so
                break;
            case QuestType.Talk: //Assumption is you will converse with an npc and a certain dialogue line completion will trigger something to detect here
                //Implement IF condition to determine whether quest is complete
                    //Call CompleteCurrentQuest() if so
                break;
            default:
                Debug.Log("ERROR: No quest type");
                break;
        }
    }

    void UpdateQuest(QuestData newQuest)
    {
        currentQuest = newQuest;
        if (questFinder != null)
        {
            questFinder.UpdateTarget(currentQuest.TargetLocation); //Updates quest marker based upon current quest objective
        }
        currentQuestIndex = questDatas.IndexOf(currentQuest); //Updates current index being used
    }

    void CompleteCurrentQuest()
    {
        researchPointsObtained += currentQuest.researchReward;
        moneyObtained += currentQuest.moneyReward; //Updates quest rewards
        completedQuestDatas.Add(currentQuest);
        questDatas.Remove(currentQuest);
        if (currentQuest.nextQuest != null)
        {
            UpdateQuest(currentQuest.nextQuest);
        }
        else
        {
            currentQuest = null;
            currentQuestIndex = -1; //Uses -1 identifier when there is no current quest
        }
    }

    //Function to be called elsewhere to select a new quest
    public void SetNewQuest(int newQuestIndex) //Functionality will be the same whether or not the current quest is null - and at the moment it will just be dropped
    {
        //Can change functionality to not work around using a quest index, if this becomes a difficult way to set quest status during level implementation
        if (newQuestIndex != currentQuestIndex) //Does nothing if new quest matches current quest
        {
            UpdateQuest(questDatas[newQuestIndex]);
        }
    }
}
