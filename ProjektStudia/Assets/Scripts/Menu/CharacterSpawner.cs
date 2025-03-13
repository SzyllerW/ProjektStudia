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
        // Upewnij si�, �e prefab zosta� poprawnie przypisany
        if (characterPrefabs == null || characterIndex < 0 || characterIndex >= characterPrefabs.Length)
        {
            Debug.LogError("Nieprawid�owy indeks lub prefaby postaci nie s� przypisane!");
            return;
        }

        // Sprawd�, czy posta� o danym indeksie ju� istnieje
        if (spawnedCharacters[characterIndex] != null)
        {
            Debug.Log($"Posta� o indeksie {characterIndex} ju� istnieje na scenie!");
            return; // Nie r�b nic, je�li posta� ju� jest na scenie

            // Wy�wietl opis tej postaci
            ShowDescription(characterIndex);
            return;
        }

        // Usu� poprzedni� posta�, je�li istnieje
        if (spawnedCharacter != null)
        {
            Destroy(spawnedCharacter);
        }

        // Spawnowanie nowej postaci
        spawnedCharacter = Instantiate(characterPrefabs[characterIndex], spawnPoint.position, Quaternion.identity);

        // Zapisz t� posta� w tablicy
        spawnedCharacters[characterIndex] = spawnedCharacter;

        // Wy�wietl opis postaci
        ShowDescription(characterIndex);

        // Zmiana pozycji postaci wzgl�dem indywidualnej korekty
        if (customOffsets != null && characterIndex < customOffsets.Length)
        {
            spawnedCharacter.transform.position += customOffsets[characterIndex];
        }
        else
        {
            Debug.LogWarning($"Brak korekty pozycji dla postaci o indeksie {characterIndex}");
        }

        // Powi�kszenie postaci
        spawnedCharacter.transform.localScale *= 6;

        // Wy��czenie skryptu PlayerMovement oraz PlayerDeath
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
            // W��cz obiekt opis, je�li jest wy��czony
            if (!descriptionPanel.activeSelf)
            {
                descriptionPanel.SetActive(true);
            }

            // Aktualizacja opisu
            descriptionText.text = characterDescriptions[characterIndex];

            // Wymuszenie aktualizacji
            Canvas.ForceUpdateCanvases();

            // Dynamiczne dostosowanie wysoko�ci Content
            if (contentTransform != null)
            {
                float textHeight = descriptionText.preferredHeight;
                float viewportHeight = contentTransform.parent.GetComponent<RectTransform>().rect.height;
                contentTransform.sizeDelta = new Vector2(contentTransform.sizeDelta.x, Mathf.Max(textHeight, viewportHeight));
            }

            // Reset pozycji przewijania na g�r�
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
            Debug.LogWarning($"Nie mo�na wy�wietli� opisu dla postaci {characterIndex}. Brak danych lub komponent�w.");
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