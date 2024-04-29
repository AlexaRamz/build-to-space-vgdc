using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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
    //public int index; //Used to reference index from quest board perspective - if necessary (commented out until this needs to be used)
    public bool active = false; //Used as a reference to whether a quest is active, initially set to false for all (May not be needed but Im leaving it in because I already implemented the setting feature)
}

public class QuestRewardManager : MonoBehaviour
{
    public List<QuestData> questDatas = new List<QuestData>(); //Public list to write quest info into
    List<QuestData> completedQuestDatas = new List<QuestData>(); //Transition quest datas into here, upon quest completion
    QuestData currentQuest;
    int currentQuestIndex = 0;

    public QuestFinder questFinder; //Set this association through the editor
    public GameObject questTemplate; //Set this association through the editor - must be the UI object for how quests will appear (reference the ShopManager for an example)
    public Transform questContainer; //Set this association through the editor - must be the UI object for where quests are stored

    public float researchPointsObtained; //Records research points obtained through quest completion
    public float moneyObtained; //Records money obtained through quest completion

    // Start is called before the first frame update
    void Start()
    {
        researchPointsObtained = 0f;
        moneyObtained = 0f;
        UpdateQuest(questDatas[currentQuestIndex]); //This assumes that the quest at index 0 is expected to be the initial quest
        PopulateMenu(); //Runs function to populate quest menu based upon the list of quests
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
                //Implement IF condition to determine whether quest is complete - this check involves the inventory object
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
        currentQuest.active = true; //Sets quest activity when updated
        if (questFinder != null)
        {
            questFinder.UpdateTarget(currentQuest.TargetLocation); //Updates quest marker based upon current quest objective
        }
        currentQuestIndex = questDatas.IndexOf(currentQuest); //Updates current index being used
    }

    void CompleteCurrentQuest()
    {
        currentQuest.active = false; //Ends quest activity when complete
        researchPointsObtained += currentQuest.researchReward;
        moneyObtained += currentQuest.moneyReward; //Updates quest rewards
        completedQuestDatas.Add(currentQuest);
        questDatas.Remove(currentQuest);
        PopulateMenu(); //Updates menu when a quest is completed, removing it from the list
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

    public float WithdrawMoney() //Used for transferring money to a different object (done this way to prevent replication)
    {
        if (moneyObtained > 0)
        {
            float tempMoney = moneyObtained;
            moneyObtained = 0;
            return tempMoney;
        }
        return 0; //Otherwise value for when there is no balance
    }

    public float WithdrawResearch() //Used for transferring research to a different object (done this way to prevent replication)
    {
        if (researchPointsObtained > 0)
        {
            float tempResearch = researchPointsObtained;
            researchPointsObtained = 0;
            return tempResearch;
        }
        return 0; //Otherwise value for when there is no balance
    }

    void PopulateMenu() //Need to work on implementing this alongside the UI elements
    {
        ClearMenu(); //Function to clear menu in preparation for populating it
        for (int i = 0; i < questDatas.Count; i++)
        {
            GameObject button = Instantiate(questTemplate, questContainer);
            button.GetComponent<Button>().onClick.AddListener(delegate { SelectQuest(i); }); //Runs quest select function when a quest is selected
            QuestTemplate template = button.GetComponent<QuestTemplate>();
            template.text.SetText(questDatas[i].informationText); //Sets proper information as needed to the quest template object for display purposes (this portion will be modified based upon the implementation of the QuestTemplate object - refer to the ShopTemplate as an example for reference)
        }
    }

    void ClearMenu()
    {
        foreach (Transform child in questContainer)
        {
            Destroy(child.gameObject); //Clears quest list
        }
    }

    public void SelectQuest(int index) //Function 
    {

    }
    //Shop Version of Select Quest for reference:
    /*public void SelectItem(int btnNo)
    {
        selectedItem = shopItems[btnNo];
        displayImage.sprite = selectedItem.image;
        nameText.text = selectedItem.name;
        descriptionText.text = selectedItem.description;
        priceText.text = selectedItem.cost.ToString();
    }*/
}
