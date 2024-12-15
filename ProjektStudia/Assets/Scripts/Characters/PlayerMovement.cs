using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private float horizontal;
    public float speed = 16f;
    private bool isFacingRight = true;

    [SerializeField] private float jumpingPower = 14f;
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private Animator animator;

    [SerializeField] private GameObject sideView;
    [SerializeField] private GameObject frontView;

    private bool isJumping = false;

    void Update()
    {
        horizontal = Input.GetAxisRaw("Horizontal");
        animator.SetFloat("Speed", Mathf.Abs(horizontal));

        if (!IsGrounded() && !isJumping)
        {
            animator.SetBool("IsJumping", true);
            isJumping = true;
            UpdateVisibility();
        }

        if (IsGrounded() && isJumping)
        {
            animator.SetBool("IsJumping", false);
            isJumping = false;
            UpdateVisibility();
        }

        if (Input.GetButtonDown("Jump") && IsGrounded())
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpingPower);
            animator.SetBool("IsJumping", true);
            isJumping = true;
            UpdateVisibility();
        }

        Flip();
        UpdateVisibility();
    }

    public bool IsGrounded()
    {
        return Physics2D.OverlapCircle(groundCheck.position, 0.2f, groundLayer) && rb.velocity.y <= 0f;
    }

    private void FixedUpdate()
    {
        if (IsGrounded())
        {
            rb.velocity = new Vector2(horizontal * speed, rb.velocity.y);
        }
        else
        {
            rb.velocity = new Vector2(horizontal * speed * 0.5f, rb.velocity.y);
        }
    }

    private void Flip()
    {
        if ((horizontal < 0f && isFacingRight) || (horizontal > 0f && !isFacingRight))
        {
            isFacingRight = !isFacingRight;
            Vector3 localScale = transform.localScale;
            localScale.x = Mathf.Abs(localScale.x) * (isFacingRight ? 1 : -1);
            transform.localScale = localScale;
        }
    }

    private void UpdateVisibility()
    {
        if (Mathf.Abs(horizontal) > 0)
        {
            sideView.SetActive(true);
            frontView.SetActive(false);
        }
        else if (IsGrounded())
        {
            sideView.SetActive(false);
            frontView.SetActive(true);
        }
        else
        {
            sideView.SetActive(true);
            frontView.SetActive(false);
        }
    }

    public float GetJumpingPower()
    {
        return jumpingPower;
    }
}