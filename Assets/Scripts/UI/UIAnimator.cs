using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class UIAnimator : MonoBehaviour
{
    public void AnimateFade(Image image, float targetAlpha, float duration)
    {
        float startAlpha = image.color.a; // Get current alpha
        StartCoroutine(FadeCoroutine(image, startAlpha, targetAlpha, duration));
    }

    IEnumerator FadeCoroutine(Image image, float startAlpha, float targetAlpha, float duration)
    {
        float elapsedTime = 0f;
        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            image.color = new Color(image.color.r, image.color.g, image.color.b, Mathf.Lerp(startAlpha, targetAlpha, elapsedTime / duration));
            yield return null;
        }
    }
}