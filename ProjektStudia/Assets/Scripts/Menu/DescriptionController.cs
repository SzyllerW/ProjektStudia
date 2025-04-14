using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DescriptionController : MonoBehaviour
{
    [Header("Character image spawned in scene")]
    [SerializeField] private Sprite[] characterSprites;
    [SerializeField] private Transform parentObject;
    private Image spawnedImage;
    
    [Header("UI for Descriptions")]
    [SerializeField] private GameObject descriptionPanel;
    [SerializeField] private TextMeshProUGUI descriptionText;
    [SerializeField] private string[] characterDescriptions;
    [SerializeField] private RectTransform contentTransform;
    [SerializeField] private AudioClip buttonSound;

    [Header("Infographic System")]
    [SerializeField] private Image characterInfographic;
    [SerializeField] private Sprite[] characterInfographicSprites;

    [Header("Avaible Characters")]
    public int characterCount;

    private int characterIndex;

    void Start()
    {
        characterCount -= 1;
        ShowDescription(0);
        SpawnImage(0);
    }
    
    public void IncrementationIndex()
    {
        SoundFXManager.instance.PlaySoundFXClip(buttonSound, transform, 1f);

        if (characterIndex == characterCount)
        {
            characterIndex = 0;
        }
        else
        {
            characterIndex +=1;
        }

        ShowDescription(characterIndex);
        SpawnImage(characterIndex);
    }

    public void DecrementationIndex()
    {
        SoundFXManager.instance.PlaySoundFXClip(buttonSound, transform, 1f);

        if (characterIndex == 0)
        {
            characterIndex = characterCount;
        }
        else
        {
            characterIndex -=1;
        }

        ShowDescription(characterIndex);
        SpawnImage(characterIndex);
    }

    private void SpawnImage(int characterIndex)
    {
        // Upewnij siê, ¿e sprite zosta³ poprawnie przypisany
        if (characterSprites == null || characterIndex < 0 || characterIndex >= characterSprites.Length)
        {
            Debug.LogError("Nieprawid³owy indeks lub sprite postaci nie jest przypisany!");
            return;
        }

        // Usuñ poprzedni¹ postaæ, jeœli istnieje
        if (spawnedImage != null)
        {
            Destroy(spawnedImage.gameObject);
        }

        // Utwórz nowy obiekt na scenie
        GameObject newImageObject = new GameObject("SpawnedImage");

        // Przypisz pozycjê w hierarchii sceny
        if (parentObject != null)
        {
            newImageObject.transform.SetParent(parentObject, false);
        }

        // Dodaj komponent Image i popierz sprite z listy
        spawnedImage = newImageObject.AddComponent<Image>();
        spawnedImage.sprite = characterSprites[characterIndex];

        // Zmieñ wymiary obiektu
        RectTransform rectTransform = newImageObject.GetComponent<RectTransform>();
        rectTransform.sizeDelta = new Vector2(694f, 694f);
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

            // Podzia³ opisu na nag³ówki
            string characterName = string.Empty;
            string characterClass = string.Empty;
            string descriptionBody = characterDescriptions[characterIndex];

            if (characterIndex == 0)
            {
                characterName = "¯ABA";
                characterClass = "kaskader";
            }
            else if (characterIndex == 1)
            {
                characterName = "PINGWIN";
                characterClass = "pomocnik";
            }
            else if (characterIndex == 2)
            {
                characterName = "KRET";
                characterClass = "inicjator";
            }

            // Zmiana formatowania nag³ówków
            string formattedHeader = $"<size=150%>{characterName} (klasa: <color=#FF8F52>{characterClass}</color>)</size>\n";

            // Aktualizacja opisu
            descriptionText.text = formattedHeader + descriptionBody;

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
                float contentHeight = contentTransform.sizeDelta.y;
                float viewportHeight = contentTransform.parent.GetComponent<RectTransform>().rect.height;

                if (contentHeight > viewportHeight)
                {
                    scrollRect.verticalNormalizedPosition = 1f;
                }
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
}
