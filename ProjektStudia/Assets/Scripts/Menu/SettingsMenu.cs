using UnityEngine;
using UnityEngine.SceneManagement;

public class SettingsMenu : MonoBehaviour
{
    public void BackToPreviousScene()
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
