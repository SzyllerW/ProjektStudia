using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    public void PlayGame()
    {
        SceneManager.LoadScene("MapSelection"); 
    }

    public void OpenOptions()
    {
        // Implementacja menu ustawie�
    }

    public void ExitGame()
    {
        Application.Quit();
        Debug.Log("Quit game");
    }
}