using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class CharacterSelectionManager : MonoBehaviour
{
    public List<Button> characterButtons;
    public List<Sprite> characterIcons;

    private List<int> selectedCharacters = new List<int>();
    private int maxSelectableCharacters;
    private int selectedMap; // Przeniesiono selectedMap na poziom klasy

    private void Start()
    {
        selectedMap = PlayerPrefs.GetInt("SelectedMap", 1); // Pobierz wybran¹ mapê z PlayerPrefs
        maxSelectableCharacters = (selectedMap == 1) ? 2 : 5; // Ustaw maksymaln¹ liczbê postaci

        // Dodaj listenery do przycisków
        for (int i = 0; i < characterButtons.Count; i++)
        {
            int index = i;
            characterButtons[i].onClick.AddListener(() => SelectCharacter(index));
        }
    }

    public void SelectCharacter(int characterIndex)
    {
        if (!selectedCharacters.Contains(characterIndex) && selectedCharacters.Count < maxSelectableCharacters)
        {
            selectedCharacters.Add(characterIndex);
            characterButtons[characterIndex].interactable = false;

            if (selectedCharacters.Count == maxSelectableCharacters)
            {
                PlayerPrefs.SetString("SelectedCharacters", string.Join(",", selectedCharacters));

                List<string> selectedIcons = new List<string>
                {
                    characterIcons[selectedCharacters[0]].name,
                    characterIcons[selectedCharacters[1]].name
                };

                for (int i = 2; i < selectedCharacters.Count; i++)
                {
                    selectedIcons.Add(characterIcons[selectedCharacters[i]].name);
                }

                PlayerPrefs.SetString("SelectedIcons", string.Join(",", selectedIcons));

                SceneManager.LoadScene("Map" + selectedMap); // Tutaj zmienna selectedMap jest widoczna
            }
        }
    }
}