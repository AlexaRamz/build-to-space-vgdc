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

    List<SelectionButton> buttons = new List<SelectionButton>();

    void UpdateQuests()
    {
        DisplayQuests(questManager.questDatas);
    }

    public void ShowQuest(int index)
    {

    }
    void ClearQuests()
    {
        buttons.Clear();
        foreach (Transform c in buttonContainer)
        {
            Destroy(c.gameObject);
        }
    }
    void DisplayQuests(List<QuestData> quests)
    {
        ClearQuests();
        //nameText.text = descriptionText.text = "";
        for (int i = 0; i < quests.Count; i++)
        {
            int index = i;
            GameObject button = Instantiate(buttonPrefab, buttonContainer);
            SelectionButton selectButton = button.GetComponent<SelectionButton>();
            selectButton.text.text = quests[i].name;
            button.GetComponent<Button>().onClick.AddListener(delegate { ShowQuest(index); });
            buttons.Add(selectButton);
        }
    }
}
