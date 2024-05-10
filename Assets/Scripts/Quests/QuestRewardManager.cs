using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

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

    public TextMeshProUGUI displayQuestDescription; //Used in UI to show current quest description - these three should appear separately from the questcontainer, likely above it within the same UI element
    public TextMeshProUGUI displayMoneyReward; //Used in UI to show current quest reward
    public TextMeshProUGUI displayResearchReward; //Used in UI to show current research reward
    public TextMeshProUGUI displayTitle; //Used in UI to show currently selected title

    public TextMeshProUGUI displayVictoryQuestDescription; //Used to demonstrate info about the quest that was just completed, after it was removed from the menu
    public TextMeshProUGUI displayVictoryMoneyReward; 
    public TextMeshProUGUI displayVictoryResearchReward;

    public TextMeshProUGUI completionNotifier;
    public Slider completionProgressBar; //Set in editor

    [SerializeField] private InventoryManager plrInventory;

    // Start is called before the first frame update
    void Start()
    {
        UpdateQuest(questDatas[currentQuestIndex]); //This assumes that the quest at index 0 is expected to be the initial quest
        PopulateMenu(); //Runs function to populate quest menu based upon the list of quests
    }

    // Update is called once per frame
    void FixedUpdate() //Run checks for whether a quest was completed
    {
        switch(currentQuest.questType)
        {
            case QuestType.Discover: //Assumption is this will detect whether you have reached a certain area and/or height
                if (currentQuest.TargetLocation != null && playerLocation.localPosition.x >= currentQuest.TargetLocation.localPosition.x-3 && playerLocation.localPosition.x <= currentQuest.TargetLocation.localPosition.x+3) //Range set to 3
                {
                    if (currentQuest.completed == false)
                    {
                        StartCoroutine(AnnounceQuestComplete(currentQuest.victoryText)); //Send any signal here for an instantaneous response to quest success
                        currentQuest.completed = true; //Calculates completion constantly, but registers it when menu is opened
                    }
                }
                break;
            case QuestType.Hunt: //Assumption is this will detect whether you have defeated a certain monster
                if (currentQuest.TargetLocation == null) //Checks if target monster has been destroyed
                {
                    if (currentQuest.completed == false)
                    {
                        StartCoroutine(AnnounceQuestComplete(currentQuest.victoryText)); //Send any signal here for an instantaneous response to quest success
                        currentQuest.completed = true; //Calculates completion constantly, but registers it when menu is opened
                    }
                }
                break;
            case QuestType.Fetch: //Assumption is this will detect whether you have collected a certain object
                                  //Implement IF condition to determine whether quest is complete - this check involves the inventory object
                                  //Call CompleteCurrentQuest() if so
                bool hasAllItems = true;
                int totalItems = 0;
                int itemsCollected = 0;
                foreach (ItemAmountInfo info in currentQuest.requiredItems)
                {
                    int enoughQuantity = plrInventory.HasEnoughInt(info.item);
                    if (!(enoughQuantity >= info.amount))
                    {
                        hasAllItems = false;
                    }
                    totalItems += info.amount; //Increments total items by quantity of this item type
                    itemsCollected += enoughQuantity; //Determines how many items have been collected
                }
                currentQuest.progress = itemsCollected/totalItems; //Determines percentage of items collected
                if (hasAllItems && currentQuest.completed == false)
                {
                    StartCoroutine(AnnounceQuestComplete(currentQuest.victoryText)); //Send any signal here for an instantaneous response to quest success
                    currentQuest.completed = true; //Calculates completion constantly, but registers it when menu is opened
                }
                break;
            case QuestType.Talk: //Assumption is you will converse with an npc and a certain dialogue line completion will trigger something to detect here
                if (currentQuest.TargetLocation != null && dialogueManager.currentSpeaker != null)
                {
                    if (currentQuest.TargetLocation.gameObject.GetComponent<DialogueTrigger>().speaker == dialogueManager.currentSpeaker) //Verifies if speaker matches target
                    {
                        if (currentQuest.completed == false)
                        {
                            StartCoroutine(AnnounceQuestComplete(currentQuest.victoryText)); //Send any signal here for an instantaneous response to quest success
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
        currentQuest.TargetLocation = GameObject.Find(currentQuest.targetName)?.transform;
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
            displayVictoryQuestDescription.SetText(currentQuest.victoryText); //Updates visuals (should happen regardless of inputs)
            displayVictoryMoneyReward.SetText(currentQuest.moneyReward.ToString()); //Updates visuals (should happen regardless of inputs)
            displayVictoryResearchReward.SetText(currentQuest.researchReward.ToString()); //Updates visuals (should happen regardless of inputs)

            //Add a popup to say quest is complete here, if desired (need to determine how to make an additive menu for a task like this)
            UpdateStatus(); //Updates availability statuses, before this quest is removed from menu/QuestDatas list
            currentQuest.active = false; //Ends quest activity when complete
            plrInventory.researchPoints += currentQuest.researchReward;
            plrInventory.money += currentQuest.moneyReward; //Updates quest rewards
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
                template.title.SetText(questDatas[i].name); //Sets proper information as needed to the quest template object for display purposes (this portion will be modified based upon the implementation of the QuestTemplate object - refer to the ShopTemplate as an example for reference)
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
        displayTitle.SetText(currentQuest.name); //Updates visuals (should happen regardless of inputs)
        completionProgressBar.SetValueWithoutNotify(currentQuest.progress);
        //Automatic quest completion check, likely make a button to do this later (if there is not a button, the completion checks would likely have to be done when this menu is open, just before the following function is called)
        bool finishedQuest = CompleteCurrentQuest();
        if (finishedQuest) //Possibly create some sort of animation to play here, or something along those lines to indicate quest completion
        {
            displayQuestDescription.SetText("");
            displayMoneyReward.SetText("");
            displayResearchReward.SetText(""); //Effectively clears UI portion of selection when quest is completed
            displayTitle.SetText("");
            completionProgressBar.SetValueWithoutNotify(0f);
        }
    }

    IEnumerator AnnounceQuestComplete(string message)
    {
        Color endColor = new Color(1f, 1f, 1f, 0f);
        Color startColor = Color.white;
        completionNotifier.color = startColor;
        completionNotifier.SetText(message); //Sets text to victory message, then fades away

        float activeTime = 0f;
        if (activeTime < 2f) //Waits 2 seconds before fading
        {
            activeTime += Time.deltaTime;
            yield return null; //Updates time but does nothing
        }
        while (activeTime<5.5f) //Sets time to fade to be 3.5 seconds
        {
            float currentTimeFactor = activeTime/3.5f;
            completionNotifier.color = Color.Lerp(startColor, endColor, currentTimeFactor); //Fades color to clear over 3.5 seconds
            activeTime += Time.deltaTime;
            yield return null;
        }

        completionNotifier.color = endColor;
        completionNotifier.SetText(""); //Clears notifier once finished fading
    }
}
