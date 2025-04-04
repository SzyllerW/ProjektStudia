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

    [Header("Warunki zwyciêstwa")]
    [SerializeField] private int totalCollectibles = 0;
    private int collectedCount = 0;

    [SerializeField] private bool requiresAllCollectibles = false;
    [SerializeField] private bool requiresAcornDelivery = false;
    [SerializeField] private string levelCompleteSceneName = "LevelComplete";

    [Header("Acorn Delivery")]
    [SerializeField] private int requiredAcorns = 1;
    private int deliveredAcorns = 0;

    private void Awake()
    {
        var all = FindObjectsOfType<CollectibleItem>(true); 
        totalCollectibles = all.Length;

        Debug.Log("[GameManager] ZNALAZ£EM {totalCollectibles} CollectibleItemów:");
        foreach (var item in all)
        {
            Debug.Log("  " + item.gameObject.name + " (Parent: " + item.transform.parent?.name + ")");
        }

    }

    private void Start()
    {
        CollectibleItem.OnItemCollected += OnItemCollected;

        if (characterPrefabs.Count == 0)
            return;

        LoadSelectedCharacters();

        if (activeCharacters.Count > 0)
        {
            ActivateCharacter(0);
            UpdateIcons();
        }
    }

    private void OnDestroy()
    {
        CollectibleItem.OnItemCollected -= OnItemCollected;
    }

    private void LoadSelectedCharacters()
    {
        string selectedCharacters = PlayerPrefs.GetString("SelectedCharacters", "");
        if (string.IsNullOrWhiteSpace(selectedCharacters))
            return;

        string[] characterIndexes = selectedCharacters.Split(',');

        foreach (string index in characterIndexes)
        {
            if (int.TryParse(index, out int characterIndex) && characterIndex < characterPrefabs.Count)
            {
                GameObject character = Instantiate(characterPrefabs[characterIndex], respawnPoint.position, Quaternion.identity);
                character.SetActive(false);
                activeCharacters.Add(character);
            }
        }
    }

    private void ActivateCharacter(int index)
    {
        if (index >= activeCharacters.Count)
            return;

        currentCharacterIndex = index;
        GameObject newCharacter = activeCharacters[currentCharacterIndex];
        StartCoroutine(PrepareAndActivate(newCharacter));
    }

    private IEnumerator PrepareAndActivate(GameObject character)
    {
        character.SetActive(true);
        yield return new WaitForFixedUpdate();

        character.transform.position = respawnPoint.position + Vector3.up * 0.05f;
        ResetCharacter(character);

        yield return null;

        var movement = character.GetComponent<PlayerMovement>();
        if (movement != null)
        {
            movement.enabled = true;
            movement.ResetAfterRespawn();
        }

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

        var movement = character.GetComponent<PlayerMovement>();
        if (movement != null)
        {
            movement.enabled = true;
        }
    }

    public void CharacterFellOffMap(GameObject character)
    {
        PlayerCarry carry = character.GetComponent<PlayerCarry>();
        if (carry != null)
        {
            carry.DropAcornAtCheckpoint();
        }

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
        SceneManager.LoadScene(gameOverSceneName);
    }

    // --- ZBIERANIE ITEMÓW ---

    private void OnItemCollected()
    {
        collectedCount++;
        CheckWinCondition();
    }

    // --- DOSTARCZANIE ¯O£ÊDZI ---

    public void AcornDelivered()
    {
        deliveredAcorns++;
        CheckWinCondition();
    }

    private void CheckWinCondition()
    {
        bool allItemsCollected = !requiresAllCollectibles || collectedCount >= totalCollectibles;
        bool allAcornsDelivered = !requiresAcornDelivery || deliveredAcorns >= requiredAcorns;

        Debug.Log($"[CheckWinCondition] Zebrane: {collectedCount}/{totalCollectibles}, ¯o³êdzie: {deliveredAcorns}/{requiredAcorns}");

        if (requiresAllCollectibles && collectedCount < totalCollectibles)
        {
            return;
        }

        if (requiresAcornDelivery && deliveredAcorns < requiredAcorns)
        {
            return; 
        }

        Debug.Log("[CheckWinCondition] WARUNKI SPE£NIONE – ³adowanie sceny zwyciêstwa");
        SceneManager.LoadScene(levelCompleteSceneName);

    }
}

