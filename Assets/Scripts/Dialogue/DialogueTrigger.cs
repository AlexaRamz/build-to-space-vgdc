using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueTrigger : Interactable
{
    public NPC speaker;
    DialogueManager dialogueManager;
    public Dialogue dialogue;
    public Action eventOnEnd;

    void Start()
    {
        dialogueManager = DialogueManager.Instance;
    }
    public override void Interact()
    {
        if (dialogue == null)
        {
            Dialogue randomDialogue = speaker.dialogues[UnityEngine.Random.Range(0, speaker.dialogues.Count)];
            dialogueManager.StartDialogue(randomDialogue, speaker, eventOnEnd);
        }
        else
        {
            dialogueManager.StartDialogue(dialogue, speaker, eventOnEnd);
        }
    }
}
