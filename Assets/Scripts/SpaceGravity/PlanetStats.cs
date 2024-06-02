using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlanetStats : MonoBehaviour
{
    [SerializeField]
    public float mass = 2; //Sets default mass value to 2 for a planet
    [SerializeField]
    public SceneAsset scene;
    [SerializeField]
    public float minimumOrbit = 0f;
    [SerializeField]
    public float maximumOrbit = 0f;
    [SerializeField]
    private Image fadeAwayImage; //Reference to full UI black image to fade into

    private void Start()
    {
        if(fadeAwayImage != null)
        {
            fadeAwayImage.gameObject.SetActive(false);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (scene != null)
        {
            Collider2D collider = GetComponent<Collider2D>();
            if (collider != null)
            {
                collider.enabled = false; //Disables collision    
            }
            string sceneString = System.IO.Path.GetFileNameWithoutExtension(AssetDatabase.GetAssetPath(scene));
            StartCoroutine(FadeAndLoadScene(sceneString));
        }
    }

    private IEnumerator FadeAndLoadScene(string sceneString)
    {
        if (fadeAwayImage != null) //Fades if there is a given image, otherwise just opens level instantly
        {
            fadeAwayImage.gameObject.SetActive(true);
            for (float time = 0.01f; time < 1; time += Time.deltaTime / 1.0f)
            {
                Color updateColor = fadeAwayImage.color;
                updateColor.a = Mathf.Lerp(0, 1, time);
                fadeAwayImage.color = updateColor;
                yield return null;
            }
            SceneManager.LoadScene(sceneString); //Loads level when collided with
        }
        else
        {
            SceneManager.LoadScene(sceneString); //Loads level when collided with
        }
    }
}
