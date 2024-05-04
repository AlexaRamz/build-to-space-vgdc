using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

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
    public string questTitle; //Stores title information
    public string informationText; //Can be displayed somewhere within the UI
    public QuestType questType;
    public int requirement; //Stores index of quest required for this to be completed, set to -1 if there are no requirements
    public int nextQuestIndex; //Stores index of next quest for auto activation, -1 for no next quest
    public bool available; //Determines whether requirements have been completed
    //public int index; //Used to reference index from quest board perspective - if necessary (commented out until this needs to be used)
    public bool active = false; //Used as a reference to whether a quest is active, initially set to false for all (May not be needed but Im leaving it in because I already implemented the setting feature)
    public bool completed = false; //Used as a reference to whether a quest is finished, but not yet moved to the other list (this process involves fulfilling requirements)
}

public class QuestRewardManager : MonoBehaviour
{
    [SerializeField] public List<QuestData> questDatas = new List<QuestData>(); //Public list to write quest info into
    List<QuestData> completedQuestDatas = new List<QuestData>(); //Transition quest datas into here, upon quest completion
    QuestData currentQuest;
    int currentQuestIndex = 0;

    public Transform playerLocation; //Set reference in editor
    public DialogueManager dialogueManager; //Set reference in editor
    public QuestFinder questFinder; //Set this association through the editor
    public GameObject questTemplate; //Set this association through the editor - must be the UI object for how quests will appear (reference the ShopManager for an example)
    public Transform questContainer; //Set this association through the editor - must be the UI object for where quests are stored

    public float researchPointsObtained; //Records research points obtained through quest completion
    public float moneyObtained; //Records money obtained through quest completion

    public TextMeshProUGUI displayQuestDescription; //Used in UI to show current quest description - these three should appear separately from the questcontainer, likely above it within the same UI element
    public TextMeshProUGUI displayMoneyReward; //Used in UI to show current quest reward
    public TextMeshProUGUI displayResearchReward; //Used in UI to show current research reward

    public TextMeshProUGUI displayVictoryQuestDescription; //Used to demonstrate info about the quest that was just completed, after it was removed from the menu
    public TextMeshProUGUI displayVictoryMoneyReward; 
    public TextMeshProUGUI displayVictoryResearchReward; 

    // Start is called before the first frame update
    void Start()
    {
        researchPointsObtained = 0f;
        moneyObtained = 0f;
        UpdateQuest(questDatas[currentQuestIndex]); //This assumes that the quest at index 0 is expected to be the initial quest
        PopulateMenu(); //Runs function to populate quest menu based upon the list of quests
    }

