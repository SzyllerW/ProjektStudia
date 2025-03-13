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
                // Nie prze³adowuj sceny, tylko zamknij menu ustawieñ i wznow grê
                gameObject.SetActive(false);
                Time.timeScale = 1f;
            }
            else
            {
                SceneManager.LoadScene(previousScene);
                Time.timeScale = 1f; // Upewnij siê, ¿e czas gry jest wznowiony
            }
        }
        else
        {
            Debug.LogError("Nie znaleziono poprawnej poprzedniej sceny! Powrót do MainMenu.");
            SceneManager.LoadScene("MainMenu");
        }
    }
}
