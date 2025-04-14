using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenuManager : MonoBehaviour
{
    [SerializeField] private GameObject pauseMenuPanel;  
    [SerializeField] private GameObject settingsPanel;
    [SerializeField] private GameObject descriptionPanel;
    [SerializeField] private AudioClip buttonSoundClip;
    [SerializeField] private GameObject pauseButton;

    private bool isPaused = false;

    void Start()
    {
        pauseButton.SetActive(true);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        { 
            if(descriptionPanel.activeSelf && isPaused)
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
        settingsPanel.SetActive(false);
        Time.timeScale = 0f;
        isPaused = true;

        pauseButton.SetActive(false);
    }

    public async void ResumeGame()
    {
        SoundFXManager.instance.PlaySoundFXClip(buttonSoundClip, transform, 1f);
        await Task.Delay(100);

        pauseMenuPanel.SetActive(false);
        settingsPanel.SetActive(false);
        descriptionPanel.SetActive(false);
        Time.timeScale = 1f;
        isPaused = false;

        pauseButton.SetActive(true);
    }

    public async void OpenOptions()
    {
        SoundFXManager.instance.PlaySoundFXClip(buttonSoundClip, transform, 1f);
        await Task.Delay(100);

        pauseMenuPanel.SetActive(false);
        settingsPanel.SetActive(true);
    }

    public async void CloseOptions()
    {
        SoundFXManager.instance.PlaySoundFXClip(buttonSoundClip, transform, 1f);
        await Task.Delay(100);

        settingsPanel.SetActive(false);
        pauseMenuPanel.SetActive(true);
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
            settingsPanel.SetActive(false);
            descriptionPanel.SetActive(true);
            Time.timeScale = 0f;
            isPaused = true;
        }
        else
        {
            descriptionPanel.SetActive(false);
            pauseMenuPanel.SetActive(true);
        }
    }
}




