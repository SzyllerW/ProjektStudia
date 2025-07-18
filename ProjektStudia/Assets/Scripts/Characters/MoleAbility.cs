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
        if (Input.GetKeyDown(KeyCode.E) && !isDigging && IsGrounded() && !IsOnMovingPlatform())
        {
            StartAnimation();
        }
        else if (Input.GetKeyDown(KeyCode.E) && IsOnMovingPlatform())
        {
            Debug.Log("Nie mo�na kopa� na ruchomej platformie.");
        }
    }

    private void StartAnimation()
    {
        animator.SetBool("Dig", true);
        rb.velocity = Vector2.zero;
        rb.isKinematic = true;

        PlayerMovement playerMovement = GetComponent<PlayerMovement>();
        if (playerMovement != null)
        {
            playerMovement.removeControl = true;
        }
        SoundFXManager.instance.PlaySoundFXClip(moundPopSound, transform, moundPopVolume);
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

    private bool IsOnMovingPlatform()
    {
        return transform.parent != null && transform.parent.GetComponent<MovingPlatform>() != null;
    }
    public void SetVisibleOutsideMask()
    {
        foreach (var sr in spriteRenderers)
        {
            sr.maskInteraction = SpriteMaskInteraction.VisibleOutsideMask;
        }
    }
}

