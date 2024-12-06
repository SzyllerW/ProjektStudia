using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using TMPro;

public class CharacterSelectionManager : MonoBehaviour
{
    public List<Button> characterButtons;
    public List<Sprite> characterIcons;
    public TextMeshProUGUI selectedCountText;
    public Button confirmButton; 

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
            characterButtons[i].onClick.AddListener(() => SelectCharacter(index));
        }

        confirmButton.onClick.AddListener(SaveSelectionAndLoadMap);
    }

    public void SelectCharacter(int characterIndex)
    {
        if (!selectedCharacters.Contains(characterIndex) && selectedCharacters.Count < maxSelectableCharacters)
        {
            selectedCharacters.Add(characterIndex);
            characterButtons[characterIndex].interactable = false;

            UpdateSelectionCount();

            if (selectedCharacters.Count == maxSelectableCharacters)
            {
                confirmButton.interactable = true;
            }
        }
    }

    private void UpdateSelectionCount()
    {
        selectedCountText.text = $"Wybrano: {selectedCharacters.Count}/{maxSelectableCharacters}";
    }

    private void SaveSelectionAndLoadMap()
    {
        // Zapisz wybrane postacie
        PlayerPrefs.SetString("SelectedCharacters", string.Join(",", selectedCharacters));

        // Zapisz wybrane ikony
        List<string> selectedIcons = new List<string>();
        foreach (int index in selectedCharacters)
        {
            selectedIcons.Add(characterIcons[index].name);
        }
        PlayerPrefs.SetString("SelectedIcons", string.Join(",", selectedIcons));

        // Debugowanie zapisanych danych
        Debug.Log("SelectedCharacters: " + PlayerPrefs.GetString("SelectedCharacters"));
        Debug.Log("SelectedIcons: " + PlayerPrefs.GetString("SelectedIcons"));

        // Pobierz wybran¹ mapê
        int selectedMap = PlayerPrefs.GetInt("SelectedMap", 1);

        // Za³aduj odpowiedni¹ scenê
        string sceneToLoad = "Map" + selectedMap;
        Debug.Log("£adowanie sceny: " + sceneToLoad);
        SceneManager.LoadScene(sceneToLoad);
    }
}