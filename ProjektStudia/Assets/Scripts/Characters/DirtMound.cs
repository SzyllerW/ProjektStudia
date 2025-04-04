using UnityEngine;

public class DirtMound : MonoBehaviour
{
    [SerializeField] private float bouncePower = 10f;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private float offsetAbovePlatform = 0.1f;
    [SerializeField] private AudioClip jumpSound;

    private void Start()
    {
        PositionOnTopOfPlatform();
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
            PlayerMovement playerMovement = other.GetComponent<PlayerMovement>();
            if (playerMovement != null)
            {
                playerMovement.RequestExternalJump(bouncePower);
                SoundFXManager.instance.PlaySoundFXClip(jumpSound, transform, 1f);
            }
        }
    }
}


