using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class CharacterSelectionManager : MonoBehaviour
{
    public List<Button> characterButtons; 

    private List<int> selectedCharacters = new List<int>();
    private int selectedMap;
    private int maxSelectableCharacters;

    private void Start()
    {
        selectedMap = PlayerPrefs.GetInt("SelectedMap", 1);

        maxSelectableCharacters = (selectedMap == 1) ? 2 : characterButtons.Count;

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
                SceneManager.LoadScene("Map1");
            }
        }
    }
}
