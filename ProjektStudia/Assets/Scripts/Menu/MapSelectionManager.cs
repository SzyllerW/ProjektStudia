using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MapSelectionManager : MonoBehaviour
{
    public GameObject map2Button;

    private void Start()
    {
        if (PlayerPrefs.GetInt("Map2Unlocked", 0) == 1)
        {
            map2Button.GetComponent<Button>().interactable = true; 
        }
        else
        {
            map2Button.GetComponent<Button>().interactable = false; 
        }
    }

    public void SelectMap1()
    {
        PlayerPrefs.SetInt("SelectedMap", 1);
        SceneManager.LoadScene("CharacterSelection");
    }

    public void SelectMap2()
    {
        PlayerPrefs.SetInt("SelectedMap", 2);
        SceneManager.LoadScene("CharacterSelection");
    }
}
