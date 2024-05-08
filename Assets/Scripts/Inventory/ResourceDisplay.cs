using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ResourceDisplay : MonoBehaviour
{
    public Text nameText;
    public Image image;
    public Image arrowImage;
    public Text amountText;

    public void SetResource(ItemAmountInfo info)
    {
        nameText.text = info.item.name;
        image.sprite = info.item.image;
        UpdateAmount(info.amount);
    }
    public void SetResourceSimple(ItemAmountInfo info)
    {
        image.sprite = info.item.image;
        UpdateAmount(info.amount);
    }
    public void UpdateAmount(int amount)
    {
        amountText.text = amount.ToString();
    }
    public void ArrowOn()
    {
        arrowImage.enabled = true;
    }
    public void ArrowOff()
    {
        arrowImage.enabled = false;
    }
}
