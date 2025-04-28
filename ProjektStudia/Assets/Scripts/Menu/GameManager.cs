using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public Transform respawnPoint;
    public List<GameObject> characterPrefabs;
    public Transform characterIconPanel;
    public GameObject characterIconPrefab;
    private List<CharacterIconButton> iconButtons = new List<CharacterIconButton>();
    [SerializeField] private List<Sprite> characterIcons;

    private List<GameObject> activeCharacters = new List<GameObject>();
    private int currentCharacterIndex = -1;

    private bool canSelectCharacter = true;
    private bool tutorialCharacterSpawned = false;

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
    }

    private void Start()
    {
        PlayerPrefs.DeleteAll();
        CollectibleItem.OnItemCollected += OnItemCollected;

        if (characterPrefabs.Count == 0)
            return;

        if (!isTutorial)
        {
            string selectedCharacters = PlayerPrefs.GetString("SelectedCharacters", "");

            if (!string.IsNullOrWhiteSpace(selectedCharacters))
            {
                // JEŒLI MAMY WYBRANE POSTACIE WCZEŒNIEJ
                GenerateCharacterIcons();
            }
            else
            {
                // JEŒLI NIE MAMY WYBRANYCH POSTACI
                GenerateCharacterIcons();
            }
        }
        else
        {
            SpawnTutorialCharacter();
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

    private void GenerateCharacterIcons()
    {
        foreach (Transform child in characterIconPanel)
        {
            Destroy(child.gameObject);
        }

        iconButtons.Clear();

        for (int i = 0; i < characterPrefabs.Count; i++)
        {
            GameObject newIcon = Instantiate(characterIconPrefab, characterIconPanel);
            CharacterIconButton iconButton = newIcon.GetComponent<CharacterIconButton>();
            if (iconButton != null)
            {
                iconButton.Setup(i);
                iconButtons.Add(iconButton);
            }

            Image img = newIcon.GetComponent<Image>();
            if (img != null && i < characterIcons.Count)
            {
                img.sprite = characterIcons[i];
            }
        }
    }

    private void SpawnTutorialCharacter()
    {
        if (characterPrefabs.Count == 0)
            return;

        GameObject tutorialChar = Instantiate(characterPrefabs[0], respawnPoint.position, Quaternion.identity);
        tutorialChar.SetActive(true);
        activeCharacters.Add(tutorialChar);
        currentCharacterIndex = 0;
        StartCoroutine(PrepareAndActivate(tutorialChar));

        tutorialCharacterSpawned = true;
        canSelectCharacter = false;
    }

    public void SelectCharacter(int index)
    {
        if (!canSelectCharacter)
            return;

        if (index >= characterPrefabs.Count)
            return;

        if (currentCharacterIndex >= 0 && currentCharacterIndex < activeCharacters.Count)
        {
            activeCharacters[currentCharacterIndex].SetActive(false);
        }

        GameObject characterToActivate = null;
        if (index < activeCharacters.Count)
        {
            characterToActivate = activeCharacters[index];
        }

        if (characterToActivate == null)
        {
            characterToActivate = Instantiate(characterPrefabs[index], respawnPoint.position, Quaternion.identity);
            activeCharacters.Add(characterToActivate);
        }

        currentCharacterIndex = index;
        StartCoroutine(PrepareAndActivate(characterToActivate));

        canSelectCharacter = false;

        if (index >= 0 && index < iconButtons.Count)
        {
            iconButtons[index].GetComponent<Button>().interactable = false;
        }
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
    }

    private void CheckIfNoCharactersLeft()
    {
        bool anyInteractable = false;

        foreach (var button in iconButtons)
        {
            if (button != null && button.GetComponent<Button>().interactable)
            {
                anyInteractable = true;
                break;
            }
        }

        if (!anyInteractable)
        {
            TriggerGameOver();
        }
        else
        {
            UnlockCharacterSelection();
        }
    }

    public void UnlockCharacterSelection()
    {
        canSelectCharacter = true;
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
        if (character != null)
        {
            PlayerCarry carry = character.GetComponent<PlayerCarry>();
            if (carry != null)
            {
                carry.DropAcornAtCheckpoint();
            }

            character.SetActive(false);
        }

        if (isTutorial)
        {
            StartCoroutine(RespawnTutorialCharacterProperly());
        }
        else
        {
            CheckIfNoCharactersLeft(); 
        }
    }

    private IEnumerator RespawnTutorialCharacterProperly()
    {
        yield return new WaitForSeconds(respawnCooldown);

        if (activeCharacters.Count > 0)
        {
            Destroy(activeCharacters[0]);
            activeCharacters.Clear();
        }

        GameObject newChar = Instantiate(characterPrefabs[0], respawnPoint.position + Vector3.up * 1.2f, Quaternion.identity);
        newChar.SetActive(true);
        activeCharacters.Add(newChar);
        currentCharacterIndex = 0;
        StartCoroutine(PrepareAndActivate(newChar));

        var death = newChar.GetComponent<PlayerDeath>();
        if (death != null)
        {
            death.ResetDeath();
        }
    }

    private void TriggerGameOver()
    {
        SceneManager.LoadScene(gameOverSceneName);
    }

    private void OnItemCollected()
    {
        collectedCount++;
        CheckWinCondition();
    }

    public void AcornDelivered()
    {
        deliveredAcorns++;
        CheckWinCondition();
    }

    private void CheckWinCondition()
    {
        bool allItemsCollected = !requiresAllCollectibles || collectedCount >= totalCollectibles;
        bool allAcornsDelivered = !requiresAcornDelivery || deliveredAcorns >= requiredAcorns;

        if (!allItemsCollected || !allAcornsDelivered)
            return;

        SceneManager.LoadScene(levelCompleteSceneName);
    }
}

