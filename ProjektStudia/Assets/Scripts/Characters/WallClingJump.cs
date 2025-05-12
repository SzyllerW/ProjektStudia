using UnityEngine;

public class WallClingJump : MonoBehaviour
{
    [Header("Wall Cling Settings")]
    public LayerMask wallLayer;
    public float wallCheckDistance = 0.3f;
    public float slideSpeed = 2.5f;
    public float slideAcceleration = 3f;
    public float wallJumpForceX = 10f;
    public float wallJumpForceY = 14f;

    private Rigidbody2D rb;
    private PlayerMovement playerMovement;

    private bool isTouchingWall;
    private bool isClinging;
    private Vector2 wallNormal;
    private bool canWallJump = true;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        playerMovement = GetComponent<PlayerMovement>();
    }

    void Update()
    {
        Vector2 direction = Vector2.right * Mathf.Sign(playerMovement.horizontal);
        RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, wallCheckDistance, wallLayer);
        isTouchingWall = hit.collider != null;

        bool wantsToCling = Input.GetAxisRaw("Horizontal") != 0 && Mathf.Sign(Input.GetAxisRaw("Horizontal")) == Mathf.Sign(direction.x);
        bool isGrounded = playerMovement.IsGrounded();

        if (isTouchingWall && !isGrounded && wantsToCling)
        {
            if (!isClinging)
            {
                wallNormal = hit.normal;
                isClinging = true;
                canWallJump = true;

                playerMovement.ResetCoyoteTime();
            }
        }
        else
        {
            isClinging = false;
        }

        if (isClinging)
        {
            float newY = Mathf.MoveTowards(rb.velocity.y, -slideSpeed, slideAcceleration * Time.deltaTime);
            rb.velocity = new Vector2(rb.velocity.x, newY);

            if (Input.GetButtonDown("Jump") && canWallJump)
            {
                Vector2 jumpDir = new Vector2(-wallNormal.x * wallJumpForceX, wallJumpForceY);
                playerMovement.RequestExternalJump(jumpDir.y);
                rb.velocity = new Vector2(jumpDir.x, rb.velocity.y);

                isClinging = false;
                canWallJump = false;
            }

            float inputDir = Input.GetAxisRaw("Horizontal");
            if (Mathf.Sign(inputDir) == -Mathf.Sign(wallNormal.x) || inputDir == 0)
            {
                isClinging = false;
            }
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawLine(transform.position, transform.position + Vector3.right * wallCheckDistance);
        Gizmos.DrawLine(transform.position, transform.position + Vector3.left * wallCheckDistance);
    }
}


