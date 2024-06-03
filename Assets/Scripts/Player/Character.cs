using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Character", menuName = "Character", order = 1)]
public class Character : ScriptableObject
{
    public Sprite hair;
    public Color hairColor;
    public Sprite hat;
    public Color hatColor;
    public Sprite suit;

    public void LoadChar(GameObject plr)
    {
        SpriteRenderer hairSprite = plr.transform.Find("Hair").GetComponent<SpriteRenderer>();
        hairSprite.sprite = hair;
        hairSprite.color = hairColor;
        SpriteRenderer hatSprite = plr.transform.Find("Hat").GetComponent<SpriteRenderer>();
        hatSprite.sprite = hat;
        hatSprite.color = hatColor;
    }
    public void SaveChar(Sprite newHair, Sprite newHat, Color newHairColor, Color newHatColor)
    {
        hair = newHair;
        hat = newHat;
        hairColor = newHairColor;
        hatColor = newHatColor;
    }
    public void ClearSave()
    {
        hair = null;
        hat = null;
        hairColor = new Color(1f, 1f, 1f);
        hatColor = new Color(1f, 1f, 1f);
    }
}
