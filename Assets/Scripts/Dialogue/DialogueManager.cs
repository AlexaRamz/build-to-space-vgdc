using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogueManager : MonoBehaviour
{
    Dialogue currentDialogue;
    NPC currentSpeaker;
    Queue<TextIconSet> sentences;
    string currentSentence;

    [SerializeField] private DialogueUI dialogueUI;
    IEnumerator currentCoroutine;

    private bool talking = false;
    bool typing;
    // bool responding;
    public float typeSpeed = 0.05f;
    public float pauseTime = 0.5f;
    //public int maxLetters = 30;

    [SerializeField] private MenuManager menuManager;
    private Action _actionOnEnd;

    public static DialogueManager Instance;

    void Awake()
    {
        Instance = this;
    }
    private void Start()
    {
        menuManager.menuClosedEvent.AddListener(OnDialogueClosed);
        sentences = new Queue<TextIconSet>();
    }
    public void StartDialogue(Dialogue dialogue, NPC speaker, Action actionOnEnd=null)
    {
        _actionOnEnd = actionOnEnd;
        currentDialogue = dialogue;
        currentSpeaker = speaker;

        sentences.Clear();
        ResetResponses();

        if (!talking)
            menuManager.ShowMenu("DialogueBox");
        dialogueUI.nameDisplay.text = currentSpeaker.name;

        StartCoroutine(StartDelay());

        foreach (TextIconSet sentence in dialogue.sentences)
        {
            sentences.Enqueue(sentence);
        }
        DisplayNextSentence();
    }
    void OnDialogueClosed(string closedMenu)
    {
        if (closedMenu == "DialogueBox")
        {
            EndDialogue();
        }
    }
    void EndDialogue()
    {
        if (currentCoroutine != null) StopCoroutine(currentCoroutine);
        currentDialogue = null;
        StartCoroutine(EndDelay());
    }
    public void CloseMenu()
    {
        menuManager.CloseCurrentMenu();
        EndDialogue();
    }
    IEnumerator StartDelay()
    {
        yield return new WaitForSeconds(0.1f);
        talking = true;
    }
    IEnumerator EndDelay()
    {
        yield return new WaitForSeconds(0.1f);
        talking = false;
        if (_actionOnEnd != null) _actionOnEnd();
    }
    List<char> pauseChars = new List<char>{ '.', ',', '!', '?' };
    IEnumerator TypeText(string sentence)
    {
        typing = true;
        int charIndex = 0;

        for (int i = 0; i < sentence.Length; i++)
        {
            dialogueUI.textDisplay.text = sentence.Substring(0, charIndex + 1);

            if (pauseChars.Contains(sentence[charIndex]) && charIndex < sentence.Length - 1 && sentence[charIndex + 1] == ' ')
            {
                yield return new WaitForSeconds(pauseTime);
            }
            else if (sentence[charIndex] != ' ')
            {
                yield return new WaitForSeconds(1 / typeSpeed);
            }
            charIndex++;
        }
        
        typing = false;
        Responses();
    }

    // Responses
    void Responses()
    {
        if (sentences.Count == 0 && currentDialogue && currentDialogue.responses.Count != 0)
        {
            dialogueUI.responseUI.enabled = true;
            for (int i = 0; i < currentDialogue.responses.Count; i++)
            {
                int index = i;
                GameObject obj = Instantiate(dialogueUI.responseTemplate, dialogueUI.responseContainer.transform);
                obj.transform.Find("Text").GetComponent<TMPro.TextMeshProUGUI>().text = currentDialogue.responses[i].description;
                obj.GetComponent<Button>().onClick.AddListener(delegate { Respond(index); });
            }
            // responding = true;
        }
        //LeanTween.moveLocal(responseContainer, new Vector3(-573, -180, 0), 0.2f).setEase(LeanTweenType.easeInCubic);
    }
    public void Respond(int i) // Called by response button
    {
        ResetResponses();
        StartDialogue(currentDialogue.responses[i].nextDialogue, currentSpeaker);
    }
    void ResetResponses()
    {
        dialogueUI.responseUI.enabled = false;
        foreach (Transform child in dialogueUI.responseContainer.transform)
        {
            Destroy(child.gameObject);
        }
    }

    public void DisplayNextSentence()
    {
        if (sentences.Count == 0)
        {
            CloseMenu();
            return;
        }
        if (currentCoroutine != null)
        {
            StopCoroutine(currentCoroutine);
        }
        dialogueUI.textDisplay.text = "";
        TextIconSet sentence = sentences.Dequeue();
        currentSentence = sentence.text;
        currentCoroutine = TypeText(currentSentence);
        StartCoroutine(currentCoroutine);
        Sprite icon = currentSpeaker.GetPortrait(sentence.emotion);
        if (icon != null)
        {
            dialogueUI.iconDisplay.sprite = icon;
        }
    }
    void Update()
    {
        if (talking && Input.GetKeyDown(KeyCode.Return))
        {
            if (typing)
            {
                if (currentCoroutine != null)
                {
                    StopCoroutine(currentCoroutine);
                }
                typing = false;
                dialogueUI.textDisplay.text = currentSentence;
                Responses();
            }
            else
            {
                DisplayNextSentence();
            }
        }
    }
}
