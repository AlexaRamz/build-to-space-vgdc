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

    public void SetResource(ResourceInfo info)
    {
        nameText.text = info.resource.type.ToString().ToLower();
        image.sprite = info.resource.image;
        UpdateAmount(info.amount);
    }
    public void SetResourceSimple(ResourceInfo info)
    {
        image.sprite = info.resource.image;
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
