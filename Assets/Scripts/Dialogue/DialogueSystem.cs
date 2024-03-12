using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogueSystem : MonoBehaviour, IMenu
{
    Dialogue currentDialogue;
    NPC currentSpeaker;
    Queue<TextIconSet> sentences;
    string currentSentence;

    // UI
    public Canvas mainUI, responseUI;
    public GameObject textBox, responseContainer;
    public TMPro.TextMeshProUGUI textDisplay, nameDisplay;
    public Image iconDisplay;
    public GameObject responseTemplate;
    IEnumerator currentCoroutine;

    [HideInInspector] public bool talking = false;
    bool typing;
    // bool responding;
    public float typeSpeed = 0.05f;
    public float pauseTime = 0.5f;
    //public int maxLetters = 30;

    MenuManager menuManager;
    private Action _actionOnEnd;

    void Awake()
    {
        menuManager = transform.parent.Find("MenuManager").GetComponent<MenuManager>();
        sentences = new Queue<TextIconSet>();
        textBox.transform.localScale = new Vector3(0, 0, 0);
    }
    void Start()
    {

    }
    public void StartDialogue(Dialogue dialogue, NPC speaker, Action actionOnEnd=null)
    {
        _actionOnEnd = actionOnEnd;
        currentDialogue = dialogue;
        currentSpeaker = speaker;
        StartCoroutine(StartDelay());
        menuManager.OpenMenu(this);

        foreach (TextIconSet sentence in dialogue.sentences)
        {
            sentences.Enqueue(sentence);
        }
        DisplayNextSentence();
    }
    void EndDialogue()
    {
        menuManager.CloseMenu();
    }
    public void OpenMenu()
    {
        nameDisplay.text = currentSpeaker.name;
        mainUI.enabled = true;
        LeanTween.scale(textBox, new Vector3(1f, 1f, 1f), 0.2f).setEase(LeanTweenType.easeInCubic);
    }
    public void CloseMenu()
    {
        if (currentCoroutine != null) StopCoroutine(currentCoroutine);
        currentDialogue = null;

        mainUI.enabled = false;
        sentences.Clear();
        ResetResponses();
        textDisplay.text = "";
        iconDisplay.sprite = null;

        textBox.transform.localScale = new Vector3(0, 0, 0);
        //LeanTween.scale(textBox, new Vector3(0, 0, 0), 0.2f).setEase(LeanTweenType.easeInCubic);
        //responseContainer.transform.position = new Vector3(387, -250, 0);
        StartCoroutine(EndDelay());
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
            textDisplay.text = sentence.Substring(0, charIndex + 1);

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
        if (sentences.Count == 0 && currentDialogue.responses.Count != 0)
        {
            responseUI.enabled = true;
            for (int i = 0; i < currentDialogue.responses.Count; i++)
            {
                int index = i;
                GameObject obj = Instantiate(responseTemplate, responseContainer.transform);
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
        responseUI.enabled = false;
        // responding = false;
        foreach (Transform child in responseContainer.transform)
        {
            Destroy(child.gameObject);
        }
    }

    public void DisplayNextSentence()
    {
        if (sentences.Count == 0)
        {
            EndDialogue();
            return;
        }
        if (currentCoroutine != null)
        {
            StopCoroutine(currentCoroutine);
        }
        textDisplay.text = "";
        TextIconSet sentence = sentences.Dequeue();
        currentSentence = sentence.text;
        currentCoroutine = TypeText(currentSentence);
        StartCoroutine(currentCoroutine);
        Sprite icon = currentSpeaker.GetPortrait(sentence.emotion);
        if (icon != null)
        {
            iconDisplay.sprite = icon;
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
                textDisplay.text = currentSentence;
                Responses();
            }
            else
            {
                DisplayNextSentence();
            }
        }
    }
}
