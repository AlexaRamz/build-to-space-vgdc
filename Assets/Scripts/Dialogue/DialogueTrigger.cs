using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueTrigger : Interactable
{
    public NPC speaker;
    DialogueSystem dialogueSys;
    public Dialogue dialogue;
    public Action eventOnEnd;

    void Start()
    {
        dialogueSys = GameObject.Find("DialogueSystem").GetComponent<DialogueSystem>();
    }
    public override void Interact()
    {
        if (!dialogueSys.talking)
        {
            if (dialogue == null)
            {
                Dialogue randomDialogue = speaker.dialogues[UnityEngine.Random.Range(0, speaker.dialogues.Count)];
                dialogueSys.StartDialogue(randomDialogue, speaker, eventOnEnd);
            }
            else
            {
                dialogueSys.StartDialogue(dialogue, speaker, eventOnEnd);
            }
        }
    }
}
