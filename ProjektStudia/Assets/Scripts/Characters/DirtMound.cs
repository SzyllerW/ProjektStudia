using UnityEngine;

public class DirtMound : MonoBehaviour
{
    [SerializeField] private float bouncePower = 10f;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Rigidbody2D playerRb = other.GetComponent<Rigidbody2D>();
            if (playerRb != null)
            {
                playerRb.velocity = new Vector2(playerRb.velocity.x, bouncePower);
                Destroy(gameObject);
            }
        }
    }
}