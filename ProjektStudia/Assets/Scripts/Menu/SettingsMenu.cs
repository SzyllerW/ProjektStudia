using UnityEngine;
using UnityEngine.SceneManagement;
using System.Threading.Tasks;

public class SettingsMenu : MonoBehaviour
{
    [SerializeField] AudioClip buttonSoundClip;
    
    public async void BackToPreviousScene()
    {
        string previousScene = PlayerPrefs.GetString("PreviousScene", "");

        if (!string.IsNullOrEmpty(previousScene))
        {
            Debug.Log("Powrót do sceny: " + previousScene);

            if (previousScene == SceneManager.GetActiveScene().name)
            {
                gameObject.SetActive(false);
                Time.timeScale = 1f;
            }
            else
            {
                SoundFXManager.instance.PlaySoundFXClip(buttonSoundClip, transform, 1f);
                await Task.Delay(100);
                SceneManager.LoadScene(previousScene);
                Time.timeScale = 1f;
            }
        }
        else
        {
            Debug.LogError("Nie znaleziono poprawnej poprzedniej sceny! Powrót do MainMenu.");
            SceneManager.LoadScene("MainMenu");
        }
    }
}
