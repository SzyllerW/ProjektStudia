using UnityEngine;

public class WallClingJump : MonoBehaviour
{
    [Header("Wall Cling Settings")]
    public LayerMask wallLayer;
    public float wallCheckDistance = 0.5f;
    public float slideSpeed = 2.5f;
    public float wallJumpForceX = 10f;
    public float wallJumpForceY = 14f;

    public bool IsClinging { get; private set; }

    private Rigidbody2D rb;
    private PlayerMovement playerMovement;
    private Animator animator;

    private bool canWallJump = true;
    private bool canCling = true;
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

        bool pressingTowardsWall = (touchingLeft && pressingLeft) || (touchingRight && pressingRight);
        bool isTouchingWall = touchingLeft || touchingRight;

        Debug.DrawRay(origin, Vector2.left * wallCheckDistance, Color.red);
        Debug.DrawRay(origin, Vector2.right * wallCheckDistance, Color.green);
        Debug.Log($"[CLING DEBUG] TouchingWall: {isTouchingWall}, Grounded: {isGrounded}, VelocityY: {rb.velocity.y}");

        if (!canCling)
        {
            StopClinging();
            return;
        }

        if (pressingTowardsWall && !isGrounded && rb.velocity.y < 0)
        {
            if (!IsClinging)
            {
                wallNormal = touchingLeft ? hitLeft.normal : hitRight.normal;
                IsClinging = true;
                canWallJump = true;
                playerMovement.ResetCoyoteTime();
                if (animator != null) animator.SetBool("IsJumping", false); 
                Debug.Log("Started Clinging");
            }
        }
        else
        {
            if (IsClinging)
            {
                StopClinging();
                Debug.Log("Stopped Clinging");
            }
        }

        if (IsClinging)
        {
            rb.velocity = new Vector2(rb.velocity.x, -slideSpeed);

            if (animator != null)
            {
                animator.SetBool("IsHoldingWall", true);
                animator.SetBool("IsJumping", false);
            }

            if (Input.GetButtonDown("Jump") && canWallJump)
            {
                Vector2 jumpDir = new Vector2(-wallNormal.x * wallJumpForceX, wallJumpForceY);
                playerMovement.RequestExternalJump(jumpDir.y);
                rb.velocity = new Vector2(jumpDir.x, rb.velocity.y);

                canWallJump = false;
                canCling = false;

                StopClinging();
                Invoke(nameof(EnableCling), 0.2f);
                Debug.Log("Wall Jump Executed");
            }
        }
    }

    void StopClinging()
    {
        IsClinging = false;
        if (animator != null)
        {
            animator.SetBool("IsHoldingWall", false);
        }
    }

    void EnableCling()
    {
        canCling = true;
    }

    void OnDrawGizmosSelected()
    {
        Vector2 origin = Application.isPlaying ? (Vector2)transform.position + new Vector2(0f, 0.5f) : (Vector2)transform.position;
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(origin, origin + Vector2.left * wallCheckDistance);
        Gizmos.DrawLine(origin, origin + Vector2.right * wallCheckDistance);
    }
}
