using System.Collections;
using UnityEngine;

public class IceBlock : MonoBehaviour
{
    private Rigidbody2D rb;
    private bool isIceBlock = false;
    private bool isScheduledForDestruction = false;

    [SerializeField] private AudioClip iceBlockEnterSoundClip;
    [SerializeField] private AudioClip iceBlockCrackSoundClip;
    [SerializeField] private GameObject iceBlockPrefab;
    [SerializeField] private float destructionDelay = 2f;

    private GameManager gameManager;
    private bool createdOnSpikes = false;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        gameManager = FindObjectOfType<GameManager>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
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
                    spawnPosition.y += offsetY - 0.05f;
                }
            }

            GameObject newBlock = Instantiate(iceBlockPrefab, spawnPosition, Quaternion.identity);
            Transform innerIce = newBlock.transform.GetChild(0);
            BoxCollider2D iceCollider = innerIce.GetComponent<BoxCollider2D>();

            if (iceCollider != null)
            {
                Vector2 min = (Vector2)innerIce.position + iceCollider.offset - iceCollider.size * 0.5f;
                Vector2 max = (Vector2)innerIce.position + iceCollider.offset + iceCollider.size * 0.5f;

                Collider2D[] hits = Physics2D.OverlapAreaAll(min, max);
                foreach (var hit in hits)
                {
                    if (hit.CompareTag("Spikes"))
                    {
                        Debug.Log("[IceBlock] Detected TAGGED spike — destroying ice block!");
                        IceBlock ice = newBlock.GetComponent<IceBlock>();
                        if (ice != null)
                        {
                            ice.TriggerDelayedDestruction();
                        }
                        break;
                    }
                }
            }

            SoundFXManager.instance.PlaySoundFXClip(iceBlockEnterSoundClip, transform, 1f);
        }

        gameObject.SetActive(false);
    }

    public void TriggerDelayedDestruction()
    {
        if (!isScheduledForDestruction)
        {
            isScheduledForDestruction = true;
            StartCoroutine(DestroyAfterDelay());
        }
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

        if (iceBlockCrackSoundClip != null)
        {
            SoundFXManager.instance.PlaySoundFXClip(iceBlockCrackSoundClip, transform, 1f);
        }

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

