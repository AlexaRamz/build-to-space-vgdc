using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class JournalUI : MonoBehaviour
{
    [SerializeField] QuestManager questManager;
    [SerializeField] Transform buttonContainer;
    [SerializeField] GameObject buttonPrefab;

    [SerializeField] TMP_Text titleText, descriptionText, moneyRewardDisplay, researchRewardDisplay;
    [SerializeField] TMP_Text victoryDescriptionText, victoryMoneyRewardDisplay, victoryResearchRewardDisplay;

    [SerializeField] Slider completionProgressBar;

    List<SelectionButton> buttons = new List<SelectionButton>();

    Quest currentSelectedQuest;

    private void OnEnable()
    {
        questManager.UIupdateList.AddListener(UpdateQuestList);
        questManager.UIupdateInfo.AddListener(UpdateQuestInfo);

        UpdateQuestList();
        ClearQuestInfo();
    }
    private void OnDisable()
    {
        questManager.UIupdateList.RemoveListener(UpdateQuestList);
        questManager.UIupdateInfo.RemoveListener(UpdateQuestInfo);
    }

    void UpdateQuestList()
    {
        DisplayQuests(questManager.activeQuests);
    }

    public void SelectQuest(int index)
    {
        currentSelectedQuest = questManager.activeQuests[index];
        UpdateQuestInfo();
    }
    public void CollectQuestReward()
    {
        if (currentSelectedQuest != null && currentSelectedQuest.questState == QuestState.Fulfilled)
        {
            questManager.CollectQuestReward(currentSelectedQuest);
            ClearQuestInfo();
        }
    }
    
    void UpdateQuestInfo()
    {
        if (currentSelectedQuest == null) return;
        QuestData questData = currentSelectedQuest.questData;

        titleText.text = questData.name;
        descriptionText.text = questData.informationText;
        moneyRewardDisplay.text = questData.moneyReward.ToString();
        researchRewardDisplay.text = questData.researchReward.ToString();

        completionProgressBar.SetValueWithoutNotify(currentSelectedQuest.progress);
        if (currentSelectedQuest.questState == QuestState.Fulfilled)
        {
            DisplayQuestCompleted();
        }

    }
    void ClearQuestInfo()
    {
        descriptionText.text = moneyRewardDisplay.text = researchRewardDisplay.text = "";
        victoryDescriptionText.text = victoryMoneyRewardDisplay.text = victoryResearchRewardDisplay.text = "";
    }
    void DisplayQuestCompleted()
    {
        QuestData questData = currentSelectedQuest.questData;

        victoryDescriptionText.text = questData.victoryText;
        victoryMoneyRewardDisplay.text = questData.moneyReward.ToString();
        victoryResearchRewardDisplay.text = questData.researchReward.ToString();
    }

    void ClearQuests()
    {
        buttons.Clear();
        foreach (Transform c in buttonContainer)
        {
            Destroy(c.gameObject);
        }
        titleText.text = descriptionText.text = moneyRewardDisplay.text = researchRewardDisplay.text = "";
    }
    void DisplayQuests(List<Quest> quests)
    {
        ClearQuests();
        for (int i = 0; i < quests.Count; i++)
        {
            int btnNo = i;
            GameObject button = Instantiate(buttonPrefab, buttonContainer);
            SelectionButton selectButton = button.GetComponent<SelectionButton>();
            selectButton.text.text = quests[i].questData.name;
            button.GetComponent<Button>().onClick.AddListener(delegate { SelectQuest(btnNo); });
            buttons.Add(selectButton);
        }
    }
}
