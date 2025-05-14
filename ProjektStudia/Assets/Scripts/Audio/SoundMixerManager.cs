using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SoundMixerManager : MonoBehaviour
{
    [SerializeField] private AudioMixer audioMixer;
    [SerializeField] private Slider masterSlider;
    [SerializeField] private Slider soundFXSlider;
    [SerializeField] private Slider musicSlider;

    private static string previousScene; // Zmienna przechowuj�ca nazw� poprzedniej sceny

    private void Start()
    {
        // Zapisuje nazw� sceny, zanim przejdziemy do opcji (tylko je�li nie jeste�my ju� w menu opcji)
        if (SceneManager.GetActiveScene().name != "OptionsMenu")
        {
            previousScene = SceneManager.GetActiveScene().name;
        }

        LoadSettings();
    }

    public void SetMasterVolume(float level)
    {
        audioMixer.SetFloat("masterVolume", Mathf.Log10(level) * 20f);
        PlayerPrefs.SetFloat("masterVolume", level);
        PlayerPrefs.Save();
    }

    public void SetSoundFXVolume(float level)
    {
        audioMixer.SetFloat("soundFXVolume", Mathf.Log10(level) * 20f);
        PlayerPrefs.SetFloat("soundFXVolume", level);
        PlayerPrefs.Save();

    }

    public void SetMusicVolume(float level)
    {
        audioMixer.SetFloat("musicVolume", Mathf.Log10(level) * 20f);
        PlayerPrefs.SetFloat("musicVolume", level);
        PlayerPrefs.Save();
    }

    private void LoadSettings()
    {
        if (PlayerPrefs.HasKey("masterVolume"))
        {
            float savedVolume = PlayerPrefs.GetFloat("masterVolume");
            audioMixer.SetFloat("masterVolume", Mathf.Log10(savedVolume) * 20f);
            masterSlider.value = savedVolume;
        }

        if (PlayerPrefs.HasKey("soundFXVolume"))
        {
            float savedFXVolume = PlayerPrefs.GetFloat("soundFXVolume");
            audioMixer.SetFloat("soundFXVolume", Mathf.Log10(savedFXVolume) * 20f);
            soundFXSlider.value = savedFXVolume;
        }

        if (PlayerPrefs.HasKey("musicVolume"))
        {
            float savedMusicVolume = PlayerPrefs.GetFloat("musicVolume");
            audioMixer.SetFloat("musicVolume", Mathf.Log10(savedMusicVolume) * 20f);
            musicSlider.value = savedMusicVolume;
        }
    }

    // Wraca do poprzedniej sceny zamiast do MainMenu
    public void BackToPreviousScene()
    {
        Time.timeScale = 1f; // Upewniamy si�, �e gra nie jest spauzowana

        if (!string.IsNullOrEmpty(previousScene))
        {
            SceneManager.LoadScene(previousScene);
        }
        else
        {
            Debug.LogWarning("Brak zapisanej poprzedniej sceny! Wr�cimy do MainMenu.");
            SceneManager.LoadScene("MainMenu"); // Je�li nie ma zapisanej sceny, wracamy do MainMenu
        }
    }
}
