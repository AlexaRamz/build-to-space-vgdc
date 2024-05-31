using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneLoader : MonoBehaviour
{
    public float fadeTime = 1.0f;
    public AsyncOperation sceneLoadOperation;
    [SerializeField] GameObject fadeUI;
    [SerializeField] Image fadeImage;

    public static SceneLoader Instance;
    bool loading = false;

    private void Awake()
    {
        Instance = this;
        StartCoroutine(FadeIn());
    }

    public void LoadScene(string sceneName, bool withFade = true)
    {
        if (loading) return;
        loading = true;

        if (withFade)
        {
            // Start loading scene asynchronously
            sceneLoadOperation = SceneManager.LoadSceneAsync(sceneName);
            sceneLoadOperation.allowSceneActivation = false; // Prevent automatic scene switch

            // Start fade to black coroutine
            StartCoroutine(FadeToBlack());
        }
        else
        {
            SceneManager.LoadScene(sceneName);
        }
    }
    public void LoadScene(int buildIndex, bool withFade = true)
    {
        if (loading) return;
        loading = true;

        if (withFade)
        {
            // Start loading scene asynchronously
            sceneLoadOperation = SceneManager.LoadSceneAsync(buildIndex);
            sceneLoadOperation.allowSceneActivation = false; // Prevent automatic scene switch

            // Start fade to black coroutine
            StartCoroutine(FadeToBlack());
        }
        else
        {
            SceneManager.LoadScene(buildIndex);
        }
    }

    IEnumerator FadeToBlack()
    {
        fadeUI.SetActive(true);
        float currentTime = 0f;
        while (currentTime < fadeTime)
        {
            float fadeAmount = Mathf.Lerp(0f, 1f, currentTime / fadeTime);
            fadeImage.color = new Color(0f, 0f, 0f, fadeAmount);
            currentTime += Time.deltaTime;
            yield return null;
        }

        // Allow scene activation after fade completes
        sceneLoadOperation.allowSceneActivation = true;
    }

    void Update()
    {
        // Check if scene has loaded
        if (sceneLoadOperation != null && sceneLoadOperation.isDone)
        {
            // Start fade in coroutine on scene load completion
            StartCoroutine(FadeIn());
        }
    }

    IEnumerator FadeIn()
    {
        float currentTime = 0f;
        fadeUI.SetActive(true);
        while (currentTime < fadeTime)
        {
            float fadeAmount = Mathf.Lerp(1.0f, 0.0f, currentTime / fadeTime);
            fadeImage.color = new Color(0f, 0f, 0f, fadeAmount);
            currentTime += Time.deltaTime;
            yield return null;
        }
        fadeUI.SetActive(false);

        // Reset sceneLoadOperation for next use
        sceneLoadOperation = null;
    }
}