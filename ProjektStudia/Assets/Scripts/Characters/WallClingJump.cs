using UnityEngine;

public class WallClingJump : MonoBehaviour
{
    public Transform wallCheck;
    public float wallCheckRadius = 0.2f;
    public LayerMask wallLayer;
    public float slideSpeed = 2.5f;
    public float wallJumpForceX = 10f;
    public float wallJumpForceY = 14f;

    private Rigidbody2D rb;
    private PlayerMovement playerMovement;

    private bool isTouchingWall;
    private bool isGrounded;
    private bool canWallJump = true;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        playerMovement = GetComponent<PlayerMovement>();
    }

    void Update()
    {
        isGrounded = playerMovement.IsGrounded();
        isTouchingWall = Physics2D.OverlapCircle(wallCheck.position, wallCheckRadius, wallLayer);

        Debug.DrawRay(wallCheck.position, Vector2.left * wallCheckRadius, Color.red);
        Debug.DrawRay(wallCheck.position, Vector2.right * wallCheckRadius, Color.green);

        Debug.Log($"[DEBUG] TouchingWall: {isTouchingWall}, Grounded: {isGrounded}, Horizontal: {playerMovement.horizontal}, VelocityY: {rb.velocity.y}");

        if (isTouchingWall && !isGrounded && rb.velocity.y <= 0f && Mathf.Abs(playerMovement.horizontal) > 0.1f)
        {
            rb.velocity = new Vector2(rb.velocity.x, -slideSpeed);
            canWallJump = true;
            Debug.Log("[CLING] Zsuwanie po œcianie");

            if (Input.GetButtonDown("Jump") && canWallJump)
            {
                float jumpDir = playerMovement.horizontal > 0 ? -1f : 1f;
                rb.velocity = new Vector2(jumpDir * wallJumpForceX, wallJumpForceY);
                canWallJump = false;
                Debug.Log("[CLING] Wykonano wall jump");
            }
        }

        if (isGrounded)
        {
            canWallJump = true;
        }
    }

    void OnDrawGizmosSelected()
    {
        if (wallCheck != null)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(wallCheck.position, wallCheckRadius);
        }
    }
}