    // Update is called once per frame
    void FixedUpdate() //Run checks for whether a quest was completed
    {
        switch(currentQuest.questType)
        {
            case QuestType.Discover: //Assumption is this will detect whether you have reached a certain area and/or height
                if (playerLocation.localPosition.x >= currentQuest.TargetLocation.localPosition.x-3 && playerLocation.localPosition.x <= currentQuest.TargetLocation.localPosition.x+3) //Range set to 3
                {
                    if (currentQuest.completed == false)
                    {
                        //Send any signal here for an instantaneous response to quest success
                        currentQuest.completed = true; //Calculates completion constantly, but registers it when menu is opened
                    }
                }
                break;
            case QuestType.Hunt: //Assumption is this will detect whether you have defeated a certain monster
                if (currentQuest.TargetLocation == null) //Checks if target monster has been destroyed
                {
                    if (currentQuest.completed == false)
                    {
                        //Send any signal here for an instantaneous response to quest success
                        currentQuest.completed = true; //Calculates completion constantly, but registers it when menu is opened
                    }
                }
                break;
            case QuestType.Fetch: //Assumption is this will detect whether you have collected a certain object
                //Implement IF condition to determine whether quest is complete - this check involves the inventory object
                    //Call CompleteCurrentQuest() if so
                break;
            case QuestType.Talk: //Assumption is you will converse with an npc and a certain dialogue line completion will trigger something to detect here
                if (dialogueManager.currentSpeaker != null)
                {
                    if (currentQuest.TargetLocation.gameObject.GetComponent<DialogueTrigger>().speaker == dialogueManager.currentSpeaker) //Verifies if speaker matches target
                    {
                        if (currentQuest.completed == false)
                        {
                            //Send any signal here for an instantaneous response to quest success
                            currentQuest.completed = true; //Calculates completion constantly, but registers it when menu is opened
                        }
                    }
                }
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

    bool CompleteCurrentQuest() //Returns whether quest was successfully completed - the return should have a UI indication for whether the completion button 'can be pressed'
    {
        if (currentQuest.completed == true)
        {
            //Displays completed quest info
            displayVictoryQuestDescription.SetText(currentQuest.informationText); //Updates visuals (should happen regardless of inputs)
            displayVictoryMoneyReward.SetText(currentQuest.moneyReward.ToString()); //Updates visuals (should happen regardless of inputs)
            displayVictoryResearchReward.SetText(currentQuest.researchReward.ToString()); //Updates visuals (should happen regardless of inputs)

            //Add a popup to say quest is complete here, if desired (need to determine how to make an additive menu for a task like this)
            UpdateStatus(); //Updates availability statuses, before this quest is removed from menu/QuestDatas list
            currentQuest.active = false; //Ends quest activity when complete
            researchPointsObtained += currentQuest.researchReward;
            moneyObtained += currentQuest.moneyReward; //Updates quest rewards
            completedQuestDatas.Add(currentQuest);
            //questDatas.Remove(currentQuest); //No more quest removal so that requirements can be properly indexed
            PopulateMenu(); //Updates menu when a quest is completed, removing it from the list
            if (currentQuest.nextQuestIndex >= 0)
            {
                UpdateQuest(questDatas[currentQuest.nextQuestIndex]);
            }
            else
            {
                currentQuest = null;
                currentQuestIndex = -1; //Uses -1 identifier when there is no current quest
            }
            return true;
        }
        return false;
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
            if (questDatas[i].available == true && questDatas[i].completed == false) //Makes sure quest is both available and not completed
            {
                int btnNo = i;
                GameObject button = Instantiate(questTemplate, questContainer);
                button.GetComponent<Button>().onClick.AddListener(delegate { SelectQuest(btnNo); }); //Runs quest select function when a quest is selected
                QuestTemplate template = button.GetComponent<QuestTemplate>();
                template.title.SetText(questDatas[i].questTitle); //Sets proper information as needed to the quest template object for display purposes (this portion will be modified based upon the implementation of the QuestTemplate object - refer to the ShopTemplate as an example for reference)
            }
        }
    }

    void UpdateStatus() //Updates quest availability statuses
    {
        for (int i = 0; i < questDatas.Count; i++)
        {
            if (questDatas[i].available != true)
            {
                int requirements = questDatas[i].requirement;
                questDatas[i].available = questDatas[requirements].completed; //Sets availability
            }
        }
    }

    void ClearMenu()
    {
        foreach (Transform child in questContainer)
        {
            Destroy(child.gameObject); //Clears quest list
        }
    }

    //This function is called when a quest's button is clicked on
    public void SelectQuest(int index) //Opens menu for individual quest - this will have no buttons within it, but doing this will set as focused currentquest, then check completion, then try to complete (check completion could be moved out of update, using this logic, depending on flag implementations)
    {
        UpdateQuest(questDatas[index]); //Updates active quest
        //Automatic quest selection, possibly make an internal button to do this later
        displayQuestDescription.SetText(currentQuest.informationText); //Updates visuals (should happen regardless of inputs)
        displayMoneyReward.SetText(currentQuest.moneyReward.ToString()); //Updates visuals (should happen regardless of inputs)
        displayResearchReward.SetText(currentQuest.researchReward.ToString()); //Updates visuals (should happen regardless of inputs)
        //Automatic quest completion check, likely make a button to do this later (if there is not a button, the completion checks would likely have to be done when this menu is open, just before the following function is called)
        bool finishedQuest = CompleteCurrentQuest();
        if (finishedQuest) //Possibly create some sort of animation to play here, or something along those lines to indicate quest completion
        {
            displayQuestDescription.SetText("");
            displayMoneyReward.SetText("");
            displayResearchReward.SetText(""); //Effectively clears UI portion of selection when quest is completed
        }
    }
}
