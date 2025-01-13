using UnityEngine;

public class DirtMound : MonoBehaviour
{
    [SerializeField] private float bouncePower = 10f;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private float offsetAbovePlatform = 0.1f;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Rigidbody2D playerRb = other.GetComponent<Rigidbody2D>();
            if (playerRb != null)
            {
                playerRb.velocity = new Vector2(playerRb.velocity.x, bouncePower);
            }

            Vector2 position = transform.position;
            Collider2D platform = Physics2D.OverlapCircle(position, 0.5f, groundLayer);

            if (platform != null)
            {
                float platformTop = platform.bounds.max.y;
                transform.position = new Vector3(transform.position.x, platformTop + offsetAbovePlatform, transform.position.z);
            }

            Destroy(gameObject, 0.1f);
        }
    }
}