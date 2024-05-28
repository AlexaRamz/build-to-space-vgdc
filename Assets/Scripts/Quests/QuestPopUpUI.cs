using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class QuestPopUpUI : MonoBehaviour
{
    [SerializeField] QuestManager questManager;
    [SerializeField] TMP_Text descriptionText;
    [SerializeField] GameObject popUp;
    RectTransform rectTransform;
    public float showTime = 4f;
    public float fadeTime = 1f;
    Coroutine currentCoroutine;

    private void Start()
    {
        rectTransform = GetComponent<RectTransform>();
    }
    private void OnEnable()
    {
        questManager.questAdded.AddListener(AnnounceQuest);
        questManager.questCompleted.AddListener(AnnounceQuestCompleted);
    }
    private void OnDisable()
    {
        questManager.questAdded.RemoveListener(AnnounceQuest);
        questManager.questCompleted.RemoveListener(AnnounceQuestCompleted);
    }

    void AnnounceQuest(QuestData quest)
    {
        ShowPopUp("New mission: " + quest.name);
    }

    void AnnounceQuestCompleted(QuestData quest)
    {
        ShowPopUp("Mission complete! " + quest.victoryText);
    }

    void ShowPopUp(string description)
    {
        descriptionText.text = description;
        popUp.SetActive(true);

        rectTransform.anchoredPosition = new Vector2(rectTransform.rect.width, rectTransform.anchoredPosition.y);
        Vector2 targetPosition = new Vector2(rectTransform.localPosition.y + rectTransform.rect.width, rectTransform.localPosition.y);
        LeanTween.moveLocal(gameObject, targetPosition, 0.4f).setEase(LeanTweenType.easeInOutCubic);

        if (currentCoroutine != null) StopCoroutine(currentCoroutine);
        currentCoroutine = StartCoroutine(DisappearCoroutine());
    }


    IEnumerator DisappearCoroutine()
    {
        yield return new WaitForSeconds(showTime);
        /*Color endColor = new Color(1f, 1f, 1f, 0f);
        Color startColor = Color.white;
        descriptionText.color = startColor;

        float activeTime = 0f;
        if (activeTime < 2f) // Waits 2 seconds before fading
        {
            activeTime += Time.deltaTime;
            yield return null;
        }
        while (activeTime < 5.5f) // Sets time to fade to be 3.5 seconds
        {
            float currentTimeFactor = activeTime / 3.5f;
            descriptionText.color = Color.Lerp(startColor, endColor, currentTimeFactor); // Fades color to clear over 3.5 seconds
            activeTime += Time.deltaTime;
            yield return null;
        }

        descriptionText.color = endColor;
        descriptionText.text = "";*/
        popUp.SetActive(false);
    }
}
