using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;
using UnityEngine.Video;

public class DescriptionController : MonoBehaviour
{
    [Header("Character image spawned in scene")]
    [SerializeField] private Sprite[] characterSprites;
    [SerializeField] private Transform parentObject;
    private Image spawnedImage;
    
    [Header("UI for Descriptions")]
    [SerializeField] private GameObject descriptionPanel;
    [SerializeField] private TextMeshProUGUI descriptionText;
    [SerializeField] private RectTransform contentTransform;
    [SerializeField] private AudioClip buttonSound;

    [Header("Video Tutorials")]
    [SerializeField] private VideoPlayer videoPlayer;
    [SerializeField] private VideoClip[] videoClips;

    [Header("Localization")]
    [SerializeField] private LocalizedString characterNameLocalized;
    [SerializeField] private LocalizedString characterClassLocalized;
    [SerializeField] private LocalizedString characterDescriptionLocalized;

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
        characterIndex = (characterIndex + 1) % (characterCount + 1);
        ShowDescription(characterIndex);
        SpawnImage(characterIndex);
    }

    public void DecrementationIndex()
    {
        SoundFXManager.instance.PlaySoundFXClip(buttonSound, transform, 1f);
        characterIndex = (characterIndex - 1 + (characterCount + 1)) % (characterCount + 1);
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

        // Stwórz now¹ postaæ
        spawnedImage = Instantiate(new GameObject("SpawnedImage"), parentObject).AddComponent<Image>();
        spawnedImage.sprite = characterSprites[characterIndex];

        // Zmieñ po³o¿enie i wymiary postaci
        RectTransform rectTransform = spawnedImage.GetComponent<RectTransform>();
        rectTransform.sizeDelta = new Vector2(694f, 694f);
    }

    private void ShowDescription(int characterIndex)
    {
        if (descriptionPanel != null && descriptionText != null)
        {
            descriptionPanel.SetActive(true);

            // Ustawienie kluczy dla lokalizacji
            characterNameLocalized.SetReference("CharacterNamesTable", $"character_{characterIndex}_name");
            characterClassLocalized.SetReference("CharacterClassesTable", $"character_{characterIndex}_class");
            characterDescriptionLocalized.SetReference("CharacterDescriptionsTable", $"character_{characterIndex}_description");

            characterNameLocalized.StringChanged += (name) => UpdateHeader(name, characterClassLocalized.GetLocalizedString());
            characterClassLocalized.StringChanged += (className) => UpdateHeader(characterNameLocalized.GetLocalizedString(), className);
            characterDescriptionLocalized.StringChanged += (desc) => UpdateDescription(desc);
        }
    }

    private void UpdateHeader(string characterName, string characterClass)
    {
        string formattedHeader = $"<size=150%>{characterName} <color=#FF8F52>({characterClass})</color></size>\n";
        descriptionText.text = formattedHeader + descriptionText.text;
    }

    private void UpdateDescription(string descriptionBody)
    {
        descriptionText.text = "";
        string name = characterNameLocalized.GetLocalizedString();
        string className = characterClassLocalized.GetLocalizedString();
        descriptionText.text = $"<size=150%>{name} <color=#FF8F52>({className})</color></size>\n{descriptionBody}";

        Canvas.ForceUpdateCanvases();

        float textHeight = descriptionText.preferredHeight;
        float viewportHeight = contentTransform.parent.GetComponent<RectTransform>().rect.height;
        contentTransform.sizeDelta = new Vector2(contentTransform.sizeDelta.x, Mathf.Max(textHeight, viewportHeight));

        ScrollRect scrollRect = contentTransform.parent.parent.GetComponent<ScrollRect>();
        if (scrollRect != null)
        {
            float contentHeight = contentTransform.sizeDelta.y;
            float viewportHeightAdjusted = contentTransform.parent.GetComponent<RectTransform>().rect.height;

            if (contentHeight > viewportHeightAdjusted)
            {
                scrollRect.verticalNormalizedPosition = 1f;
            }
        }

        if (videoPlayer != null && videoClips.Length > characterIndex)
        {
            videoPlayer.clip = videoClips[characterIndex];
            videoPlayer.Play();
        }
        else
        {
            Debug.LogWarning($"Nie przypisano nagranego tutorialu dla postaci o indeksie {characterIndex}!");
        }
    }
}
