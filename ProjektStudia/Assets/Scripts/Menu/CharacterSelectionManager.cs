using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;

public class CharacterSelectionManager : MonoBehaviour
{
    public List<Button> characterButtons;
    public List<Sprite> characterIcons;
    public TextMeshProUGUI selectedCountText;
    public Button confirmButton;
    public Button backButton;

    private List<int> selectedCharacters = new List<int>();
    private int maxSelectableCharacters;

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
    }

    public void ToggleCharacterSelection(int characterIndex)
    {
        if (selectedCharacters.Contains(characterIndex))
        {
            selectedCharacters.Remove(characterIndex);
            characterButtons[characterIndex].interactable = true;
            characterButtons[characterIndex].GetComponent<Image>().color = Color.white;
        }
        else if (selectedCharacters.Count < maxSelectableCharacters)
        {
            selectedCharacters.Add(characterIndex);
            characterButtons[characterIndex].GetComponent<Image>().color = Color.gray;
        }

        UpdateSelectionCount();
        confirmButton.interactable = selectedCharacters.Count == maxSelectableCharacters;
    }

    private void UpdateSelectionCount()
    {
        selectedCountText.text = $"Wybrano: {selectedCharacters.Count}/{maxSelectableCharacters}";
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
