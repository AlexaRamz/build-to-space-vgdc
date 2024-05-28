using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestRewardManager : MonoBehaviour
{
    [SerializeField] QuestManager questManager;

    public Transform playerLocation;
    public QuestFinder questFinder;

    [SerializeField] InventoryManager plrInventory;
    DialogueManager dialogueManager;

    void LateUpdate()
    {
        foreach (Quest quest in questManager.activeQuests)
        {
            CheckQuestCompleted(quest);
        }
        UpdateQuestPointer();
        dialogueManager = DialogueManager.Instance;
    }

    void CheckQuestCompleted(Quest quest)
    {
        if (quest.questState != QuestState.Active) return;
        switch (quest.questData.questType)
        {
            case QuestType.Discover: // Detect whether you have reached a certain area and/or height
                if (quest.targetLocation != null && playerLocation.localPosition.x >= quest.targetLocation.localPosition.x - 3 && playerLocation.localPosition.x <= quest.targetLocation.localPosition.x + 3) //Range set to 3
                {
                    questManager.FulfillQuest(quest);
                }
                break;
            case QuestType.Talk: // Converse with an npc and a certain dialogue line completion will trigger something to detect here
                if (quest.targetLocation != null && dialogueManager.currentSpeaker != null)
                {
                    if (quest.targetLocation.gameObject.GetComponent<DialogueTrigger>().speaker == dialogueManager.currentSpeaker)
                    {
                        questManager.FulfillQuest(quest);
                    }
                }
                break;
        }
    }

    void UpdateQuestPointer()
    {
        if (questManager.currentQuestMain != null)
        {
            questFinder.UpdateTarget(questManager.currentQuestMain.targetLocation);
        }
    }
}
