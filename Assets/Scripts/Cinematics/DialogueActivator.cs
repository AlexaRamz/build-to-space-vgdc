using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class DialogueActivator : MonoBehaviour
{
    public Dialogue dialogue;
    public NPC speaker;
    public UnityEvent eventOnEnd;
    public float eventDelay;

    void Start()
    {
        DialogueManager.Instance.StartDialogue(dialogue, speaker, eventOnEnd);
    }
}
