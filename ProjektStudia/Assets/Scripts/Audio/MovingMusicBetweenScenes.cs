using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingMusicBetweenScenes : MonoBehaviour
{
    [SerializeField] AudioSource audioSource;
    [SerializeField] AudioClip mainMenuClip;
    [SerializeField] AudioClip[] backgroundMusic;

    private int lastIndex = -1;
    private float fadeDuration = 1.5f;

    private void Awake()
    {
        GameObject[] musicObj = GameObject.FindGameObjectsWithTag("GameMusic");

        if (musicObj.Length > 1)
        {
            Destroy(this.gameObject);
        }

        DontDestroyOnLoad(this.gameObject);
    }

    void Start()
    {
        playMainMenuMusic();
    }

    public void playMainMenuMusic()
    {
        StartCoroutine(FadeIn(mainMenuClip));
        audioSource.loop = true;
    }

    public void playRandomMusic()
    {
        StopAllCoroutines();
        StartCoroutine(FadeOutAndChangeMusic());
        StartCoroutine(CheckAudioEnd());
        audioSource.loop = false;
    }

    private IEnumerator CheckAudioEnd()
    {
        yield return new WaitUntil(() => !audioSource.isPlaying);
        yield return StartCoroutine(FadeOutAndChangeMusic());
    }

    private IEnumerator FadeOutAndChangeMusic()
    {
        yield return StartCoroutine(FadeOut());

        int newIndex;
        do
        {
            newIndex = Random.Range(0, backgroundMusic.Length);
        }
        while (newIndex == lastIndex);

        lastIndex = newIndex;

        yield return StartCoroutine(FadeIn(backgroundMusic[newIndex]));
    }

    private IEnumerator FadeOut()
    {
        float startVolume = audioSource.volume;

        while (audioSource.volume > 0)
        {
            audioSource.volume -= startVolume * Time.deltaTime / fadeDuration;
            yield return null;
        }

        audioSource.Stop();
    }

    private IEnumerator FadeIn(AudioClip newClip)
    {
        audioSource.clip = newClip;
        audioSource.Play();
        audioSource.volume = 0;

        while (audioSource.volume < 1)
        {
            audioSource.volume += Time.deltaTime / fadeDuration;
            yield return null;
        }
    }
}