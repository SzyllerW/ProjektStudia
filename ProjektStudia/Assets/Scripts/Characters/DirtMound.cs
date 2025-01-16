using UnityEngine;

public class DirtMound : MonoBehaviour
{
    [SerializeField] private float bouncePower = 10f; 
    [SerializeField] private LayerMask groundLayer;    
    [SerializeField] private float offsetAbovePlatform = 0.1f;

    private bool playerOnMound = false;
    private Rigidbody2D playerRb;

    private void Start()
    {
        PositionOnTopOfPlatform();
    }

    private void Update()
    {
        if (playerOnMound && Input.GetKeyDown(KeyCode.Space) && playerRb != null)
        {
            playerRb.velocity = new Vector2(playerRb.velocity.x, bouncePower);
        }
    }

    private void PositionOnTopOfPlatform()
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, Mathf.Infinity, groundLayer);
        if (hit.collider != null)
        {
            float platformTop = hit.collider.bounds.max.y;

            transform.position = new Vector3(transform.position.x, platformTop + offsetAbovePlatform, transform.position.z);
        }
    
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerOnMound = true;
            playerRb = other.GetComponent<Rigidbody2D>();
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerOnMound = false;
            playerRb = null;
        }
    }
}