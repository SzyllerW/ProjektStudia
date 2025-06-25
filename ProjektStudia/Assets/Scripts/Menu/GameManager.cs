using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public Transform respawnPoint;
    public CharacterDatabase characterDatabase;
    public Transform characterIconPanel;
    public GameObject characterIconPrefab;
    public List<Sprite> characterIcons;

    private List<CharacterIconButton> iconButtons = new List<CharacterIconButton>();
    private List<GameObject> activeCharacters = new List<GameObject>();
    private int currentCharacterIndex = -1;
    private bool canSelectCharacter = true;
    private bool tutorialCharacterSpawned = false;

    [SerializeField] private bool isTutorial = false;
    [SerializeField] private float switchDelay = 0.5f;
    [SerializeField] private float respawnCooldown = 1.0f;
    [SerializeField] private GameObject lossScreen;
    [SerializeField] private AudioClip lossAudioClip;
    [SerializeField] private float lossVolume = 0.1f;

    [Header("Warunki zwyciêstwa")]
    [SerializeField] private bool requiresAllCollectibles = false;
    [SerializeField] private bool requiresAcornDelivery = false;
    [SerializeField] private GameObject winScreen;
    [SerializeField] private AudioClip winAudioClip;
    [SerializeField] private float winVolume = 0.1f;

    private int totalCollectibles = 0;
    private int collectedCount = 0;
    private int requiredAcorns = 1;
    private int deliveredAcorns = 0;
    private int totalBerries = 0;
    private int collectedBerries = 0;

    [SerializeField] private LevelConfig levelConfig;

    private void Awake()
    {
        SaveSystem.Load();
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
        totalCollectibles = FindObjectsOfType<CollectibleItem>(true).Length;
    }

    private void Start()
    {
        Debug.Log("ODBLOKOWANE POSTACIE: " + string.Join(",", SaveSystem.GetUnlockedCharacters()));

        CollectibleItem.OnItemCollected += OnItemCollected;
        totalBerries = CountTotalBerries();

        if (characterDatabase == null || characterDatabase.allCharacters.Count == 0) return;

        if (!isTutorial) GenerateCharacterIcons();
        else SpawnTutorialCharacter();
    }

    private void OnDestroy()
    {
        CollectibleItem.OnItemCollected -= OnItemCollected;
    }

    private int CountTotalBerries()
    {
        int count = 0;
        foreach (var bush in FindObjectsOfType<BerryBush>())
        {
            count += bush.Berries.Count;
        }
        return count;
    }

    private void GenerateCharacterIcons()
    {
        foreach (Transform child in characterIconPanel)
        {
            Destroy(child.gameObject);
        }
        iconButtons.Clear();

        List<int> unlocked = SaveSystem.GetUnlockedCharacters();

        for (int i = 0; i < characterDatabase.allCharacters.Count; i++)
        {
            if (!unlocked.Contains(i)) continue;

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
        if (characterDatabase.allCharacters.Count == 0) return;

        GameObject tutorialChar = Instantiate(characterDatabase.allCharacters[0], respawnPoint.position, Quaternion.identity);
        tutorialChar.SetActive(true);
        activeCharacters.Add(tutorialChar);
        currentCharacterIndex = 0;
        StartCoroutine(PrepareAndActivate(tutorialChar));

        tutorialCharacterSpawned = true;
        canSelectCharacter = false;
    }

    public void SelectCharacter(int index)
    {
        if (!canSelectCharacter || index >= characterDatabase.allCharacters.Count) return;

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
            characterToActivate = Instantiate(characterDatabase.allCharacters[index], respawnPoint.position, Quaternion.identity);
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

        var movement = character.GetComponent<PlayerMovement>();
        if (movement != null)
        {
            movement.enabled = true;
        }

        var berryCollector = character.GetComponent<PlayerBerryCollector>();
        if (berryCollector != null)
        {
            berryCollector.ResetCollector();
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
        if (character != null)
        {
            PlayerCarry carry = character.GetComponent<PlayerCarry>();
            if (carry != null)
            {
                carry.DropAcornAtCheckpoint();
            }
            character.SetActive(false);
            activeCharacters.Remove(character);
        }

        if (isTutorial) StartCoroutine(RespawnTutorialCharacterProperly());
        else StartCoroutine(FinalDeathCheck());
    }

    private IEnumerator RespawnTutorialCharacterProperly()
    {
        yield return new WaitForSeconds(respawnCooldown);

        if (activeCharacters.Count > 0)
        {
            Destroy(activeCharacters[0]);
            activeCharacters.Clear();
        }

        GameObject newChar = Instantiate(characterDatabase.allCharacters[0], respawnPoint.position + Vector3.up * 1.2f, Quaternion.identity);
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

    private IEnumerator FinalDeathCheck()
    {
        yield return new WaitForEndOfFrame();
        UnlockCharacterSelection();
        CheckIfNoCharactersLeft();
    }

    public void UnlockCharacterSelection()
    {
        canSelectCharacter = true;
    }

    public void OnItemCollected()
    {
        collectedCount++;
        CheckWinCondition();
    }

    public void AcornDelivered()
    {
        deliveredAcorns++;
        CheckWinCondition();
    }

    public void BerryCollected()
    {
        collectedBerries++;
        CheckWinCondition();
    }

    public void CheckIfNoCharactersLeft()
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

        bool anyCharacterActive = false;
        foreach (var character in activeCharacters)
        {
            if (character != null && character.activeSelf)
            {
                anyCharacterActive = true;
                break;
            }
        }

        if (!anyInteractable && !anyCharacterActive)
        {
            TriggerGameOver();
        }
        else if (anyInteractable && !anyCharacterActive)
        {
            UnlockCharacterSelection();
        }
    }

    private void TriggerGameOver()
    {
        StartCoroutine(GameOverSequence());
    }

    private IEnumerator GameOverSequence()
    {
        /*CameraFollow cameraFollow = Camera.main.GetComponent<CameraFollow>();
        if (cameraFollow != null)
        {
            cameraFollow.ShakeBeforeFollow(1f, 8f);
        }*/

        yield return new WaitForSeconds(2f);
        lossScreen.SetActive(true);
        SoundFXManager.instance.PlaySoundFXClip(lossAudioClip, transform, lossVolume);
    }

    private void CheckWinCondition()
    {
        bool allItemsCollected = !requiresAllCollectibles || collectedCount >= totalCollectibles;
        bool allAcornsDelivered = !requiresAcornDelivery || deliveredAcorns >= requiredAcorns;
        bool allBerriesCollected = collectedBerries >= totalBerries;

        if (allItemsCollected && allAcornsDelivered && allBerriesCollected)
        {
            StartCoroutine(WinSequence());
        }
    }

    private IEnumerator WinSequence()
    {
        yield return new WaitForSeconds(0.5f);
        winScreen.SetActive(true);
        SoundFXManager.instance.PlaySoundFXClip(winAudioClip, transform, winVolume);
    }
}





