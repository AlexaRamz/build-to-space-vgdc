using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "QuestManager", menuName = "Scriptable Objects/Managers/Quest Manager")]
public class QuestManager : ScriptableObject
{
    public List<QuestData> starterQuests = new List<QuestData>();
    public List<Quest> activeQuests = new List<Quest>();
    public List<Quest> completedQuests = new List<Quest>();

    public UnityEvent<QuestData> questAdded;
    public UnityEvent<QuestData> questCompleted;
    public UnityEvent UIupdateList;
    public UnityEvent UIupdateInfo;

    public Quest currentQuestMain; // The current quest to keep track of. This is usually a quest part of the main storyline rather than side quests
    [SerializeField] InventoryManager plrInventory;

    private void OnEnable()
    {
        activeQuests.Clear();
        foreach (var i in starterQuests)
        {
            Quest quest = new Quest(i);
            activeQuests.Add(quest);
        }

        plrInventory.inventoryModifiedEvent += UpdateFetchQuests;

        UpdateFetchQuests();
    }
    private void OnDisable()
    {
        plrInventory.inventoryModifiedEvent -= UpdateFetchQuests;
    }

    public Quest AddQuest(QuestData questData)
    {
        Quest quest = new Quest(questData);
        activeQuests.Add(quest);
        questAdded?.Invoke(questData);
        UIupdateList?.Invoke();

        quest.questState = QuestState.Active;
        return quest;
    }
    public Quest AddQuestMain(QuestData questData)
    {
        currentQuestMain = AddQuest(questData);
        return currentQuestMain;
    }
    public Quest GetActiveQuest(QuestData questData)
    {
        foreach (Quest quest in activeQuests)
        {
            if (quest.questData == questData)
            {
                return quest;
            }
        }
        return null;
    }
    public bool IsActiveQuest(QuestData questData)
    {
        foreach (Quest quest in activeQuests)
        {
            if (quest.questData == questData)
            {
                return true;
            }
        }
        return false;
    }

    public void FulfillQuest(QuestData questData, bool notify = true)
    {
        Quest quest = GetActiveQuest(questData);
        if (quest != null) FulfillQuest(quest, notify);
    }

    public void FulfillQuest(Quest quest, bool notify = true)
    {
        if (notify)
        {
            questCompleted?.Invoke(quest.questData);
        }

        quest.questState = QuestState.Fulfilled;
        if (currentQuestMain == quest) currentQuestMain = null;
    }

    public void CollectQuestReward(Quest quest)
    {
        plrInventory.researchPoints += quest.questData.researchReward;
        plrInventory.money += quest.questData.moneyReward;

        quest.questState = QuestState.Completed;
        activeQuests.Remove(quest);
        completedQuests.Add(quest);
        UIupdateList?.Invoke();
    }

    public void UpdateHuntQuests(string enemyName) // Called externally when an enemy has been killed. Updates "hunt" quests accordingly.
    {
        foreach (Quest quest in activeQuests)
        {
            if (quest.questData.questType == QuestType.Hunt && quest.questData.targetName == enemyName && quest.questState == QuestState.Active)
            {
                quest.destroyedSoFar += 1;
                if (quest.destroyedSoFar > quest.questData.requiredToDestroy) quest.destroyedSoFar = quest.questData.requiredToDestroy;
                quest.progress = (float)quest.destroyedSoFar / quest.questData.requiredToDestroy;
                if (quest.progress == 1f)
                {
                    FulfillQuest(quest);
                }
            }
        }
        UIupdateInfo?.Invoke();
    }

    public void UpdateFetchQuests() // Called when player's items have been updated. Updates "fetch" quests accordingly.
    {
        foreach (Quest quest in activeQuests)
        {
            if (quest.questData.questType == QuestType.Fetch && quest.questState == QuestState.Active)
            {
                quest.destroyedSoFar += 1;
                if (quest.destroyedSoFar > quest.questData.requiredToDestroy) quest.destroyedSoFar = quest.questData.requiredToDestroy;

                // Update the progress for the fetch quest
                bool hasAllItems = plrInventory.HasAll(quest.questData.requiredItems);
                int totalItems = 0;
                int itemsCollected = 0;
                foreach (ItemAmountInfo info in quest.questData.requiredItems)
                {
                    int enoughQuantity = plrInventory.GetItemAmount(info.item);
                    if (enoughQuantity > info.amount)
                    {
                        enoughQuantity = info.amount;
                    }
                    totalItems += info.amount; // Increments total items by quantity of this item type
                    itemsCollected += enoughQuantity; // Determines how many items have been collected
                }
                quest.progress = (float)itemsCollected / totalItems; // Determines percentage of items collected

                // Check if the quest has been completed
                if (hasAllItems)
                {
                    FulfillQuest(quest);
                }
            }
        }
        UIupdateInfo?.Invoke();
    }
}
