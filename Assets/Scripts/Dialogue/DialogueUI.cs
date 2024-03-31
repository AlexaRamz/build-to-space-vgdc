using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogueUI : MonoBehaviour
{
    public Canvas responseUI;
    public GameObject responseContainer;
    public TMPro.TextMeshProUGUI textDisplay, nameDisplay;
    public Image iconDisplay;
    public GameObject responseTemplate;

    private void OnEnable()
    {
        transform.localScale = new Vector3(0, 0, 0);
        LeanTween.scale(gameObject, new Vector3(1f, 1f, 1f), 0.2f).setEase(LeanTweenType.easeInCubic);
    }
}
