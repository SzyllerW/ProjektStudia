using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenuManager : MonoBehaviour
{
    [SerializeField] private GameObject pauseMenuPanel;
    [SerializeField] private GameObject[] settingsPanels;
    [SerializeField] private GameObject navigationPanel;
    [SerializeField] private GameObject leftArrowButton;
    [SerializeField] private GameObject rightArrowButton;
    [SerializeField] private GameObject descriptionPanel;
    [SerializeField] private AudioClip buttonSoundClip;
    [SerializeField] private GameObject pauseButton;

    private bool isPaused = false;
    private int currentSettingsIndex = 0;

    void Start()
    {
        pauseMenuPanel.SetActive(false);
        HideAllSettingsPanels();
        descriptionPanel.SetActive(false);
        navigationPanel.SetActive(false);
        pauseButton.SetActive(true);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (descriptionPanel.activeSelf && isPaused)
            {
                ShowDescriptionPanel();
                return;
            }

            if (isPaused)
            {
                ResumeGame();
            }
            else
            {
                PauseGame();
            }
        }
    }

    public async void RestartLevel()
    {
        SoundFXManager.instance.PlaySoundFXClip(buttonSoundClip, transform, 1f);
        await Task.Delay(100);

        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public async void PauseGame()
    {
        SoundFXManager.instance.PlaySoundFXClip(buttonSoundClip, transform, 1f);
        await Task.Delay(100);

        pauseMenuPanel.SetActive(true);
        HideAllSettingsPanels();
        descriptionPanel.SetActive(false);
        navigationPanel.SetActive(false);

        Time.timeScale = 0f;
        isPaused = true;
        pauseButton.SetActive(false);
    }

    public async void ResumeGame()
    {
        SoundFXManager.instance.PlaySoundFXClip(buttonSoundClip, transform, 1f);
        await Task.Delay(100);

        pauseMenuPanel.SetActive(false);
        HideAllSettingsPanels();
        descriptionPanel.SetActive(false);
        navigationPanel.SetActive(false);

        Time.timeScale = 1f;
        isPaused = false;
        pauseButton.SetActive(true);
    }

    public async void OpenSettings()
    {
        SoundFXManager.instance.PlaySoundFXClip(buttonSoundClip, transform, 1f);
        await Task.Delay(100);

        pauseMenuPanel.SetActive(false);
        navigationPanel.SetActive(true);
        ResetNavigation(); 
        ShowSettingsPanel(currentSettingsIndex);

        descriptionPanel.SetActive(false);
    }

    public void ShowNextSettingsPanel()
    {
        currentSettingsIndex = (currentSettingsIndex + 1) % settingsPanels.Length;
        ShowSettingsPanel(currentSettingsIndex);
    }

    public void ShowPreviousSettingsPanel()
    {
        currentSettingsIndex--;
        if (currentSettingsIndex < 0)
            currentSettingsIndex = settingsPanels.Length - 1;

        ShowSettingsPanel(currentSettingsIndex);
    }

    private void ShowSettingsPanel(int index)
    {
        for (int i = 0; i < settingsPanels.Length; i++)
        {
            settingsPanels[i].SetActive(i == index);
        }

        UpdateNavigationArrows();
    }

    private void HideAllSettingsPanels()
    {
        foreach (GameObject panel in settingsPanels)
        {
            panel.SetActive(false);
        }
    }

    private void UpdateNavigationArrows()
    {
        if (settingsPanels.Length <= 1)
        {
            leftArrowButton.SetActive(false);
            rightArrowButton.SetActive(false);
            return;
        }

        if (currentSettingsIndex == 0)
        {
            leftArrowButton.SetActive(false);
            rightArrowButton.SetActive(true);
        }
        else if (currentSettingsIndex == settingsPanels.Length - 1)
        {
            leftArrowButton.SetActive(true);
            rightArrowButton.SetActive(false);
        }
        else
        {
            leftArrowButton.SetActive(true);
            rightArrowButton.SetActive(true);
        }
    }

    private void ResetNavigation()
    {
        currentSettingsIndex = 0; 
        leftArrowButton.SetActive(false); 
        rightArrowButton.SetActive(true);
    }

    public async void BackToPauseMenu()
    {
        SoundFXManager.instance.PlaySoundFXClip(buttonSoundClip, transform, 1f);
        await Task.Delay(100);

        pauseMenuPanel.SetActive(true);
        HideAllSettingsPanels();
        descriptionPanel.SetActive(false);
        navigationPanel.SetActive(false);
    }

    public async void GoToMainMenu()
    {
        SoundFXManager.instance.PlaySoundFXClip(buttonSoundClip, transform, 1f);
        await Task.Delay(100);

        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }

    public async void ShowDescriptionPanel()
    {
        SoundFXManager.instance.PlaySoundFXClip(buttonSoundClip, transform, 1f);
        await Task.Delay(100);

        if (!descriptionPanel.activeSelf)
        {
            pauseMenuPanel.SetActive(false);
            HideAllSettingsPanels();
            descriptionPanel.SetActive(true);
            navigationPanel.SetActive(false);

            Time.timeScale = 0f;
            isPaused = true;
        }
        else
        {
            descriptionPanel.SetActive(false);
            pauseMenuPanel.SetActive(true);
            navigationPanel.SetActive(false);
        }
    }
}

