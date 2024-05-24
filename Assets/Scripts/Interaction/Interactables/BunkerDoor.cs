using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BunkerDoor : Interactable
{
    [SerializeField] QuestData headToBaseQuest;

    public override void Interact()
    {
        if (headToBaseQuest.active)
        {
            SceneLoader.Instance.LoadScene("MysteriousMessageCutscene");
        }
    }
}
