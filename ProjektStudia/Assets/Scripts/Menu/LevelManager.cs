using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    private int totalItems = 0;
    private int itemsCollected = 0;

    [SerializeField] private GameObject victoryScreenPrefab; // Przeci�gnij prefab ekranu wygranej w inspektorze

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

        // Zlicza przedmioty tylko na pocz�tku gry
        totalItems = FindObjectsOfType<CollectibleItem>().Length;
        Debug.Log($"[LevelManager] Total items to collect at start: {totalItems}");
    }

    private void ClearPrefsOnLoad()
    {
        PlayerPrefs.DeleteKey("SelectedMap");
        PlayerPrefs.DeleteKey("SelectedCharacters");
        PlayerPrefs.Save();
        Debug.Log("[LevelManager] Cleared PlayerPrefs for selected map and characters.");
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

        // Sprawd�, czy prefab ekranu wygranej zosta� przypisany
        if (victoryScreenPrefab != null)
        {
            // Wy�wietl prefab ekranu wygranej
            GameObject victoryScreen = Instantiate(victoryScreenPrefab);
            victoryScreen.SetActive(true); // Aktywuj ekran wygranej
        }
        else
        {
            // Domy�lne zachowanie: przej�cie do kolejnej sceny
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
            // Mo�na doda� kod powrotu do menu g��wnego
            SceneManager.LoadScene("MainMenu");
        }
    }
}