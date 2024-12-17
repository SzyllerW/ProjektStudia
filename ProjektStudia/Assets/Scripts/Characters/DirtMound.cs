using UnityEngine;

public class DirtMound : MonoBehaviour
{
    [SerializeField] private float bouncePower = 10f;
    [SerializeField] private LayerMask groundLayer; 

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Rigidbody2D playerRb = other.GetComponent<Rigidbody2D>();
            if (playerRb != null)
            {
                playerRb.velocity = new Vector2(playerRb.velocity.x, bouncePower);
            }

            Vector2 rayOrigin = other.transform.position;
            RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.down, 5f, groundLayer);

            if (hit.collider != null)
            {
                Vector3 moundPosition = new Vector3(other.transform.position.x, hit.point.y, other.transform.position.z);
                Instantiate(gameObject, moundPosition, Quaternion.identity);
            }
            else
            {
                Debug.LogWarning("Nie znaleziono platformy poni¿ej gracza!");
            }

            Destroy(gameObject);
        }
    }
}