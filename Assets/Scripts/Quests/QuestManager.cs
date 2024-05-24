using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "QuestManager", menuName = "Scriptable Objects/Managers/Quest Manager")]
public class QuestManager : ScriptableObject
{
    public List<QuestData> starterQuests = new List<QuestData>();
    public List<QuestData> questDatas = new List<QuestData>();
    public UnityEvent<QuestData> questAdded;

    private void OnEnable()
    {
        questDatas.Clear();
        foreach (var i in starterQuests)
        {
            questDatas.Add(i);
        }
    }

    public void AddQuest(QuestData quest)
    {
        questDatas.Add(quest);
        questAdded?.Invoke(quest);

        quest.active = true;
    }
}
