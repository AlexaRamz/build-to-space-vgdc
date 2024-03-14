using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialManager : MonoBehaviour
{
    DialogueSystem dialogueSys;
    [SerializeField]
    private NPC commander;

    [SerializeField]
    private Dialogue[] commanderDialogues;
    [SerializeField]
    private Dialogue[] bot1Dialogues;
    [SerializeField]
    private Dialogue[] bot2Dialogues;

    [SerializeField]
    private DialogueTrigger bot1;

    [SerializeField]
    private GameObject menuInfo;
    private bool awaitingCloseInput;
    [SerializeField]
    private GameObject interactInfo;
    private bool awaitingMenuInput;

    void Start()
    {
        dialogueSys = transform.parent.Find("DialogueSystem").GetComponent<DialogueSystem>();
        interactInfo.SetActive(true);
        awaitingCloseInput = true;
    }

    public void HeadVRArea()
    {
        Debug.Log("Head to VR area!");
        bot1.dialogue = bot1Dialogues[0];
        bot1.eventOnEnd = BuildPopUp;
    }

    public void BuildPopUp()
    {
        Debug.Log("How to build pop up");
        menuInfo.SetActive(true);
        awaitingMenuInput = true;
    }

    private void Update()
    {
        if (awaitingMenuInput && Input.GetKeyDown(KeyCode.M))
        {
            menuInfo.SetActive(false);
            bot1.dialogue = bot1Dialogues[1];
            bot1.eventOnEnd = null;
            awaitingMenuInput = false;
        }
        if (awaitingCloseInput && Input.GetKeyDown(KeyCode.RightShift))
        {
            interactInfo.SetActive(false);
            dialogueSys.StartDialogue(commanderDialogues[0], commander, HeadVRArea);
            awaitingCloseInput = false;
        }
    }
}
