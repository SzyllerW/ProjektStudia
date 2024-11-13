using UnityEngine;

public class CollectibleItem : MonoBehaviour
{
    public delegate void ItemCollected();
    public static event ItemCollected OnItemCollected;

    private bool isCollected = false;

    private void Start()
    {
        Debug.Log($"[CollectibleItem] Initialized collectible item: {gameObject.name} at position {transform.position}");
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !isCollected)
        {
            isCollected = true;  // Ustawiamy flagê, aby zapobiec wielokrotnemu zebraniu
            Debug.Log($"[CollectibleItem] {gameObject.name} collected by player.");
            OnItemCollected?.Invoke();
            Destroy(gameObject);
        }
    }
}