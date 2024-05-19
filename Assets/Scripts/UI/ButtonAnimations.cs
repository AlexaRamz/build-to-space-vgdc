using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonAnimations : MonoBehaviour
{
    public UIAnimator UIAnimator;
    public Image targetImage;
    public RectTransform targetTransform;
    public float fadeDuration = 0.25f;
    public float scaleDuration = 0.25f;

    bool isOnButton;
    bool isHeldDown;

    public void AnimateFade(float targetAlpha)
    {
        UIAnimator.AnimateFade(targetImage, targetAlpha, fadeDuration);
    }

    public void AnimateScale(float targetScale)
    {
        Vector2 targetSize = new Vector2(targetScale, targetScale);
        UIAnimator.AnimateScale(targetTransform, targetSize, scaleDuration);
    }

    public void OnPointerEnter()
    {
        isOnButton = true;
    }
    public void OnPointerExit()
    {
        isOnButton = false;
    }
    public void OnPointerDown()
    {
        isHeldDown = true;
    }
    public void OnPointerUp()
    {
        isHeldDown = true;
    }

    public void OnPointerExitScale(float onButtonScale)
    {
        isOnButton = false;
        if (!isHeldDown)
        {
            AnimateScale(onButtonScale);
        }
    }
    public void OnPointerUpScale(float onButtonScale)
    {
        isHeldDown = false;
        if (isOnButton)
        {
            AnimateScale(onButtonScale);
        }
        else
        {
            AnimateScale(1f);
        }
    }
}
