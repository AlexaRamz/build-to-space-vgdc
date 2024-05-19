using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [SerializeField] AudioSource musicSource;
    [SerializeField] AudioSource SFXSource;

    public AudioClip background1;
    public AudioClip background2;
    public AudioClip rocket;

    public static AudioManager instance;
    private float pauseTime = 6f;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        StartCoroutine(PlayBackgroundLoop());
    }

    IEnumerator PlayBackgroundLoop()
    {
        while (true)
        {
            for (int i = 0; i < 2; i++)
            {
                musicSource.clip = background1;
                musicSource.Play();

                yield return new WaitForSeconds(musicSource.clip.length); // Play the audio

                musicSource.Stop();
                yield return new WaitForSeconds(pauseTime); // Pause before looping
            }
            for (int i = 0; i < 2; i++)
            {
                musicSource.clip = background2;
                musicSource.Play();

                yield return new WaitForSeconds(musicSource.clip.length); // Play the audio

                musicSource.Stop();
                yield return new WaitForSeconds(pauseTime); // Pause before looping
            }
        }
    }

    public void PlaySFX(AudioClip clip)
    {
        SFXSource.PlayOneShot(clip);
    }

    public void StopSFX()
    {
        SFXSource.Stop();
    }
}
