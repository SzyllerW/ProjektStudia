using UnityEngine;
using UnityEngine.SceneManagement;
using System.Threading.Tasks;

public class SettingsMenu : MonoBehaviour
{
    [SerializeField] private AudioClip buttonSoundClip;
    
    public async void BackToPreviousScene()
    {
        string previousScene = PlayerPrefs.GetString("PreviousScene", "");
        SoundFXManager.instance.PlaySoundFXClip(buttonSoundClip, transform, 1f);
        await Task.Delay(100);

        if (!string.IsNullOrEmpty(previousScene))
        {
            Debug.Log("Powr�t do sceny: " + previousScene);

            if (previousScene == SceneManager.GetActiveScene().name)
            {
                gameObject.SetActive(false);
                Time.timeScale = 1f;
            }
            else
            {
                SceneManager.LoadScene(previousScene);
                Time.timeScale = 1f;
            }
        }
        else
        {
            Debug.LogError("Nie znaleziono poprawnej poprzedniej sceny! Powr�t do MainMenu.");
            SceneManager.LoadScene("MainMenu");
        }
    }
}
