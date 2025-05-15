using UnityEngine;

public class WallClingJump : MonoBehaviour
{
    [Header("Wall Cling Settings")]
    public LayerMask wallLayer;
    public float wallCheckDistance = 0.3f;
    public float slideSpeed = 2.5f;
    public float wallJumpForceX = 10f;
    public float wallJumpForceY = 14f;

    private Rigidbody2D rb;
    private PlayerMovement playerMovement;
    private Animator animator;

    private bool isClinging;
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

        Debug.DrawRay(origin, Vector2.left * wallCheckDistance, Color.red);
        Debug.DrawRay(origin, Vector2.right * wallCheckDistance, Color.green);

        if (hitLeft.collider != null)
        {
            Debug.Log("ŒCIANA W LEWO: " + hitLeft.collider.name);
        }
        if (hitRight.collider != null)
        {
            Debug.Log("ŒCIANA W PRAWO: " + hitRight.collider.name);
        }

        bool isTouchingWall = hitLeft.collider != null || hitRight.collider != null;

        if (!canCling)
        {
            ResetClingState();
            return;
        }

        if (isTouchingWall && !isGrounded)
        {
            if (!isClinging)
            {
                wallNormal = hitLeft.collider ? hitLeft.normal : hitRight.normal;
                isClinging = true;
                canWallJump = true;
                playerMovement.ResetCoyoteTime();
                Debug.Log("CLING START");
            }
        }
        else
        {
            if (isClinging)
            {
                Debug.Log("CLING END");
                ResetClingState();
            }
        }

        if (isClinging)
        {
            animator.SetBool("IsHoldingWall", true);
            animator.SetBool("IsJumping", false);

            rb.velocity = new Vector2(rb.velocity.x, -slideSpeed);

            if (Input.GetButtonDown("Jump") && canWallJump)
            {
                Vector2 jumpDir = new Vector2(-wallNormal.x * wallJumpForceX, wallJumpForceY);
                playerMovement.RequestExternalJump(jumpDir.y);
                rb.velocity = new Vector2(jumpDir.x, rb.velocity.y);

                canWallJump = false;
                canCling = false;
                Debug.Log("WALL JUMP");

                ResetClingState();
                Invoke(nameof(EnableCling), 0.2f);
            }
        }
    }

    void ResetClingState()
    {
        isClinging = false;
        animator.SetBool("IsHoldingWall", false);
    }

    void EnableCling()
    {
        canCling = true;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(transform.position, transform.position + Vector3.right * wallCheckDistance);
        Gizmos.DrawLine(transform.position, transform.position + Vector3.left * wallCheckDistance);
    }
}




