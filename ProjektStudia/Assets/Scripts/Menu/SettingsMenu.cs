using UnityEngine;
using UnityEngine.SceneManagement;
using System.Threading.Tasks;

public class SettingsMenu : MonoBehaviour
{
    [SerializeField] private AudioClip buttonSoundClip;
    
    public async void BackToPreviousScene()
    {
        SoundFXManager.instance.PlaySoundFXClip(buttonSoundClip, transform, 1f);
        await Task.Delay(100);

        SceneManager.LoadScene("MainMenu");
    }
}
