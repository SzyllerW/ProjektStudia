using UnityEngine;

public class WallClingJump : MonoBehaviour
{
    [Header("Wall Cling Settings")]
    public LayerMask wallLayer;
    public float wallCheckDistance = 20f;
    public float slideSpeed = 2.5f;
    public float wallJumpForceX = 10f;
    public float wallJumpForceY = 14f;

    public bool IsClinging { get; private set; }

    private Rigidbody2D rb;
    private PlayerMovement playerMovement;
    private Animator animator;

    private Vector2 wallNormal;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        playerMovement = GetComponent<PlayerMovement>();
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        bool isGrounded = playerMovement.IsGrounded();
        Vector2 origin = (Vector2)transform.position + new Vector2(0f, 0.5f);

        RaycastHit2D hitLeft = Physics2D.Raycast(origin, Vector2.left, wallCheckDistance, wallLayer);
        RaycastHit2D hitRight = Physics2D.Raycast(origin, Vector2.right, wallCheckDistance, wallLayer);

        bool touchingLeft = hitLeft.collider != null;
        bool touchingRight = hitRight.collider != null;

        bool pressingLeft = Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A);
        bool pressingRight = Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D);

        bool pressingTowardsWall =
            (touchingLeft && (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A))) ||
            (touchingRight && (Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D)));

        bool isTouchingWall = touchingLeft || touchingRight;

        Debug.Log($"[CLING] IsClinging={IsClinging} | isTouchingWall={isTouchingWall} | pressingTowardsWall={pressingTowardsWall} | isGrounded={isGrounded} | velY={rb.velocity.y} | canWallJump=True");

        if (pressingTowardsWall && isTouchingWall && !isGrounded && rb.velocity.y <= 0.05f)
        {
            if (!IsClinging)
            {
                wallNormal = touchingLeft ? hitLeft.normal : hitRight.normal;
                IsClinging = true;
                playerMovement.ResetCoyoteTime();
                animator.SetBool("IsJumping", false);
                Debug.Log("[CLING] Started Clinging");
            }
        }
        else
        {
            if (IsClinging)
            {
                StopClinging();
                Debug.Log("[CLING] Stopped Clinging");
            }
        }

        if (IsClinging)
        {
            rb.velocity = new Vector2(rb.velocity.x, -slideSpeed);
            animator.SetBool("IsHoldingWall", true);
            animator.SetBool("IsJumping", false);

            if (Input.GetButtonDown("Jump"))
            {
                Vector2 jumpDir = new Vector2(-wallNormal.x * wallJumpForceX, wallJumpForceY);
                playerMovement.RequestExternalJump(jumpDir.y);
                rb.velocity = new Vector2(jumpDir.x, rb.velocity.y);
                StopClinging();
                Debug.Log("[CLING] Wall Jump Executed");
            }
        }

        if (!IsClinging && animator != null)
        {
            animator.SetBool("IsHoldingWall", false);
        }
    }

    void StopClinging()
    {
        IsClinging = false;
        animator.SetBool("IsHoldingWall", false);
    }

    void OnDrawGizmosSelected()
    {
        Vector2 origin = Application.isPlaying ? (Vector2)transform.position + new Vector2(0f, 0.5f) : (Vector2)transform.position;
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(origin, origin + Vector2.left * wallCheckDistance);
        Gizmos.DrawLine(origin, origin + Vector2.right * wallCheckDistance);
    }
}
