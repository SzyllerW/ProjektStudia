using UnityEngine;

public class CollectibleItem : MonoBehaviour
{
    public delegate void ItemCollected();
    public static event ItemCollected OnItemCollected;

    private bool isCollected = false;

    private void Start()
    {
        Debug.Log($"[CollectibleItem] Initialized {gameObject.name}");
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !isCollected)
        {
            isCollected = true;
            Debug.Log($"[CollectibleItem] {gameObject.name} collected.");
            OnItemCollected?.Invoke();
            Destroy(gameObject);
        }
    }
}