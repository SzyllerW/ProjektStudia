using UnityEngine;
using UnityEngine.SceneManagement;

public class BackToPreviousScene : MonoBehaviour
{
    public static void SaveCurrentScene()
    {
        PlayerPrefs.SetString("PreviousScene", SceneManager.GetActiveScene().name);
        PlayerPrefs.Save();
    }
}