using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class DialogueTrigger : Interactable
{
    public NPC speaker;
    DialogueManager dialogueManager;
    public Dialogue dialogue;
    public UnityEvent eventOnEnd;
    public float eventDelay;

    void Start()
    {
        dialogueManager = DialogueManager.Instance;
    }
    public override void Interact()
    {

        if (dialogue == null)
        {
            Dialogue randomDialogue = speaker.dialogues[UnityEngine.Random.Range(0, speaker.dialogues.Count)];
            dialogueManager.StartDialogue(randomDialogue, speaker, eventOnEnd, eventDelay);
        }
        else
        {
            dialogueManager.StartDialogue(dialogue, speaker, eventOnEnd);
        }
    }
}
