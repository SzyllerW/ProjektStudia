using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;

public class CharacterSelectionManager : MonoBehaviour
{
    public List<Button> characterButtons;
    public List<Sprite> originalSprites;
    public List<Sprite> alternateSprites;
    public List<Sprite> characterIcons;
    public TextMeshProUGUI selectedCountText;
    public Button confirmButton;
    public Button backButton;
    public Button tutorialButton;
    public CharacterSpawner characterSpawner;

    private List<int> selectedCharacters = new List<int>();
    private int maxSelectableCharacters;
    private HashSet<int> toggledButtons = new HashSet<int>();
    private int lastSelectedCharacterIndex = -1;
    private Dictionary<int, string> characterSceneMap = new Dictionary<int, string>
    {
        { 0, "FrogTutorial" },
        { 1, "PenguinTutorial" },
        { 2, "MoleTutorial" }
    };

    [SerializeField] private AudioClip buttonSoundClip;

    private void Start()
    {
        int selectedMap = PlayerPrefs.GetInt("SelectedMap", 1);
        maxSelectableCharacters = (selectedMap == 1) ? 2 : 3;
        confirmButton.interactable = false;
        UpdateSelectionCount();

        for (int i = 0; i < characterButtons.Count; i++)
        {
            int index = i;
            characterButtons[i].onClick.AddListener(() => ToggleCharacterSelection(index));
        }

        confirmButton.onClick.AddListener(SaveSelectionAndLoadMap);
        backButton.onClick.AddListener(BackToPreviousScreen);
        tutorialButton.onClick.AddListener(GoToTutorialScene);

        tutorialButton.gameObject.SetActive(false);
    }

    private void ToggleCharacterSprite(int characterIndex)
    {
        if (toggledButtons.Contains(characterIndex))
        {
            characterButtons[characterIndex].GetComponent<Image>().sprite = originalSprites[characterIndex];
            toggledButtons.Remove(characterIndex);
        }
        else
        {
            characterButtons[characterIndex].GetComponent<Image>().sprite = alternateSprites[characterIndex];
            toggledButtons.Add(characterIndex);
        }
    }

    public void ToggleCharacterSelection(int characterIndex)
    {
        if (selectedCharacters.Contains(characterIndex))
        {
            selectedCharacters.Remove(characterIndex);

            if (originalSprites != null && originalSprites.Count > characterIndex)
            {
                characterButtons[characterIndex].GetComponent<Image>().sprite = originalSprites[characterIndex];
            }

            Debug.Log($"Postaæ {characterIndex} zosta³a odznaczona.");
        }
        else if (selectedCharacters.Count < maxSelectableCharacters)
        {
            selectedCharacters.Add(characterIndex);
            lastSelectedCharacterIndex = characterIndex;

            if (alternateSprites != null && alternateSprites.Count > characterIndex)
            {
                characterButtons[characterIndex].GetComponent<Image>().sprite = alternateSprites[characterIndex];
            }

            Debug.Log($"Postaæ {characterIndex} zosta³a wybrana.");

            if (characterSpawner != null)
            {
                characterSpawner.SpawnCharacter(characterIndex);
            }
            else
            {
                Debug.LogError("CharacterSpawner nie jest przypisany w inspektorze!");
            }

            //W³¹cz widocznoœæ przycisku tutorialButton
            if (!tutorialButton.gameObject.activeSelf)
            {
                tutorialButton.gameObject.SetActive(true);
            }
        }

        UpdateSelectionCount();

        confirmButton.interactable = selectedCharacters.Count == maxSelectableCharacters;
        SoundFXManager.instance.PlaySoundFXClip(buttonSoundClip, transform, 1f);
    }

    private void UpdateSelectionCount()
    {
        selectedCountText.text = $"Wybrano: {selectedCharacters.Count} z {maxSelectableCharacters}";
    }

    private async void GoToTutorialScene()
    {
        if (characterSceneMap.TryGetValue(lastSelectedCharacterIndex, out string tutorialSceneName))
        {
            SoundFXManager.instance.PlaySoundFXClip(buttonSoundClip, transform, 1f);
            await Task.Delay(100);
            SceneManager.LoadScene(tutorialSceneName);
        }
    }

    private async void SaveSelectionAndLoadMap()
    {
        if (selectedCharacters.Count == 0)
        {
            Debug.LogError("[CharacterSelectionManager] No characters selected to save!");
            return;
        }

        PlayerPrefs.SetString("SelectedCharacters", string.Join(",", selectedCharacters));
        PlayerPrefs.Save();
        Debug.Log($"[CharacterSelectionManager] Saved characters: {string.Join(",", selectedCharacters)}");

        int selectedMap = PlayerPrefs.GetInt("SelectedMap", 1);
        string sceneToLoad = "Map" + selectedMap;

        SoundFXManager.instance.PlaySoundFXClip(buttonSoundClip, transform, 1f);
        await Task.Delay(100);

        Debug.Log($"[CharacterSelectionManager] Loading scene: {sceneToLoad}");
        SceneManager.LoadScene(sceneToLoad);
    }

    private async void BackToPreviousScreen()
    {
        SoundFXManager.instance.PlaySoundFXClip(buttonSoundClip, transform, 1f);
        await Task.Delay(100);
        SceneManager.LoadScene("MapSelection");
    }
}
