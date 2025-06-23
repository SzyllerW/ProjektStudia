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
    public Button confirmButton;

    [Header("Przyciski do tutoriali")]
    public Button frogTutorialButton;
    public Button moleTutorialButton;
    public Button penguinTutorialButton;
    public Button catTutorialButton;

    private int selectedMap = 0;
    private Color defaultColor = Color.white;
    private Color selectedColor = Color.grey;

    [SerializeField] private AudioClip buttonSoundClip;

    private void Start()
    {
        int map1Completed = PlayerPrefs.GetInt("Map1Completed", 1);
        int map2Completed = PlayerPrefs.GetInt("Map2Completed", 1);

        Debug.Log($"Map1Completed: {map1Completed}");
        Debug.Log($"Map2Completed: {map2Completed}");

        map1Button.interactable = true;
        map2Button.interactable = true;
        map3Button.interactable = true;
        map4Button.interactable = true;
        map5Button.interactable = true;
        map6Button.interactable = true;
        map7Button.interactable = true;
        map8Button.interactable = true;

        backButton.onClick.AddListener(BackToMainMenu);
        confirmButton.onClick.AddListener(ConfirmSelection);
        confirmButton.interactable = false;

        map1Button.onClick.AddListener(() => ToggleMapSelection(1, map1Button));
        map2Button.onClick.AddListener(() => ToggleMapSelection(2, map2Button));
        map3Button.onClick.AddListener(() => ToggleMapSelection(3, map3Button));
        map4Button.onClick.AddListener(() => ToggleMapSelection(4, map4Button));
        map5Button.onClick.AddListener(() => ToggleMapSelection(5, map5Button));
        map6Button.onClick.AddListener(() => ToggleMapSelection(6, map6Button));
        map7Button.onClick.AddListener(() => ToggleMapSelection(7, map7Button));
        map8Button.onClick.AddListener(() => ToggleMapSelection(8, map8Button));

        frogTutorialButton.onClick.AddListener(LoadFrogTutorial);
        moleTutorialButton.onClick.AddListener(LoadMoleTutorial);
        penguinTutorialButton.onClick.AddListener(LoadPenguinTutorial);
        catTutorialButton.onClick.AddListener(LoadCatTutorial);

        ResetButtonColors();
    }

    public async void ToggleMapSelection(int mapIndex, Button button)
    {
        SoundFXManager.instance.PlaySoundFXClip(buttonSoundClip, transform, 1f);
        await Task.Delay(100);

        if (selectedMap == mapIndex)
        {
            selectedMap = 0;
            confirmButton.interactable = false;
            button.GetComponent<Image>().color = defaultColor;
        }
        else
        {
            selectedMap = mapIndex;
            confirmButton.interactable = true;
            ResetButtonColors();
            button.GetComponent<Image>().color = selectedColor;
        }
    }

    private void ResetButtonColors()
    {
        map1Button.GetComponent<Image>().color = defaultColor;
        map2Button.GetComponent<Image>().color = defaultColor;
        map3Button.GetComponent<Image>().color = defaultColor;
        map4Button.GetComponent<Image>().color = defaultColor;
        map5Button.GetComponent<Image>().color = defaultColor;
        map6Button.GetComponent<Image>().color = defaultColor;
        map7Button.GetComponent<Image>().color = defaultColor;
        map8Button.GetComponent<Image>().color = defaultColor;
    }

    private async void ConfirmSelection()
    {
        if (selectedMap > 0)
        {
            SoundFXManager.instance.PlaySoundFXClip(buttonSoundClip, transform, 1f);
            await Task.Delay(100);
            PlayerPrefs.SetInt("SelectedMap", selectedMap);
            PlayerPrefs.Save();
            SceneManager.LoadScene("Map" + selectedMap);
        }
    }

    private async void BackToMainMenu()
    {
        SoundFXManager.instance.PlaySoundFXClip(buttonSoundClip, transform, 1f);
        await Task.Delay(100);
        SceneManager.LoadScene("MainMenu");
    }

    public void CompleteLevel()
    {
        Debug.Log("[LevelManager] Level Completed!");

        int currentMap = PlayerPrefs.GetInt("SelectedMap", 1);
        Debug.Log($"[LevelManager] Current Map: {currentMap}");

        if (currentMap == 1 && PlayerPrefs.GetInt("Map1Completed", 0) == 0)
        {
            PlayerPrefs.SetInt("Map1Completed", 1);
            Debug.Log("Mapa 1 ukoñczona! Map2 odblokowana!");
        }
        else if (currentMap == 2 && PlayerPrefs.GetInt("Map2Completed", 0) == 0)
        {
            PlayerPrefs.SetInt("Map2Completed", 1);
            Debug.Log("Mapa 2 ukoñczona! Map3 odblokowana!");
        }

        PlayerPrefs.Save();
    }

    private void LoadFrogTutorial()
    {
        SoundFXManager.instance.PlaySoundFXClip(buttonSoundClip, transform, 1f);
        SceneManager.LoadScene("FrogTutorial");
    }

    private void LoadMoleTutorial()
    {
        SoundFXManager.instance.PlaySoundFXClip(buttonSoundClip, transform, 1f);
        SceneManager.LoadScene("MoleTutorial");
    }

    private void LoadPenguinTutorial()
    {
        SoundFXManager.instance.PlaySoundFXClip(buttonSoundClip, transform, 1f);
        SceneManager.LoadScene("PenguinTutorial");
    }

    private void LoadCatTutorial()
    {
        SoundFXManager.instance.PlaySoundFXClip(buttonSoundClip, transform, 1f);
        SceneManager.LoadScene("KittyTutorial");
    }
}
