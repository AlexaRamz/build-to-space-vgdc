using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueTrigger : Interactable
{
    public NPC speaker;
    DialogueSystem dialogueSys;

    void Start()
    {
        dialogueSys = GameObject.Find("DialogueSystem").GetComponent<DialogueSystem>();
    }
    public override void Interact()
    {
        if (dialogueSys.talking == false)
        {
            Dialogue dialogue = speaker.dialogues[Random.Range(0, speaker.dialogues.Count)];
            dialogueSys.StartDialogue(dialogue, speaker);
        }
    }
}
