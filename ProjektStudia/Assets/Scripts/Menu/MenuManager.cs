using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Audio;

public class MenuManager : MonoBehaviour
{
    [SerializeField] private AudioMixer audioMixer;
    [SerializeField] private AudioClip buttonSoundClip;

    private void Start()
    {
        LoadSettings();
    }

    public async void PlayGame()
    {
        SoundFXManager.instance.PlaySoundFXClip(buttonSoundClip, transform, 1f);
        await Task.Delay(100);
        UnityEngine.SceneManagement.SceneManager.LoadScene("MapSelection");
    }

    public async void OpenOptions()
    {
        SoundFXManager.instance.PlaySoundFXClip(buttonSoundClip, transform, 1f);
        await Task.Delay(100);
        UnityEngine.SceneManagement.SceneManager.LoadScene("Settings");
    }

    public async void BackToMainMenu()
    {
        SoundFXManager.instance.PlaySoundFXClip(buttonSoundClip, transform, 1f);
        await Task.Delay(100);
        UnityEngine.SceneManagement.SceneManager.LoadScene("MainMenu");
    }

    public async void ExitGame()
    {
        SoundFXManager.instance.PlaySoundFXClip(buttonSoundClip, transform, 1f);
        await Task.Delay(1000);
        Application.Quit();
        Debug.Log("Quit game");
    }

    private void LoadSettings()
    {
        if (PlayerPrefs.HasKey("masterVolume"))
        {
            float savedVolume = PlayerPrefs.GetFloat("masterVolume");
            audioMixer.SetFloat("masterVolume", Mathf.Log10(savedVolume) * 20f);
        }

        if (PlayerPrefs.HasKey("soundFXVolume"))
        {
            float savedFXVolume = PlayerPrefs.GetFloat("soundFXVolume");
            audioMixer.SetFloat("soundFXVolume", Mathf.Log10(savedFXVolume) * 20f);
        }

        if (PlayerPrefs.HasKey("musicVolume"))
        {
            float savedMusicVolume = PlayerPrefs.GetFloat("musicVolume");
            audioMixer.SetFloat("musicVolume", Mathf.Log10(savedMusicVolume) * 20f);
        }
    }
}
