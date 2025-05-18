using UnityEngine;

public class WallClingJump : MonoBehaviour
{
    [Header("Wall Cling Settings")]
    public LayerMask wallLayer;
    public float wallCheckDistance = 20f;
    public float slideSpeed = 2.5f;
    public float wallJumpForceX = 10f;
    public float wallJumpForceY = 14f;

    private Rigidbody2D rb;
    private PlayerMovement playerMovement;
    private Animator animator;

    private bool isClinging = false;
    private bool canWallJump = false;
    private Vector2 wallNormal = Vector2.zero;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        playerMovement = GetComponent<PlayerMovement>();
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        Vector2 origin = transform.position;
        Vector2 direction = playerMovement.horizontal > 0 ? Vector2.right : Vector2.left;

        RaycastHit2D hit = Physics2D.Raycast(origin, direction, wallCheckDistance, wallLayer);
        Debug.DrawRay(origin, direction * wallCheckDistance, hit.collider != null ? Color.green : Color.red);

        bool isGrounded = playerMovement.IsGrounded();
        bool isTouchingWall = hit.collider != null;

        if (isTouchingWall && !isGrounded && Mathf.Abs(playerMovement.horizontal) > 0.1f)
        {
            if (!isClinging)
            {
                wallNormal = hit.normal;
                isClinging = true;
                canWallJump = true;
                Debug.Log("Started Clinging");
            }
        }
        else
        {
            if (isClinging)
            {
                isClinging = false;
                Debug.Log("Stopped Clinging");
            }
        }

        if (isClinging)
        {
            // Zsuwanie
            rb.velocity = new Vector2(rb.velocity.x, -slideSpeed);
            animator.SetBool("IsHoldingWall", true);

            if (Input.GetButtonDown("Jump") && canWallJump)
            {
                rb.velocity = new Vector2(-wallNormal.x * wallJumpForceX, wallJumpForceY);
                isClinging = false;
                canWallJump = false;
                playerMovement.ResetCoyoteTime();
                Debug.Log("Wall Jump");
            }
        }
        else
        {
            animator.SetBool("IsHoldingWall", false);
        }

        if (isGrounded)
        {
            canWallJump = true;
        }
    }
}




