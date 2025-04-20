using UnityEngine;
using UnityEngine.SceneManagement;
using System.Threading.Tasks;

public class MainMenuSettingsManager : MonoBehaviour
{
    [SerializeField] private GameObject mainMenuPanel;
    [SerializeField] private GameObject optionsMenuPanel;
    [SerializeField] private GameObject soundSettingsPanel;
    [SerializeField] private GameObject resolutionSettingsPanel;
    [SerializeField] private AudioClip buttonSoundClip;

    public async void OpenOptionsMenu()
    {
        SoundFXManager.instance.PlaySoundFXClip(buttonSoundClip, transform, 1f);
        await Task.Delay(100);

        mainMenuPanel.SetActive(false);
        optionsMenuPanel.SetActive(true);
    }

    public async void OpenSoundSettings()
    {
        SoundFXManager.instance.PlaySoundFXClip(buttonSoundClip, transform, 1f);
        await Task.Delay(100);

        optionsMenuPanel.SetActive(false);
        soundSettingsPanel.SetActive(true);
    }

    public async void OpenResolutionSettings()
    {
        SoundFXManager.instance.PlaySoundFXClip(buttonSoundClip, transform, 1f);
        await Task.Delay(100);

        optionsMenuPanel.SetActive(false);
        resolutionSettingsPanel.SetActive(true);
    }

    public async void BackToOptionsMenu()
    {
        SoundFXManager.instance.PlaySoundFXClip(buttonSoundClip, transform, 1f);
        await Task.Delay(100);

        soundSettingsPanel.SetActive(false);
        resolutionSettingsPanel.SetActive(false);
        optionsMenuPanel.SetActive(true);
    }

    public async void BackToMainMenu()
    {
        SoundFXManager.instance.PlaySoundFXClip(buttonSoundClip, transform, 1f);
        await Task.Delay(100);

        optionsMenuPanel.SetActive(false);
        mainMenuPanel.SetActive(true);
    }
}

