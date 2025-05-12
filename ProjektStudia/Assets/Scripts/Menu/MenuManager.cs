using System.Threading.Tasks;
using UnityEngine;

public class MenuManager : MonoBehaviour
{
    [SerializeField] private GameObject mainMenuPanel;
    [SerializeField] private GameObject optionsMenuPanel;
    [SerializeField] private GameObject soundSettingsPanel;
    [SerializeField] private GameObject resolutionSettingsPanel;
    [SerializeField] private AudioClip buttonSoundClip;

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

        mainMenuPanel.SetActive(false);
        optionsMenuPanel.SetActive(true);
        soundSettingsPanel.SetActive(false);
        resolutionSettingsPanel.SetActive(false);
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

        optionsMenuPanel.SetActive(true);
        soundSettingsPanel.SetActive(false);
        resolutionSettingsPanel.SetActive(false);
    }

    public async void BackToMainMenu()
    {
        SoundFXManager.instance.PlaySoundFXClip(buttonSoundClip, transform, 1f);
        await Task.Delay(100);

        optionsMenuPanel.SetActive(false);
        mainMenuPanel.SetActive(true);
    }

    public async void ExitGame()
    {
        SoundFXManager.instance.PlaySoundFXClip(buttonSoundClip, transform, 1f);
        await Task.Delay(1000);
        Application.Quit();
        Debug.Log("Quit game");
    }
}
