using UnityEngine;

public class CollectibleItem : MonoBehaviour
{
    public delegate void ItemCollected();
    public static event ItemCollected OnItemCollected;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            OnItemCollected?.Invoke();
            Destroy(gameObject);
        }
    }
}