using System.Collections;
using UnityEngine;

public class IceBlock : MonoBehaviour
{
    private Rigidbody2D rb;
    private bool isIceBlock = false;
    private bool isScheduledForDestruction = false;

    [SerializeField] private AudioClip iceBlockEnterSoundClip;
    [SerializeField] private GameObject iceBlockPrefab;
    [SerializeField] private float destructionDelay = 2f; // Czas w sekundach przed znikniêciem kostki

    private GameManager gameManager;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        gameManager = FindObjectOfType<GameManager>();
    }

    void Update()
    {
        if (!isIceBlock && Input.GetKeyDown(KeyCode.E))
        {
            ActivateIceBlock();
            gameManager?.SwitchToNextCharacter();
        }
    }

    private void ActivateIceBlock()
    {
        isIceBlock = true;

        rb.velocity = Vector2.zero;
        rb.isKinematic = true;
        rb.simulated = false;

        if (iceBlockPrefab != null)
        {
            Vector3 spawnPosition = transform.position;

            BoxCollider2D playerCollider = GetComponent<BoxCollider2D>();
            Transform iceChild = iceBlockPrefab.transform.GetChild(0);

            if (playerCollider != null && iceChild != null)
            {
                BoxCollider2D iceChildCollider = iceChild.GetComponent<BoxCollider2D>();
                if (iceChildCollider != null)
                {
                    float offsetY = (playerCollider.bounds.extents.y - iceChildCollider.bounds.extents.y);
                    spawnPosition.y += offsetY;
                }
            }

            Instantiate(iceBlockPrefab, spawnPosition, Quaternion.identity);
            SoundFXManager.instance.PlaySoundFXClip(iceBlockEnterSoundClip, transform, 1f);
        }

        gameObject.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Spikes") && !isScheduledForDestruction)
        {
            isScheduledForDestruction = true;
            StartCoroutine(DestroyAfterDelay());
        }
    }

    private IEnumerator DestroyAfterDelay()
    {
        yield return new WaitForSeconds(destructionDelay);
        Destroy(gameObject);
    }

    private void OnDisable()
    {
        if (rb != null)
        {
            rb.simulated = true;
            rb.isKinematic = false;
            rb.gravityScale = 5f;
            rb.velocity = Vector2.zero;
        }

        var col = GetComponent<Collider2D>();
        if (col != null)
        {
            col.enabled = true;
        }

        transform.position += Vector3.up * 0.01f;
    }
}
