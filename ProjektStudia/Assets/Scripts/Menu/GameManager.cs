using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public Transform respawnPoint;
    public List<GameObject> playableCharacters; 
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

    [Header("Warunki zwycięstwa")]
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

    private void Awake()
    {
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
        SaveSystem.Load(); // <- tu

        CollectibleItem.OnItemCollected += OnItemCollected;
        totalBerries = CountTotalBerries();

        if (playableCharacters == null || playableCharacters.Count == 0) return;

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

        for (int i = 0; i < playableCharacters.Count; i++)
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
        if (playableCharacters.Count == 0) return;

        GameObject tutorialChar = Instantiate(playableCharacters[0], respawnPoint.position, Quaternion.identity);
        tutorialChar.SetActive(true);
        activeCharacters.Add(tutorialChar);
        currentCharacterIndex = 0;
        StartCoroutine(PrepareAndActivate(tutorialChar));

        tutorialCharacterSpawned = true;
        canSelectCharacter = false;
    }

    public void SelectCharacter(int index)
    {
        if (!canSelectCharacter || index >= playableCharacters.Count) return;

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
            characterToActivate = Instantiate(playableCharacters[index], respawnPoint.position, Quaternion.identity);
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
        if (movement != null) movement.enabled = true;

        var berryCollector = character.GetComponent<PlayerBerryCollector>();
        if (berryCollector != null) berryCollector.ResetCollector();
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
            if (carry != null) carry.DropAcornAtCheckpoint();
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

        GameObject newChar = Instantiate(playableCharacters[0], respawnPoint.position + Vector3.up * 1.2f, Quaternion.identity);
        newChar.SetActive(true);
        activeCharacters.Add(newChar);
        currentCharacterIndex = 0;
        StartCoroutine(PrepareAndActivate(newChar));

        var death = newChar.GetComponent<PlayerDeath>();
        if (death != null) death.ResetDeath();
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
        yield return new WaitForSeconds(2f);
        lossScreen?.SetActive(true);
        if (lossAudioClip != null)
            SoundFXManager.instance?.PlaySoundFXClip(lossAudioClip, transform, lossVolume);
    }

    private void CheckWinCondition()
    {
        Debug.Log("CheckWinCondition wywołany");

        bool allItemsCollected = !requiresAllCollectibles || collectedCount >= totalCollectibles;
        bool allAcornsDelivered = !requiresAcornDelivery || deliveredAcorns >= requiredAcorns;
        bool allBerriesCollected = collectedBerries >= totalBerries;

        Debug.Log($"Items: {collectedCount}/{totalCollectibles}, Acorns: {deliveredAcorns}/{requiredAcorns}, Berries: {collectedBerries}/{totalBerries}");

        if (allItemsCollected && allAcornsDelivered && allBerriesCollected)
        {
            Debug.Log("Spełniono warunki zwycięstwa — odblokowuję kolejny poziom");
            StartCoroutine(WinSequence());
        }
    }

    private IEnumerator WinSequence()
    {
        yield return new WaitForSeconds(0.5f);

        string currentScene = SceneManager.GetActiveScene().name;
        Debug.Log($"WinSequence dla sceny: {currentScene}");

        switch (currentScene)
        {
            case "FrogTutorial":
                SaveSystem.UnlockLevel("Map6");
                break;
            case "Map6":
                SaveSystem.UnlockLevel("PenguinTutorial");
                SaveSystem.UnlockCharacter(1); // pingwin
                break;
            case "PenguinTutorial":
                SaveSystem.UnlockLevel("Map2");
                break;
            case "Map2":
                SaveSystem.UnlockLevel("MoleTutorial");
                SaveSystem.UnlockCharacter(2); // kret
                break;
            case "MoleTutorial":
                SaveSystem.UnlockLevel("Map8");
                break;
            case "Map8":
                SaveSystem.UnlockLevel("Map3");
                break;
            case "Map3":
                SaveSystem.UnlockLevel("KittyTutorial");
                SaveSystem.UnlockCharacter(3); // kot
                break;
            case "KittyTutorial":
                SaveSystem.UnlockLevel("Map4");
                break;
            case "Map4":
                SaveSystem.UnlockLevel("Map5");
                break;
            case "Map5":
                SaveSystem.UnlockLevel("Map7");
                break;
            case "Map7":
                SaveSystem.UnlockLevel("Map1");
                break;
        }

        SaveSystem.Save();

        winScreen?.SetActive(true);
        if (winAudioClip != null)
            SoundFXManager.instance?.PlaySoundFXClip(winAudioClip, transform, winVolume);
    }

}





