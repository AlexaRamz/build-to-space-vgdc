using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New NPC", menuName = "NPC")]
public class NPC : ScriptableObject
{
    public enum Emotion
    {
        Neutral,
        Happy,
        Sad,
        Angry,
        Shocked,
        Flattered,
    }
    public Sprite[] portraits = new Sprite[6]; // Assign in above order
    public List<Dialogue> dialogues;

    public Sprite GetPortrait(Emotion em = Emotion.Neutral)
    {
        return portraits[(int)em];
    }
}
