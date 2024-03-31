using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TutorialManager : MonoBehaviour
{
    DialogueManager dialogueManager;
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
    private DialogueTrigger bot2;

    [SerializeField]
    private GameObject skipInfo;
    [SerializeField]
    private GameObject menuInfo;
    private bool awaitingMenuInput;
    [SerializeField]
    private GameObject interactInfo;
    [SerializeField]
    private GameObject travelInfo;
    [SerializeField]
    private GameObject flyInfo;
    [SerializeField]
    private GameObject exitInfo;
    [SerializeField]
    private GameObject buildInfo2;

    void Start()
    {
        dialogueManager = DialogueManager.Instance;
        skipInfo.SetActive(true);
        bot2.dialogue = bot2Dialogues[0];
        bot2.eventOnEnd = BuildPopUp2;
    }
    public void InteractInfo()
    {
        interactInfo.SetActive(true);
    }
    public void CommanderDialogue1()
    {
        interactInfo.SetActive(false);
        dialogueManager.StartDialogue(commanderDialogues[0], commander, HeadVRArea);
    }
    public void HeadVRArea()
    {
        bot1.dialogue = bot1Dialogues[0];
        bot1.eventOnEnd = BuildPopUp;
    }

    public void BuildPopUp()
    {
        menuInfo.SetActive(true);
        awaitingMenuInput = true;
    }
    public void GoRightPopUp()
    {
        travelInfo.SetActive(true);
    }
    public void BuildPopUp2()
    {
        bot2.dialogue = bot2Dialogues[1];
        bot2.eventOnEnd = FlyPopUp;
        buildInfo2.SetActive(true);
    }
    public void FlyPopUp()
    {
        bot2.dialogue = bot2Dialogues[2];
        bot2.eventOnEnd = ExitPopUp;
        flyInfo.SetActive(true);
    }
    public void ExitPopUp()
    {
        exitInfo.SetActive(true);
    }
    public void ExitTutorial()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
    private void Update()
    {
        if (awaitingMenuInput && Input.GetKeyDown(KeyCode.M))
        {
            menuInfo.SetActive(false);
            bot1.dialogue = bot1Dialogues[1];
            bot1.eventOnEnd = GoRightPopUp;
            awaitingMenuInput = false;
        }
    }
}
