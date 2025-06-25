using System.Collections;
using UnityEngine;
using System.Collections.Generic;

public class MoleAbility : MonoBehaviour
{
    private Rigidbody2D rb;

    [SerializeField] private GameObject dirtMoundPrefab;
    [SerializeField] private float diggingDelay = 2.833f;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundCheckRadius = 0.2f;
    [SerializeField] private Animator animator;
    [SerializeField] private Transform parentObject;
    private List<SpriteRenderer> spriteRenderers = new List<SpriteRenderer>();
    private bool isDigging = false;
    public AudioClip moundPopSound;
    public float moundPopVolume = 1.0f;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        if (parentObject == null)
        {
            return;
        }

        spriteRenderers.AddRange(parentObject.GetComponentsInChildren<SpriteRenderer>());
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E) && !isDigging && IsGrounded())
        {
            StartAnimation();
        }
    }

    private void StartAnimation()
    {
        animator.SetBool("Dig", true);

        PlayerMovement playerMovement = GetComponent<PlayerMovement>();
        if (playerMovement != null)
        {
            playerMovement.removeControl = true;
        }
    }

    private void StartDigging()
    {
        if (isDigging) return;

        isDigging = true;
        rb.velocity = Vector2.zero;
        rb.isKinematic = true;
        StartCoroutine(DigAndSwitch());
    }

    private IEnumerator DigAndSwitch()
    {
        yield return new WaitForSeconds(diggingDelay);

        Instantiate(dirtMoundPrefab, transform.position, Quaternion.identity);
        SoundFXManager.instance.PlaySoundFXClip(moundPopSound, transform, moundPopVolume);

        GameManager gm = FindObjectOfType<GameManager>();
        if (gm != null)
        {
            gm.CharacterFellOffMap(gameObject);
        }

        rb.isKinematic = false;
        gameObject.SetActive(false);
        isDigging = false;
    }

    private bool IsGrounded()
    {
        return Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
    }

    public void SetVisibleOutsideMask()
    {
        foreach (var sr in spriteRenderers)
        {
            sr.maskInteraction = SpriteMaskInteraction.VisibleOutsideMask;
        }
    }
}

