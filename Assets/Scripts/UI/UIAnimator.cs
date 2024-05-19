using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class UIAnimator : MonoBehaviour
{
    public void AnimateFade(Image image, float targetAlpha, float duration)
    {
        float startAlpha = image.color.a;
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

    public void AnimateScale(RectTransform transform, Vector2 targetSize, float duration)
    {
        Vector2 startSize = transform.localScale;
        StartCoroutine(ScaleCoroutine(transform, startSize, targetSize, duration));
    }

    IEnumerator ScaleCoroutine(RectTransform transform, Vector2 startSize, Vector2 targetSize, float duration)
    {
        float elapsedTime = 0f;
        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            transform.localScale = Vector2.Lerp(startSize, targetSize, elapsedTime / duration);
            yield return null;
        }
    }
}