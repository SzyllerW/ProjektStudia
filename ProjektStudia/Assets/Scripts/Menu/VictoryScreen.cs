using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

public class VictoryScreen : MonoBehaviour
{
    [SerializeField] private AudioClip buttonSoundClip;

    public async void LoadNextMap()
    {
        PlayerPrefs.SetInt("Map2Unlocked", 1);
        PlayerPrefs.Save();

        SoundFXManager.instance.PlaySoundFXClip(buttonSoundClip, transform, 1f);
        await Task.Delay(100);
        
        SceneManager.LoadScene("MapSelection");
    }

    public async void BackToMainMenu()
    {
        SoundFXManager.instance.PlaySoundFXClip(buttonSoundClip, transform, 1f);
        await Task.Delay(100);

        SceneManager.LoadScene("MainMenu");
    }

    public async void ReturnToMapSelection()
    {
        SoundFXManager.instance.PlaySoundFXClip(buttonSoundClip, transform, 1f);
        await Task.Delay(100);
        
        SceneManager.LoadScene("MapSelection");
    }
}