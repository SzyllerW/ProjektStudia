using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MapSelectionManager : MonoBehaviour
{
    public Button map1Button;
    public Button map2Button;
    public Button map3Button;
    public Button map4Button;
    public Button map5Button;
    public Button map6Button;
    public Button map7Button;
    public Button map8Button;
    public Button backButton;

    [Header("Przyciski do tutoriali (przypisz w Inspectorze zgodnie z indexem postaci)")]
    public List<Button> tutorialButtons; 

    [SerializeField] private AudioClip buttonSoundClip;
    [SerializeField] private TransitionsManager transitionsManager;

    private int selectedMap = 0;
    private Dictionary<string, Button> levelButtonMap;

    private void Start()
    {
        SaveSystem.Load();

        levelButtonMap = new Dictionary<string, Button>
        {
            { "Map1", map1Button },
            { "Map2", map2Button },
            { "Map3", map3Button },
            { "Map4", map4Button },
            { "Map5", map5Button },
            { "Map6", map6Button },
            { "Map7", map7Button },
            { "Map8", map8Button },
        };

        foreach (var btn in levelButtonMap.Values)
            btn.interactable = false;

        foreach (string level in SaveSystem.CurrentData.unlockedLevels)
        {
            if (levelButtonMap.TryGetValue(level, out Button btn))
                btn.interactable = true;
        }

        foreach (var btn in tutorialButtons)
            btn.interactable = false;

        foreach (int characterIndex in SaveSystem.CurrentData.unlockedCharacters)
        {
            if (characterIndex >= 0 && characterIndex < tutorialButtons.Count)
                tutorialButtons[characterIndex].interactable = true;
        }

        map1Button.onClick.AddListener(() => ToggleMapSelection(1, "Map1"));
        map2Button.onClick.AddListener(() => ToggleMapSelection(2, "Map2"));
        map3Button.onClick.AddListener(() => ToggleMapSelection(3, "Map3"));
        map4Button.onClick.AddListener(() => ToggleMapSelection(4, "Map4"));
        map5Button.onClick.AddListener(() => ToggleMapSelection(5, "Map5"));
        map6Button.onClick.AddListener(() => ToggleMapSelection(6, "Map6"));
        map7Button.onClick.AddListener(() => ToggleMapSelection(7, "Map7"));
        map8Button.onClick.AddListener(() => ToggleMapSelection(8, "Map8"));

        for (int i = 0; i < tutorialButtons.Count; i++)
        {
            int capturedIndex = i;
            tutorialButtons[i].onClick.AddListener(() => LoadTutorial(capturedIndex));
        }

        backButton.onClick.AddListener(BackToMainMenu);
    }

    public async void ToggleMapSelection(int mapIndex, string sceneName)
    {
        SoundFXManager.instance?.PlaySoundFXClip(buttonSoundClip, transform, 1f);
        await Task.Delay(100);
        PlayerPrefs.SetInt("SelectedMap", mapIndex);
        PlayerPrefs.Save();
        transitionsManager.StartCoroutine(transitionsManager.fadeExit(sceneName));
    }

    private async void BackToMainMenu()
    {
        SoundFXManager.instance?.PlaySoundFXClip(buttonSoundClip, transform, 1f);
        await Task.Delay(100);
        SceneManager.LoadScene("MainMenu");
    }

    private void LoadTutorial(int index)
    {
        SoundFXManager.instance?.PlaySoundFXClip(buttonSoundClip, transform, 1f);

        string[] tutorialScenes = { "FrogTutorial", "PenguinTutorial", "MoleTutorial", "KittyTutorial" };

        if (index >= 0 && index < tutorialScenes.Length)
        {
            transitionsManager.StartCoroutine(transitionsManager.fadeExit(tutorialScenes[index]));
        }
    }
}
