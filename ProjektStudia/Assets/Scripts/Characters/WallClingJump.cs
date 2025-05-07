using UnityEngine;

public class WallClingJump : MonoBehaviour
{
    [Header("Wall Cling Settings")]
    public LayerMask wallLayer;
    public float wallCheckDistance = 0.3f;
    public float maxClingTime = 1.5f;
    public float slideSpeed = 2.5f;
    public float slideAcceleration = 3f;
    public float wallJumpForceX = 10f;
    public float wallJumpForceY = 14f;

    private Rigidbody2D rb;
    private PlayerMovement playerMovement;

    private bool isTouchingWall;
    private bool isClinging;
    private float clingTimer;
    private Vector2 wallNormal;
    private bool canCling = true;
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

        if (isTouchingWall && !isGrounded && wantsToCling && canCling)
        {
            if (!isClinging)
            {
                wallNormal = hit.normal;
                isClinging = true;
                clingTimer = 0f;
            }
        }
        else
        {
            isClinging = false;
        }

        if (isClinging)
        {
            clingTimer += Time.deltaTime;

            if (clingTimer > maxClingTime)
            {
                float newY = Mathf.MoveTowards(rb.velocity.y, -slideSpeed, slideAcceleration * Time.deltaTime);
                rb.velocity = new Vector2(rb.velocity.x, newY);
            }
            else
            {
                rb.velocity = new Vector2(rb.velocity.x, 0f);
            }

            if (Input.GetButtonDown("Jump") && canWallJump)
            {
                Vector2 jumpDir = new Vector2(-wallNormal.x * wallJumpForceX, wallJumpForceY);
                playerMovement.RequestExternalJump(jumpDir.y);
                rb.velocity = new Vector2(jumpDir.x, rb.velocity.y);
                isClinging = false;
                canCling = false;
                canWallJump = false;

                Invoke(nameof(ResetCling), 0.2f);
                Invoke(nameof(ResetWallJump), 0.1f);
            }

            if (Mathf.Sign(Input.GetAxisRaw("Horizontal")) == -Mathf.Sign(wallNormal.x))
            {
                isClinging = false;
            }
        }
    }

    void ResetCling()
    {
        canCling = true;
    }

    void ResetWallJump()
    {
        canWallJump = true;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawLine(transform.position, transform.position + Vector3.right * wallCheckDistance);
        Gizmos.DrawLine(transform.position, transform.position + Vector3.left * wallCheckDistance);
    }
}


