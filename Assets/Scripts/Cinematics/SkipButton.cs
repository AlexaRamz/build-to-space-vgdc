using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SkipButton : MonoBehaviour
{
    public float skipTime = 2.5f;
    float increaseSpeed;
    public Slider slider;
    public float reboundSpeed = 2f;
    public Image buttonImage;
    public float disappearTime = 5f;
    public float fadeInDuration = 0.1f;
    public float fadeOutDuration = 0.25f;

    bool isOnButton = false;
    Coroutine currentCoroutine;
    Coroutine disappearCoroutine;
    bool visible = false;
    Vector2 previousMousePos;
    bool forceOff = true;
    public UIAnimator UIAnimator;

    private void Start()
    {
        increaseSpeed = 1f / skipTime;
        buttonImage.color = new Color(buttonImage.color.r, buttonImage.color.g, buttonImage.color.b, 0f);
        StartCoroutine(BeginOff());
    }
    public void OnPointerEnter()
    {
        if (currentCoroutine != null)
        {
            StopCoroutine(currentCoroutine);
        }
        currentCoroutine = StartCoroutine(SkipCoroutine());
        isOnButton = true;
    }
    IEnumerator BeginOff()
    {
        yield return new WaitForSeconds(0.1f);
        forceOff = false;
    }

    IEnumerator SkipCoroutine()
    {
        while (slider.value < 1)
        {
            slider.value += increaseSpeed * Time.deltaTime;
            yield return null;
        }
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
    IEnumerator ReboundCoroutine()
    {
        while (slider.value > 0)
        {
            slider.value -= reboundSpeed * Time.deltaTime;
            yield return null;
        }
    }

    IEnumerator DisappearCoroutine()
    {
        yield return new WaitForSeconds(disappearTime);
        UIAnimator.AnimateFade(buttonImage, 0f, fadeOutDuration);
        visible = false;
    }
    void Update()
    {
        if (!isOnButton)
        {
            Vector2 currentMousePos = Input.mousePosition;
            bool mouseMoved = Vector2.Distance(previousMousePos, currentMousePos) > 0.01f;
            if (!forceOff && (mouseMoved || Input.anyKeyDown))
            {
                if (disappearCoroutine != null)
                {
                    StopCoroutine(disappearCoroutine);
                    disappearCoroutine = null;
                }
                if (!visible)
                {
                    UIAnimator.AnimateFade(buttonImage, 1f, fadeInDuration);
                    visible = true;
                }
            }
            else
            {
                if (disappearCoroutine == null)
                {
                    disappearCoroutine = StartCoroutine(DisappearCoroutine());
                }
            }
            previousMousePos = currentMousePos;
        }
    }
    public void OnPointerExit()
    {
        if (currentCoroutine != null)
        {
            StopCoroutine(currentCoroutine);
        }
        StopCoroutine(currentCoroutine);
        currentCoroutine = StartCoroutine(ReboundCoroutine());
        isOnButton = false;
    }
}
