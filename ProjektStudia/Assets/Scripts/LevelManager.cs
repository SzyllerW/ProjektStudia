using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    public int totalItems;
    private int itemsCollected = 0;

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
        totalItems = FindObjectsOfType<CollectibleItem>().Length;
    }

    private void ItemCollected()
    {
        itemsCollected++;

        if (itemsCollected >= totalItems)
        {
            LevelCompleted();
        }
    }

    private void LevelCompleted()
    {
        Debug.Log("Level Completed!");
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
}