using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    public Transform respawnPoint; // Punkt respawnu postaci
    public Image[] iconSlots;     // Ikony w UI (lewym górnym rogu)
    public List<GameObject> characterPrefabs; // Prefaby postaci

    private List<GameObject> activeCharacters = new List<GameObject>();
    private int currentCharacterIndex = 0;

    private void Start()
    {
        LoadSelectedCharacters();
        ActivateCharacter(0);
        UpdateIcons();
    }

    private void LoadSelectedCharacters()
    {
        string selectedCharacters = PlayerPrefs.GetString("SelectedCharacters", "");
        string[] characterIndexes = selectedCharacters.Split(',');

        foreach (string index in characterIndexes)
        {
            int characterIndex = int.Parse(index);
            GameObject character = Instantiate(characterPrefabs[characterIndex], respawnPoint.position, Quaternion.identity);
            character.SetActive(false);
            activeCharacters.Add(character);
        }
    }

    private void ActivateCharacter(int index)
    {
        if (index >= activeCharacters.Count)
        {
            Debug.Log("Brak wiêcej postaci! Koniec gry.");
            return; // Koniec gry
        }

        currentCharacterIndex = index;
        activeCharacters[currentCharacterIndex].transform.position = respawnPoint.position;
        activeCharacters[currentCharacterIndex].SetActive(true);
    }

    public void SwitchToNextCharacter()
    {
        activeCharacters[currentCharacterIndex].SetActive(false);
        ActivateCharacter(currentCharacterIndex + 1);
        UpdateIcons();
    }

    private void UpdateIcons()
    {
        for (int i = 0; i < iconSlots.Length; i++)
        {
            if (i < activeCharacters.Count)
            {
                iconSlots[i].color = (i == currentCharacterIndex) ? Color.white : Color.gray; // Aktywna postaæ na bia³o
            }
            else
            {
                iconSlots[i].color = Color.clear; // Ukryj puste sloty
            }
        }
    }
}