using UnityEngine;

public class SettingsMenuManager : MonoBehaviour
{
    public GameObject optionsMenu;
    public GameObject soundSettingsPanel;
    public GameObject resolutionSettingsPanel;

    private void Start()
    {
        ShowMainOptions();
    }

    public void ShowMainOptions()
    {
        optionsMenu.SetActive(true);
        soundSettingsPanel.SetActive(false);
        resolutionSettingsPanel.SetActive(false);
    }

    public void OpenSoundSettings()
    {
        optionsMenu.SetActive(false);
        soundSettingsPanel.SetActive(true);
        resolutionSettingsPanel.SetActive(false);
    }

    public void OpenResolutionSettings()
    {
        optionsMenu.SetActive(false);
        soundSettingsPanel.SetActive(false);
        resolutionSettingsPanel.SetActive(true);
    }
}

