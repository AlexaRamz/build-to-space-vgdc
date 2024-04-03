using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SelectionButton : MonoBehaviour
{
    public Image image;
    public Image selectionImage;

    public void SetImage(Sprite sprite)
    {
        image.sprite = sprite;
    }
    public void SetSelection(bool selectionBool)
    {
        selectionImage.color = new Color(selectionImage.color.r, selectionImage.color.g, selectionImage.color.b, selectionBool ? 1 : 0);
    }
}
