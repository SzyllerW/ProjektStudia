using UnityEngine;

public class CheckPoint : MonoBehaviour
{
    [SerializeField] private GameObject respawnPoint;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if (respawnPoint != null)
            {
                respawnPoint.transform.position = transform.position;
            }

            Destroy(gameObject);
        }
    }
}
