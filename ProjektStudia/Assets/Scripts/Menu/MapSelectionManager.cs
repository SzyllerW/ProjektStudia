using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MapSelectionManager : MonoBehaviour
{
    public UnityEngine.UI.Button map1Button; 
    public UnityEngine.UI.Button map2Button;
    public UnityEngine.UI.Button map3Button;
    public UnityEngine.UI.Button backButton; 
    public UnityEngine.UI.Button confirmButton; 
    private int selectedMap = 0; 
    private Color defaultColor = Color.white;
    private Color selectedColor = Color.grey;

    [SerializeField] private AudioClip buttonSoundClip;

    private void Start()
    {
        map2Button.interactable = PlayerPrefs.GetInt("Map2Unlocked", 0) == 1;

        backButton.onClick.AddListener(BackToMainMenu);
        confirmButton.onClick.AddListener(ConfirmSelection);
        confirmButton.interactable = false;

        map1Button.onClick.AddListener(() => ToggleMapSelection(1, map1Button));
        map2Button.onClick.AddListener(() => ToggleMapSelection(2, map2Button));
        map3Button.onClick.AddListener(() => ToggleMapSelection(3, map3Button));

        ResetButtonColors();
    }

    public void ToggleMapSelection(int mapIndex, UnityEngine.UI.Button button)
    {
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
    }

    private async void ConfirmSelection()
    {
        if (selectedMap > 0)
        {
            SoundFXManager.instance.PlaySoundFXClip(buttonSoundClip, transform, 1f);
            await Task.Delay(100);
            PlayerPrefs.SetInt("SelectedMap", selectedMap);
            SceneManager.LoadScene("CharacterSelection");
        }
    }

    private async void BackToMainMenu()
    {
        SoundFXManager.instance.PlaySoundFXClip(buttonSoundClip, transform, 1f);
        await Task.Delay(100);
        SceneManager.LoadScene("MainMenu");
    }
}