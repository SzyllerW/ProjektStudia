using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using TMPro;

public class CharacterSelectionManager : MonoBehaviour
{
    public List<UnityEngine.UI.Button> characterButtons; 
    public List<Sprite> characterIcons;
    public TextMeshProUGUI selectedCountText;
    public UnityEngine.UI.Button confirmButton; 
    public UnityEngine.UI.Button backButton; 

    private List<int> selectedCharacters = new List<int>();
    private int maxSelectableCharacters;

    private void Start()
    {
        int selectedMap = PlayerPrefs.GetInt("SelectedMap", 1);
        maxSelectableCharacters = (selectedMap == 1) ? 2 : 3;
        confirmButton.interactable = false;
        UpdateSelectionCount();

        for (int i = 0; i < characterButtons.Count; i++)
        {
            int index = i;
            characterButtons[i].onClick.AddListener(() => ToggleCharacterSelection(index));
        }

        confirmButton.onClick.AddListener(SaveSelectionAndLoadMap);
        backButton.onClick.AddListener(BackToPreviousScreen);
    }

    public void ToggleCharacterSelection(int characterIndex)
    {
        if (selectedCharacters.Contains(characterIndex))
        {
            selectedCharacters.Remove(characterIndex);
            characterButtons[characterIndex].interactable = true;
            characterButtons[characterIndex].GetComponent<Image>().color = Color.white;
        }
        else if (selectedCharacters.Count < maxSelectableCharacters)
        {
            selectedCharacters.Add(characterIndex);
            characterButtons[characterIndex].GetComponent<Image>().color = Color.gray;
        }

        UpdateSelectionCount();
        confirmButton.interactable = selectedCharacters.Count == maxSelectableCharacters;
    }

    private void UpdateSelectionCount()
    {
        selectedCountText.text = $"Wybrano: {selectedCharacters.Count}/{maxSelectableCharacters}";
    }

    private void SaveSelectionAndLoadMap()
    {
        PlayerPrefs.SetString("SelectedCharacters", string.Join(",", selectedCharacters));
        List<string> selectedIcons = new List<string>();
        foreach (int index in selectedCharacters)
        {
            selectedIcons.Add(characterIcons[index].name);
        }
        PlayerPrefs.SetString("SelectedIcons", string.Join(",", selectedIcons));

        int selectedMap = PlayerPrefs.GetInt("SelectedMap", 1);
        string sceneToLoad = "Map" + selectedMap;
        SceneManager.LoadScene(sceneToLoad);
    }

    private void BackToPreviousScreen()
    {
        SceneManager.LoadScene("MapSelection");
    }
}