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

    [SerializeField] private MenuManager menuManager;
    bool awaitingMenuInput;

    void Start()
    {
        dialogueManager = DialogueManager.Instance;
        menuManager.ShowMenu("SkipPrompt");
        bot2.dialogue = bot2Dialogues[0];

        bot2.eventOnEnd.RemoveAllListeners();
        bot2.eventOnEnd.AddListener(BuildPopUp2);
    }
    public void InteractInfo()
    {
        menuManager.ShowMenu("InteractInfo");
    }
    public void CommanderDialogue1()
    {
        dialogueManager.StartDialogue(commanderDialogues[0], commander, HeadVRArea);
    }
    public void HeadVRArea()
    {
        bot1.dialogue = bot1Dialogues[0];

        bot1.eventOnEnd.RemoveAllListeners();
        bot1.eventOnEnd.AddListener(BuildPopUp);
    }

    public void BuildPopUp()
    {
        menuManager.ShowMenu("MenuInfo");
        awaitingMenuInput = true;
    }
    public void GoRightPopUp()
    {
        menuManager.ShowMenu("TravelInfo");
    }
    public void BuildPopUp2()
    {
        bot2.dialogue = bot2Dialogues[1];

        bot2.eventOnEnd.RemoveAllListeners();
        bot2.eventOnEnd.AddListener(FlyPopUp);
        menuManager.ShowMenu("BuildInfo");
    }
    public void FlyPopUp()
    {
        bot2.dialogue = bot2Dialogues[2];

        bot2.eventOnEnd.RemoveAllListeners();
        bot2.eventOnEnd.AddListener(ExitPopUp);
        menuManager.ShowMenu("FlyInfo");
    }
    public void ExitPopUp()
    {
        menuManager.ShowMenu("ExitPrompt");
    }
    public void ExitTutorial()
    {
        SceneLoader.Instance.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
    private void Update()
    {
        if (awaitingMenuInput && Input.GetKeyDown(KeyCode.M))
        {
            bot1.dialogue = bot1Dialogues[1];

            bot2.eventOnEnd.RemoveAllListeners();
            bot1.eventOnEnd.AddListener(GoRightPopUp);
            awaitingMenuInput = false;
        }
    }
}
