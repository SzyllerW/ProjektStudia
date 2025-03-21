using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public Transform respawnPoint;
    public Image[] iconSlots;
    public List<GameObject> characterPrefabs;

    private List<GameObject> activeCharacters = new List<GameObject>();
    private int currentCharacterIndex = 0;
    private int usedCharactersCount = 0;

    [SerializeField] private bool isTutorial = false;
    [SerializeField] private float switchDelay = 0.5f;
    [SerializeField] private float respawnCooldown = 1.0f; 
    [SerializeField] private string gameOverSceneName = "GameOverScene"; 

    private void Start()
    {
        Debug.Log($"[LevelManager] Character prefabs count: {characterPrefabs.Count}");
        if (characterPrefabs.Count == 0)
        {
            Debug.LogError("[LevelManager] No prefabs assigned in the inspector.");
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
            Debug.LogError("[LevelManager] No characters loaded!");
        }
    }

    private void LoadSelectedCharacters()
    {
        string selectedCharacters = PlayerPrefs.GetString("SelectedCharacters", "");
        Debug.Log($"[LevelManager] Loaded Selected Characters: '{selectedCharacters}'");

        if (string.IsNullOrWhiteSpace(selectedCharacters))
        {
            Debug.LogError("[LevelManager] No characters selected or data is empty.");
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
                Debug.Log($"[LevelManager] Instantiated character: {character.name}");
            }
            else
            {
                Debug.LogError($"[LevelManager] Invalid character index or prefab not found: {index}");
            }
        }
    }

    private void ActivateCharacter(int index)
    {
        if (index >= activeCharacters.Count)
        {
            Debug.LogError("[LevelManager] Invalid character index to activate.");
            return;
        }

        currentCharacterIndex = index;
        GameObject newCharacter = activeCharacters[currentCharacterIndex];
        ResetCharacter(newCharacter);
        newCharacter.transform.position = respawnPoint.position;
        newCharacter.SetActive(true);
        Debug.Log($"[LevelManager] Activated character: {newCharacter.name}");
    }

    public void SwitchToNextCharacter()
    {
        if (isTutorial)
        {
            ActivateCharacter(0);
            return;
        }

        if (activeCharacters.Count == 0) return;

        StartCoroutine(SwitchCharacterWithDelay());
    }

    private System.Collections.IEnumerator SwitchCharacterWithDelay()
    {
        GameObject currentCharacter = activeCharacters[currentCharacterIndex];
        currentCharacter.SetActive(false);
        usedCharactersCount++;

        yield return new WaitForSeconds(switchDelay);

        if (usedCharactersCount >= activeCharacters.Count)
        {
            TriggerGameOver();
        }
        else
        {
            ActivateCharacter((currentCharacterIndex + 1) % activeCharacters.Count);
            UpdateIcons();

            yield return new WaitForSeconds(respawnCooldown);
        }
    }

    private void UpdateIcons()
    {
        for (int i = 0; i < iconSlots.Length; i++)
        {
            if (i < activeCharacters.Count)
            {
                if (i == currentCharacterIndex)
                {
                    iconSlots[i].color = Color.white;
                }
                else
                {
                    iconSlots[i].color = Color.gray; 
                }
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

    public void CharacterFellOffMap(GameObject character)
    {
        character.SetActive(false);
        usedCharactersCount++;

        if (usedCharactersCount >= activeCharacters.Count)
        {
            TriggerGameOver();
        }
        else
        {
            SwitchToNextCharacter();
        }
    }

    private void TriggerGameOver()
    {
        Debug.Log("[LevelManager] Game Over! Switching to game over scene.");
        SceneManager.LoadScene(gameOverSceneName);
    }
}

