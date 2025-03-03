using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float horizontal { get; private set; }
    private bool isJumping;
    private bool isFalling;
    private float coyoteTimeCounter;
    private float jumpBufferCounter;
    private bool isFacingRight = true;
    private float fallTime = 0f;
    private bool isAtPeak = false;

    [Header("Movement Settings")]
    public float speed = 25f;
    public float acceleration = 30f;
    public float deceleration = 35f;
    public float airAcceleration = 15f;
    public float airDeceleration = 18f;

    [Header("Jump Settings")]
    public float jumpForce = 35f;
    public float baseFallMultiplier = 5f;
    public float maxFallMultiplier = 15f;
    public float fallAccelerationRate = 3f;
    public float lowJumpMultiplier = 6f;
    public float maxFallSpeed = -50f;
    public float coyoteTime = 0.2f;
    public float jumpBufferTime = 0.2f;
    public float peakPauseTime = 0.1f;
    public float peakSpeedReduction = 0.5f; 

    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private Animator animator;

    void Start()
    {
        rb.gravityScale = 5;
    }

    void Update()
    {
        horizontal = Input.GetAxisRaw("Horizontal");
        animator.SetFloat("Speed", Mathf.Abs(horizontal));
        HandleJumpInput();
        Flip();
    }

    private void FixedUpdate()
    {
        ApplyMovement();
        ApplyJumpPhysics();
    }

    private void HandleJumpInput()
    {
        if (IsGrounded())
        {
            coyoteTimeCounter = coyoteTime;
            isFalling = false;
            fallTime = 0f;
        }
        else
        {
            coyoteTimeCounter -= Time.deltaTime;
        }

        if (Input.GetButtonDown("Jump"))
        {
            jumpBufferCounter = jumpBufferTime;
            Debug.Log("Jump Buffer Activated! Timer: " + jumpBufferCounter);
        }
        else
        {
            jumpBufferCounter -= Time.deltaTime;
        }

        if (jumpBufferCounter > 0 && coyoteTimeCounter > 0)
        {
            Jump();
            jumpBufferCounter = 0;
            Debug.Log("Jump Buffer Used! Skok wykonany.");
        }

        if (jumpBufferCounter <= 0 && jumpBufferCounter > -0.1f)
        {
            Debug.Log("Jump Buffer Expired!");
        }
    }

    private void Jump()
    {
        rb.velocity = new Vector2(rb.velocity.x, jumpForce);
        animator.SetBool("IsJumping", true);
        coyoteTimeCounter = 0;
        isJumping = true;
    }

    private void ApplyMovement()
    {
        float targetSpeed = horizontal * speed;
        float accelerationRate = IsGrounded() ? (Mathf.Abs(horizontal) > 0.1f ? acceleration : deceleration) : airAcceleration;

        if (!IsGrounded())
        {
            accelerationRate = Mathf.Abs(horizontal) > 0.1f ? airAcceleration : airDeceleration;
        }

        if (isAtPeak)
        {
            targetSpeed *= peakSpeedReduction; // Zmniejszamy prêdkoœæ poziom¹ na szczycie skoku
            Debug.Log("Peak Movement Activated! Speed Reduced: " + targetSpeed);
        }

        rb.velocity = new Vector2(Mathf.Lerp(rb.velocity.x, targetSpeed, accelerationRate * Time.fixedDeltaTime), rb.velocity.y);
    }

    private void ApplyJumpPhysics()
    {
        if (rb.velocity.y > 0 && rb.velocity.y < 1f) // Pauza na szczycie skoku
        {
            if (!isAtPeak)
            {
                isAtPeak = true;
                Debug.Log("Peak Reached! Applying Pause...");
                StartCoroutine(PeakPause());
            }
        }
        else
        {
            isAtPeak = false;
        }

        if (rb.velocity.y < 0)
        {
            fallTime += Time.fixedDeltaTime;
            float currentFallMultiplier = Mathf.Clamp(baseFallMultiplier + (fallTime * fallAccelerationRate), baseFallMultiplier, maxFallMultiplier);

            rb.velocity += Vector2.up * Physics2D.gravity.y * (currentFallMultiplier - 1) * Time.fixedDeltaTime;

            if (rb.velocity.y < maxFallSpeed)
            {
                rb.velocity = new Vector2(rb.velocity.x, maxFallSpeed);
            }

            isFalling = true;
        }
        else if (rb.velocity.y > 0 && !Input.GetButton("Jump"))
        {
            rb.velocity += Vector2.up * Physics2D.gravity.y * (lowJumpMultiplier - 1) * Time.fixedDeltaTime;
        }
        else
        {
            fallTime = 0f;
        }
    }

    private System.Collections.IEnumerator PeakPause()
    {
        float originalGravity = rb.gravityScale;
        rb.gravityScale = originalGravity * 0.2f; 
        yield return new WaitForSeconds(peakPauseTime);
        rb.gravityScale = originalGravity;
        Debug.Log("Peak Pause Ended! Gravity Restored.");
    }


    private void Flip()
    {
        if ((horizontal < 0f && isFacingRight) || (horizontal > 0f && !isFacingRight))
        {
            isFacingRight = !isFacingRight;
            transform.localScale = new Vector3(isFacingRight ? 1 : -1, 1, 1);
        }
    }

    private bool IsGrounded()
    {
        return Physics2D.OverlapCircle(groundCheck.position, 0.2f, groundLayer);
    }
}
