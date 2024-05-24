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
    }
    private void OnDisable()
    {
        questManager.questAdded.RemoveListener(AnnounceQuest);
    }

    IEnumerator DisappearCoroutine()
    {
        yield return new WaitForSeconds(showTime);
        popUp.SetActive(false);
    }

    void AnnounceQuest(QuestData quest)
    {
        descriptionText.text = "New mission! " + quest.name;

        popUp.SetActive(true);
        rectTransform.anchoredPosition = new Vector2(rectTransform.rect.width, rectTransform.anchoredPosition.y);
        Vector2 targetPosition = new Vector2(rectTransform.localPosition.y + rectTransform.rect.width, rectTransform.localPosition.y);
        LeanTween.moveLocal(gameObject, targetPosition, 0.4f).setEase(LeanTweenType.easeInOutCubic);
        if (currentCoroutine != null) StopCoroutine(currentCoroutine);
        currentCoroutine = StartCoroutine(DisappearCoroutine());
    }
}
