using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MapSelectionManager : MonoBehaviour
{
    public Button map1Button; 
    public Button map2Button; 
    public Button backButton; 
    public Button confirmButton; 

    private int selectedMap = 0; 
    private Color defaultColor = Color.white;
    private Color selectedColor = Color.grey;

    private void Start()
    {
        map2Button.interactable = PlayerPrefs.GetInt("Map2Unlocked", 0) == 1;

        backButton.onClick.AddListener(BackToMainMenu);
        confirmButton.onClick.AddListener(ConfirmSelection);
        confirmButton.interactable = false;

        map1Button.onClick.AddListener(() => ToggleMapSelection(1, map1Button));
        map2Button.onClick.AddListener(() => ToggleMapSelection(2, map2Button));

        ResetButtonColors();
    }

    public void ToggleMapSelection(int mapIndex, Button button)
    {
        if (selectedMap == mapIndex)
        {
            selectedMap = 0; 
            confirmButton.interactable = false;
            button.GetComponent<Image>().color = defaultColor;
        }
        else
        {
            selectedMap = mapIndex;
            confirmButton.interactable = true;
            ResetButtonColors();
            button.GetComponent<Image>().color = selectedColor; 
        }
    }

    private void ResetButtonColors()
    {
        map1Button.GetComponent<Image>().color = defaultColor;
        map2Button.GetComponent<Image>().color = defaultColor;
    }

    private void ConfirmSelection()
    {
        if (selectedMap > 0)
        {
            PlayerPrefs.SetInt("SelectedMap", selectedMap);
            SceneManager.LoadScene("CharacterSelection");
        }
    }

    private void BackToMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }
}