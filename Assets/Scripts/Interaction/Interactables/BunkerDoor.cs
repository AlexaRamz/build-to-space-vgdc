using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BunkerDoor : Interactable
{
    [SerializeField] QuestManager questManager;
    [SerializeField] QuestData headToBaseQuest;
    [SerializeField] QuestData headToMoonQuest;

    public static bool questFulfilled;
    bool loading;

    private void Start()
    {
        if (questFulfilled)
        {
            questManager.FulfillQuest(headToBaseQuest, false);
            questManager.AddQuest(headToMoonQuest);
        }
    }
    public override void Interact()
    {
        if (!loading && !questFulfilled && questManager.IsActiveQuest(headToBaseQuest))
        {
            loading = true;
            SceneLoader.Instance.LoadScene("MysteriousMessageCutscene");
            questFulfilled = true;
        }
    }
}
