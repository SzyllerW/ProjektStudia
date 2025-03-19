using System.Collections;
using UnityEngine;

public class TrapPlatform : MonoBehaviour
{
    [SerializeField] private GameObject greenLight;
    [SerializeField] private GameObject redLight;
    [SerializeField] private GameObject spikes;
    [SerializeField] private float trapCycleTime = 3f;
    [SerializeField] private Animator animator;

    private bool isTrapActive = false;
    private GameObject playerOnPlatform;

    private void Start()
    {
        SetTrapState(false);
        StartCoroutine(ToggleTrap());
    }

    private IEnumerator ToggleTrap()
    {
        while (true)
        {
            yield return new WaitForSeconds(trapCycleTime);
            isTrapActive = !isTrapActive;
            SetTrapState(isTrapActive);

            if (isTrapActive && playerOnPlatform != null)
            {
                KillPlayer(playerOnPlatform);
            }
        }
    }

    private void SetTrapState(bool active)
    {
        greenLight.SetActive(!active);
        redLight.SetActive(active);
        animator.SetBool("RedLight", isTrapActive);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Player"))
        {
            playerOnPlatform = collision.gameObject;

            if (isTrapActive)
            {
                KillPlayer(playerOnPlatform);
            }
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Player"))
        {
            playerOnPlatform = null;
        }
    }

    private void KillPlayer(GameObject player)
    {
        PlayerVisualManager playerVisual = FindObjectOfType<PlayerVisualManager>();
        if (playerVisual != null)
        {
            playerVisual.PlayerTouchedSpikes();
        }

        PlayerMovement playerMovement = player.GetComponent<PlayerMovement>();
        if (playerMovement != null)
        {
            playerMovement.enabled = false;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("DirtMound"))
        {
            Destroy(other.gameObject);
        }
    }
}