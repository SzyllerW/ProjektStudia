using UnityEngine;

public class WallClingJump : MonoBehaviour
{
    [Header("Wall Cling Settings")]
    public LayerMask wallLayer;
    public float wallCheckDistance = 20f;
    public float slideSpeed = 2.5f;
    public float clingDelayTime = 0.2f;
    public float wallJumpForceX = 10f;
    public float wallJumpForceY = 14f;

    public bool IsClinging { get; private set; }

    private Rigidbody2D rb;
    private PlayerMovement playerMovement;
    private Animator animator;

    private bool canWallJump = true;
    private bool canCling = true;
    private Vector2 wallNormal;
    private bool clingDelayActive = false;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        playerMovement = GetComponent<PlayerMovement>();
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        bool isGrounded = playerMovement.IsGrounded();
        animator.SetBool("IsGrounded", isGrounded);

        Vector2 origin = (Vector2)transform.position + new Vector2(0f, 0.5f);

        RaycastHit2D hitLeft = Physics2D.Raycast(origin, Vector2.left, wallCheckDistance, wallLayer);
        RaycastHit2D hitRight = Physics2D.Raycast(origin, Vector2.right, wallCheckDistance, wallLayer);

        bool touchingLeft = hitLeft.collider != null;
        bool touchingRight = hitRight.collider != null;
        bool isTouchingWall = touchingLeft || touchingRight;

        bool pressingLeft = Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A);
        bool pressingRight = Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D);
        bool pressingTowardsWall = (touchingLeft && pressingLeft) || (touchingRight && pressingRight);
        bool pressingOppositeWall = (touchingLeft && pressingRight) || (touchingRight && pressingLeft);

        if (isGrounded || pressingOppositeWall)
        {
            StopClinging();
            return;
        }

        if (!canCling)
        {
            return;
        }

        if (pressingTowardsWall && !isGrounded)
        {
            if (!IsClinging)
            {
                wallNormal = touchingLeft ? hitLeft.normal : hitRight.normal;
                IsClinging = true;
                canWallJump = true;
                playerMovement.ResetCoyoteTime();
                animator.SetBool("IsJumping", false);

                clingDelayActive = true;
                Invoke(nameof(DisableClingDelay), clingDelayTime);
            }
        }

        if (IsClinging)
        {
            if (clingDelayActive)
            {
                rb.velocity = new Vector2(rb.velocity.x, 0f);
            }
            else
            {
                rb.velocity = new Vector2(rb.velocity.x, -slideSpeed);
            }

            animator.SetBool("IsHoldingWall", true);
            animator.SetBool("IsJumping", false);

            if (Input.GetButtonDown("Jump") && canWallJump)
            {
                Vector2 jumpDir = new Vector2(-wallNormal.x * wallJumpForceX, wallJumpForceY);
                playerMovement.RequestExternalJump(jumpDir.y, true);
                rb.velocity = new Vector2(jumpDir.x, rb.velocity.y);

                canWallJump = false;
                canCling = false;

                StopClinging();
                Invoke(nameof(EnableCling), 0.2f);
            }
        }

        if (animator != null)
        {
            animator.SetBool("IsHoldingWall", IsClinging);
        }
    }

    void StopClinging()
    {
        IsClinging = false;
        animator.SetBool("IsHoldingWall", false);
    }

    void EnableCling()
    {
        canCling = true;
    }

    void DisableClingDelay()
    {
        clingDelayActive = false;
    }
}