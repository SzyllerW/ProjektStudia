using UnityEngine;
using UnityEngine.SceneManagement;

public class VictoryScreen : MonoBehaviour
{
    public void LoadNextMap()
    {
        PlayerPrefs.SetInt("Map2Unlocked", 1);
        PlayerPrefs.Save();

        SceneManager.LoadScene("MapSelection");
    }

    public void BackToMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }

    public void ReturnToMapSelection()
    {
        SceneManager.LoadScene("MapSelection");
    }
}