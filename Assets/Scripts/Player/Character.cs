using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Character", menuName = "Character", order = 1)]
public class Character : ScriptableObject
{
    public Sprite hair;
    public Sprite hat;
    public Sprite suit;

    public void UpdateChar(GameObject plr, Sprite newHair, Sprite newHat)
    {
        plr.transform.Find("Hair").GetComponent<SpriteRenderer>().sprite = newHair;
        plr.transform.Find("Hat").GetComponent<SpriteRenderer>().sprite = newHat;
    }
    public void LoadChar(GameObject plr)
    {
        plr.transform.Find("Hair").GetComponent<SpriteRenderer>().sprite = hair;
        plr.transform.Find("Hat").GetComponent<SpriteRenderer>().sprite = hat;
    }
    public void SaveChar(Sprite newHair, Sprite newHat)
    {
        hair = newHair;
        hat = newHat;
    }
    public void ClearSave()
    {
        hair = null;
        hat = null;
    }
}
