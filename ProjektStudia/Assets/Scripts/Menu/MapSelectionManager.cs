using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MapSelectionManager : MonoBehaviour
{
    [Header("Przyciski do poziomów i tutoriali (w³aœciwa kolejnoœæ):")]
    public Button frogTutorialButton;
    public Button penguinTutorialButton;
    public Button moleTutorialButton;
    public Button kittyTutorialButton;
    public Button map1Button;
    public Button map2Button;
    public Button map3Button;
    public Button map4Button;
    public Button map5Button;
    public Button map6Button;
    public Button map7Button;
    public Button map8Button;
    public Button backButton;

    [SerializeField] private AudioClip buttonSoundClip;
    [SerializeField] private TransitionsManager transitionsManager;

    private Dictionary<string, Button> levelButtonMap;

    private void Start()
    {
        SaveSystem.Load();

        levelButtonMap = new Dictionary<string, Button>
        {
            {"FrogTutorial", frogTutorialButton},
            {"Map6", map6Button},
            {"PenguinTutorial", penguinTutorialButton},
            {"Map2", map2Button},
            {"MoleTutorial", moleTutorialButton},
            {"Map8", map8Button},
            {"Map3", map3Button},
            {"KittyTutorial", kittyTutorialButton},
            {"Map4", map4Button},
            {"Map5", map5Button},
            {"Map7", map7Button},
            {"Map1", map1Button},
        };

        foreach (var entry in levelButtonMap)
        {
            string levelName = entry.Key;
            Button btn = entry.Value;
            btn.interactable = SaveSystem.CurrentData.unlockedLevels.Contains(levelName);
            string capturedLevel = levelName;
            btn.onClick.AddListener(() => LoadLevel(capturedLevel));
        }

        backButton.onClick.AddListener(BackToMainMenu);
    }

    private async void LoadLevel(string sceneName)
    {
        SoundFXManager.instance?.PlaySoundFXClip(buttonSoundClip, transform, 1f);
        await Task.Delay(100);
        transitionsManager.StartCoroutine(transitionsManager.fadeExit(sceneName));
    }

    private async void BackToMainMenu()
    {
        SoundFXManager.instance?.PlaySoundFXClip(buttonSoundClip, transform, 1f);
        await Task.Delay(100);
        SceneManager.LoadScene("MainMenu");
    }
}
