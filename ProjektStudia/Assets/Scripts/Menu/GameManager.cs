using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public Transform respawnPoint;
    public Image[] iconSlots;
    public List<GameObject> characterPrefabs;

    private List<GameObject> activeCharacters = new List<GameObject>();
    private int currentCharacterIndex = 0;

    [SerializeField] private float switchDelay = 0.5f;

    private void Start()
    {
        Debug.Log($"[GameManager] Character prefabs count: {characterPrefabs.Count}");
        if (characterPrefabs.Count == 0)
        {
            Debug.LogError("[GameManager] No prefabs assigned in the inspector.");
            return;
        }

        LoadSelectedCharacters();

        if (activeCharacters.Count > 0)
        {
            ActivateCharacter(0);
            UpdateIcons();
        }
        else
        {
            Debug.LogError("[GameManager] No characters loaded!");
        }
    }

    private void LoadSelectedCharacters()
    {
        string selectedCharacters = PlayerPrefs.GetString("SelectedCharacters", "");
        Debug.Log($"[GameManager] Loaded Selected Characters: '{selectedCharacters}'");

        if (string.IsNullOrWhiteSpace(selectedCharacters))
        {
            Debug.LogError("[GameManager] No characters selected or data is empty.");
            return;
        }

        string[] characterIndexes = selectedCharacters.Split(',');

        foreach (string index in characterIndexes)
        {
            if (int.TryParse(index, out int characterIndex) && characterIndex < characterPrefabs.Count)
            {
                GameObject character = Instantiate(characterPrefabs[characterIndex], respawnPoint.position, Quaternion.identity);
                character.SetActive(false);
                activeCharacters.Add(character);
                Debug.Log($"[GameManager] Instantiated character: {character.name}");
            }
            else
            {
                Debug.LogError($"[GameManager] Invalid character index or prefab not found: {index}");
            }
        }
    }

    private void ActivateCharacter(int index)
    {
        if (index >= activeCharacters.Count)
        {
            Debug.LogError("[GameManager] Invalid character index to activate.");
            return;
        }

        currentCharacterIndex = index;
        GameObject newCharacter = activeCharacters[currentCharacterIndex];
        ResetCharacter(newCharacter);
        newCharacter.transform.position = respawnPoint.position;
        newCharacter.SetActive(true);
        Debug.Log($"[GameManager] Activated character: {newCharacter.name}");
    }

    public void SwitchToNextCharacter()
    {
        if (activeCharacters.Count == 0) return;

        StartCoroutine(SwitchCharacterWithDelay());
    }

    private System.Collections.IEnumerator SwitchCharacterWithDelay()
    {
        GameObject currentCharacter = activeCharacters[currentCharacterIndex];
        currentCharacter.SetActive(false);

        yield return new WaitForSeconds(switchDelay);

        ActivateCharacter((currentCharacterIndex + 1) % activeCharacters.Count);
        UpdateIcons();
    }

    private void UpdateIcons()
    {
        for (int i = 0; i < iconSlots.Length; i++)
        {
            if (i < activeCharacters.Count)
            {
                iconSlots[i].color = (i == currentCharacterIndex) ? Color.white : Color.gray;
            }
            else
            {
                iconSlots[i].color = Color.clear;
            }
        }
    }

    private void ResetCharacter(GameObject character)
    {
        Rigidbody2D rb = character.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.velocity = Vector2.zero;
        }

        Animator animator = character.GetComponent<Animator>();
        if (animator != null)
        {
            animator.SetFloat("Speed", 0f);
            animator.SetBool("IsJumping", false);
        }
    }
}
