using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    private int totalItems = 0;
    private int itemsCollected = 0;

    [SerializeField] private string victorySceneName = "LevelComplete";

    private void OnEnable()
    {
        CollectibleItem.OnItemCollected += ItemCollected;
    }

    private void OnDisable()
    {
        CollectibleItem.OnItemCollected -= ItemCollected;
    }

    private void Start()
    {
        ClearPrefsOnLoad();

        totalItems = FindObjectsOfType<CollectibleItem>().Length;
        Debug.Log($"[LevelManager] Total items to collect at start: {totalItems}");
    }

    private void ClearPrefsOnLoad()
    {
        PlayerPrefs.DeleteKey("OtherUnnecessaryKey");
        PlayerPrefs.Save();
    }

    private void ItemCollected()
    {
        itemsCollected++;
        Debug.Log($"[LevelManager] Item collected. Total collected items: {itemsCollected}/{totalItems}");

        if (itemsCollected >= totalItems)
        {
            Debug.Log("[LevelManager] All items collected. Triggering level completion.");
            LevelCompleted();
        }
    }

    private void LevelCompleted()
    {
        Debug.Log("[LevelManager] Level Completed!");

        if (!string.IsNullOrEmpty(victorySceneName))
        {
            Debug.Log("[LevelManager] Loading victory scene.");
            SceneManager.LoadScene(victorySceneName); 
        }
        else
        {
            Debug.LogWarning("[LevelManager] Victory scene not set. Loading next scene.");
            LoadNextScene();
        }
    }

    public void LoadNextScene()
    {
        int nextSceneIndex = SceneManager.GetActiveScene().buildIndex + 1;
        if (nextSceneIndex < SceneManager.sceneCountInBuildSettings)
        {
            Debug.Log($"[LevelManager] Loading next scene: {nextSceneIndex}");
            SceneManager.LoadScene(nextSceneIndex);
        }
        else
        {
            Debug.Log("[LevelManager] All levels completed. Returning to main menu or ending game.");
            SceneManager.LoadScene("MainMenu");
        }
    }
}