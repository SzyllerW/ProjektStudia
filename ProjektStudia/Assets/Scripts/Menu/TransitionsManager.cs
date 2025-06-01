using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class TransitionsManager : MonoBehaviour
{
    public static TransitionsManager instance;

    [SerializeField] private bool playOnStart = true;
    [SerializeField] private Image img;
    [SerializeField] private GameObject canvas;
    [SerializeField] private float duration = 0.5f;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        canvas.SetActive(false);

        if (playOnStart)
        {
            canvas.SetActive(true);
            img.color = new Color(0f, 0f, 0f, 1f);
            StartCoroutine(fadeEntry());
        }
    }

    public IEnumerator fadeExit(string nextScene)
    {
        float elapsedTime = 0f;
        canvas.SetActive(true);

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float alpha = Mathf.Lerp(0f, 1f, elapsedTime / duration);
            img.color = new Color(0f, 0f, 0f, alpha);
            yield return null;
        }

        img.color = new Color(0f, 0f, 0f, 1f);
        SceneManager.LoadScene(nextScene);
    }

    IEnumerator fadeEntry()
    {
        float elapsedTime = 0f;

        yield return new WaitForSeconds(0.5f);

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float alpha = Mathf.Lerp(1f, 0f, elapsedTime / duration);
            img.color = new Color(0f, 0f, 0f, alpha);
            yield return null;
        }

        img.color = new Color(0f, 0f, 0f, 0f);
        canvas.SetActive(false);
    }

}
