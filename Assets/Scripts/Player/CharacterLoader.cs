using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterLoader : MonoBehaviour
{
    public Character plrChar;
    private void Start()
    {
        plrChar.LoadChar(gameObject);
    }
}
