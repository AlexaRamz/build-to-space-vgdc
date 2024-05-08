using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SelectionButton : MonoBehaviour
{
    public Image image;
    [SerializeField] Image selectionImage;
    public TMP_Text text;

    public void SetSelection(bool selectionBool)
    {
        selectionImage.color = new Color(selectionImage.color.r, selectionImage.color.g, selectionImage.color.b, selectionBool ? 1 : 0);
    }
}
