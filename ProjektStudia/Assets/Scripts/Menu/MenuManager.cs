using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    [SerializeField] private AudioClip buttonSoundClip;

    public async void PlayGame()
    {
        SoundFXManager.instance.PlaySoundFXClip(buttonSoundClip, transform, 1f);
        await Task.Delay(100);
        SceneManager.LoadScene("MapSelection"); 
    }

    public async void OpenOptions()
    {
        SoundFXManager.instance.PlaySoundFXClip(buttonSoundClip, transform, 1f);
        PlayerPrefs.SetString("PreviousScene", "MainMenu");
        PlayerPrefs.Save();
        await Task.Delay(100);
        SceneManager.LoadScene("Settings");
    }

    public async void ExitGame()
    {
        SoundFXManager.instance.PlaySoundFXClip(buttonSoundClip, transform, 1f);
        await Task.Delay(1000);
        Application.Quit();
        Debug.Log("Quit game");
    }
}