using UnityEngine;
using UnityEngine.SceneManagement;

public class SettingsMenu : MonoBehaviour
{
    public void BackToPreviousScene()
    {
        string previousScene = PlayerPrefs.GetString("PreviousScene", "");

        if (!string.IsNullOrEmpty(previousScene))
        {
            Debug.Log("Powr�t do sceny: " + previousScene);

            if (previousScene == SceneManager.GetActiveScene().name)
            {
                // Nie prze�adowuj sceny, tylko zamknij menu ustawie� i wznow gr�
                gameObject.SetActive(false);
                Time.timeScale = 1f;
            }
            else
            {
                SceneManager.LoadScene(previousScene);
                Time.timeScale = 1f; // Upewnij si�, �e czas gry jest wznowiony
            }
        }
        else
        {
            Debug.LogError("Nie znaleziono poprawnej poprzedniej sceny! Powr�t do MainMenu.");
            SceneManager.LoadScene("MainMenu");
        }
    }
}
