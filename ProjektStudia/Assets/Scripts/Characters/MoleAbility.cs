using System.Collections;
using UnityEngine;

public class MoleAbility : MonoBehaviour
{
    private Rigidbody2D rb;

    [SerializeField] private GameObject dirtMoundPrefab;
    [SerializeField] private float diggingDelay = 2.833f;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundCheckRadius = 0.2f;
    [SerializeField] private Animator animator;

    private bool isDigging = false;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
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
        rb.isKinematic = false;
        gameObject.SetActive(false);

        GameManager gameManager = FindObjectOfType<GameManager>();
        if (gameManager != null)
        {
            gameManager.UnlockCharacterSelection();

        }

        isDigging = false;
    }

    private bool IsGrounded()
    {
        return Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
    }
}
