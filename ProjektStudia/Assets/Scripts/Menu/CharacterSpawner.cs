using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CharacterSpawner : MonoBehaviour
{
    [Header("Character spawned in scene")]
    public GameObject[] characterPrefabs;
    public Transform spawnPoint;
    public Vector3[] customOffsets;
    private GameObject spawnedCharacter;
    private GameObject[] spawnedCharacters;

    [Header("UI for Descriptions")]
    public GameObject descriptionPanel;
    public TextMeshProUGUI descriptionText;
    public string[] characterDescriptions;
    public RectTransform contentTransform;

    [Header("Infographic System")]
    public Image characterInfographic;
    public Sprite[] characterInfographicSprites;

    private void Start()
    {
        // Inicjalizacja tablicy dla przechowywania postaci
        spawnedCharacters = new GameObject[characterPrefabs.Length];

        // Ukryj panel opisu na start
        if (descriptionPanel != null)
        {
            descriptionPanel.SetActive(false);
        }
    }

    public void SpawnCharacter(int characterIndex)
    {
        // Upewnij siê, ¿e prefab zosta³ poprawnie przypisany
        if (characterPrefabs == null || characterIndex < 0 || characterIndex >= characterPrefabs.Length)
        {
            Debug.LogError("Nieprawid³owy indeks lub prefaby postaci nie s¹ przypisane!");
            return;
        }

        // SprawdŸ, czy postaæ o danym indeksie ju¿ istnieje
        if (spawnedCharacters[characterIndex] != null)
        {
            Debug.Log($"Postaæ o indeksie {characterIndex} ju¿ istnieje na scenie!");
            return; // Nie rób nic, jeœli postaæ ju¿ jest na scenie

            // Wyœwietl opis tej postaci
            ShowDescription(characterIndex);
            return;
        }

        // Usuñ poprzedni¹ postaæ, jeœli istnieje
        if (spawnedCharacter != null)
        {
            Destroy(spawnedCharacter);
        }

        // Spawnowanie nowej postaci
        spawnedCharacter = Instantiate(characterPrefabs[characterIndex], spawnPoint.position, Quaternion.identity);

        // Zapisz tê postaæ w tablicy
        spawnedCharacters[characterIndex] = spawnedCharacter;

        // Wyœwietl opis postaci
        ShowDescription(characterIndex);

        // Zmiana pozycji postaci wzglêdem indywidualnej korekty
        if (customOffsets != null && characterIndex < customOffsets.Length)
        {
            spawnedCharacter.transform.position += customOffsets[characterIndex];
        }
        else
        {
            Debug.LogWarning($"Brak korekty pozycji dla postaci o indeksie {characterIndex}");
        }

        // Powiêkszenie postaci
        spawnedCharacter.transform.localScale *= 6;

        // Wy³¹czenie skryptu PlayerMovement oraz PlayerDeath
        var playerMovementScript = spawnedCharacter.GetComponent<PlayerMovement>();
        if (playerMovementScript != null)
        {
            playerMovementScript.enabled = false;
        }

        var playerDeathScript = spawnedCharacter.GetComponent<PlayerDeath>();
        if (playerDeathScript != null)
        {
            playerDeathScript.enabled = false;
        }
    }

    private void ShowDescription(int characterIndex)
    {
        if (descriptionPanel != null && descriptionText != null && characterDescriptions.Length > characterIndex)
        {
            // W³¹cz obiekt opis, jeœli jest wy³¹czony
            if (!descriptionPanel.activeSelf)
            {
                descriptionPanel.SetActive(true);
            }

            // Aktualizacja opisu
            descriptionText.text = characterDescriptions[characterIndex];

            // Wymuszenie aktualizacji
            Canvas.ForceUpdateCanvases();

            // Dynamiczne dostosowanie wysokoœci Content
            if (contentTransform != null)
            {
                float textHeight = descriptionText.preferredHeight;
                float viewportHeight = contentTransform.parent.GetComponent<RectTransform>().rect.height;
                contentTransform.sizeDelta = new Vector2(contentTransform.sizeDelta.x, Mathf.Max(textHeight, viewportHeight));
            }

            // Reset pozycji przewijania na górê
            ScrollRect scrollRect = contentTransform.parent.parent.GetComponent<ScrollRect>();
            if (scrollRect != null)
            {
                scrollRect.verticalNormalizedPosition = 1f;
            }

            // Aktualizacja infografiki
            if (characterInfographic != null && characterInfographicSprites.Length > characterIndex)
            {
                characterInfographic.sprite = characterInfographicSprites[characterIndex];
            }
            else
            {
                Debug.LogWarning($"Nie przypisano infografiki dla postaci o indeksie {characterIndex}!");
            }
        }
        else
        {
            Debug.LogWarning($"Nie mo¿na wyœwietliæ opisu dla postaci {characterIndex}. Brak danych lub komponentów.");
        }
    }

    public void HideDescription()
    {
        if (descriptionPanel != null)
        {
            descriptionPanel.SetActive(false); // Ukrycie panelu
        }
    }
}