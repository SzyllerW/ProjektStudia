using System.Collections;
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
        if (respawnPoint == null)
        {
            Debug.LogError("[GameManager] respawnPoint is NULL! Postacie bêd¹ siê pojawiaæ w miejscu starej pozycji.");
        }
        else
        {
            Debug.Log(" [GameManager] Respawn point set to: " + respawnPoint.position);
        }

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
            Debug.LogError("[GameManager] Invalid character index to activate.");
            return;
        }

        currentCharacterIndex = index;
        GameObject newCharacter = activeCharacters[currentCharacterIndex];

        StartCoroutine(PrepareAndActivate(newCharacter));

    }

    private IEnumerator PrepareAndActivate(GameObject character)
    {
        // Aktywuj obiekt — ale jeszcze nie przesuwaj!
        character.SetActive(true);

        // Poczekaj 1 klatkê a¿ fizyka siê aktywuje
        yield return new WaitForFixedUpdate();

        // Dopiero TERAZ przesuñ postaæ!
        character.transform.position = respawnPoint.position + Vector3.up * 0.05f;

        // Resetuj fizykê i animacje
        ResetCharacter(character);

        // Poczekaj 1 frame na przeliczenie kolizji
        yield return null;

        var movement = character.GetComponent<PlayerMovement>();
        if (movement != null)
        {
            movement.ResetAfterRespawn();
        }

        Debug.Log("[GameManager] Final respawn at: " + character.transform.position);

        transform.SetParent(null);
        transform.position = respawnPoint.position + Vector3.up * 0.05f;
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

    private IEnumerator SwitchCharacterWithDelay()
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
            rb.isKinematic = false;
            rb.simulated = true;
            rb.gravityScale = 5f;
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

