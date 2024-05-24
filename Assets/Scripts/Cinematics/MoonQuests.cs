using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoonQuests : MonoBehaviour
{
    DialogueManager dialogueManager;
    [SerializeField] NPC commander;
    [SerializeField] Dialogue[] commanderDialogues;
    [SerializeField] QuestManager questManager;
    [SerializeField] QuestData headToBase;

    void Start()
    {
        dialogueManager = DialogueManager.Instance;
    }

    public void CallToBase()
    {
        dialogueManager.StartDialogue(commanderDialogues[0], commander, HeadToBase);
    }

    public void HeadToBase()
    {
        questManager.AddQuest(headToBase);
    }
}
