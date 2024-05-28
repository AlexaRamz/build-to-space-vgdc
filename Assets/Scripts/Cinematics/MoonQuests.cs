using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class MoonQuests : MonoBehaviour
{
    DialogueManager dialogueManager;
    [SerializeField] NPC commander;
    [SerializeField] Dialogue[] commanderDialogues;
    [SerializeField] QuestManager questManager;
    [SerializeField] QuestData headToBase;
    [SerializeField] Transform bunkerDoor;

    void Start()
    {
        dialogueManager = DialogueManager.Instance;
    }

    public void CallToBase()
    {
        UnityEvent myEvent = new UnityEvent();
        myEvent.AddListener(HeadToBase);
        dialogueManager.StartDialogue(commanderDialogues[0], commander, myEvent);
    }

    public void HeadToBase()
    {
        Quest quest = questManager.AddQuestMain(headToBase);
        quest.targetLocation = bunkerDoor;
    }
}
