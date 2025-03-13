using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float horizontal { get; private set; }
    private bool isFalling;
    private float coyoteTimeCounter;
    private float jumpBufferCounter;
    private bool isFacingRight = true;
    private float fallTime = 0f;

    [Header("Movement Settings")]
    public float speed = 25f;
    public float acceleration = 30f;
    public float deceleration = 35f;
    public float airAcceleration = 15f;
    public float airDeceleration = 18f;

    [Header("Jump Settings")]
    public float jumpForce = 35f;
    public float ascentSpeedMultiplier = 1.5f;
    public float baseFallMultiplier = 5f;
    public float maxFallMultiplier = 15f;
    public float fallAccelerationRate = 3f;
    public float lowJumpMultiplier = 6f;
    public float maxFallSpeed = -50f;
    public float coyoteTime = 0.2f;
    public float jumpBufferTime = 0.2f;
    public float peakPauseTime = 0.1f;
    public float peakSpeedReduction = 0.8f;
    public float horizontalJumpReduction = 0.6f;

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
        HandleJumpInput(); // Obs³uguje wykrywanie skoku
        Flip(); // Obraca postaæ w zale¿noœci od kierunku ruchu
    }

    private void FixedUpdate()
    {
        ApplyMovement(); // Obs³uguje ruch poziomy
        ApplyJumpPhysics(); // Kontroluje fizykê skoku i opadania
    }

    // Sprawdza wejœcie gracza dotycz¹ce skoku i stosuje mechanizmy u³atwiaj¹ce skok (coyote time, jump buffer)
    private void HandleJumpInput()
    {
        if (IsGrounded())
        {
            coyoteTimeCounter = coyoteTime; // Resetuje czas na dodatkowy skok po zejœciu z platformy
            isFalling = false;
            fallTime = 0f;
        }
        else
        {
            coyoteTimeCounter -= Time.deltaTime;
        }

        if (Input.GetButtonDown("Jump"))
        {
            jumpBufferCounter = jumpBufferTime; // Aktywuje buforowanie skoku
            Debug.Log("Jump Buffer Activated! Timer: " + jumpBufferCounter);
        }
        else
        {
            jumpBufferCounter -= Time.deltaTime;
        }

        if (jumpBufferCounter > 0 && coyoteTimeCounter > 0)
        {
            Jump(); // Wywo³uje skok
            jumpBufferCounter = 0;
        }
    }

    // Wykonuje skok - ustawia prêdkoœæ pionow¹ i zmniejsza poziom¹, jeœli trzeba
    private void Jump()
    {
        float modifiedJumpForce = jumpForce * ascentSpeedMultiplier;
        float modifiedHorizontalVelocity = rb.velocity.x * horizontalJumpReduction;

        rb.velocity = new Vector2(modifiedHorizontalVelocity, modifiedJumpForce);
        animator.SetBool("IsJumping", true);
        coyoteTimeCounter = 0;

        Debug.Log("Jump! Modified Jump Force: " + modifiedJumpForce + " | Modified Horizontal Velocity: " + modifiedHorizontalVelocity);
    }

    private void ApplyMovement()
    {
        float targetSpeed = horizontal * speed;
        float accelerationRate = IsGrounded() ? (Mathf.Abs(horizontal) > 0.1f ? acceleration : deceleration) : airAcceleration;

        if (!IsGrounded())
        {
            accelerationRate = Mathf.Abs(horizontal) > 0.1f ? airAcceleration : airDeceleration;
        }

        rb.velocity = new Vector2(Mathf.Lerp(rb.velocity.x, targetSpeed, accelerationRate * Time.fixedDeltaTime), rb.velocity.y);
    }

    // Kontroluje opadanie - im d³u¿ej spadamy, tym szybciej opadamy
    private void ApplyJumpPhysics()
    {
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
            Debug.Log("Falling: " + rb.velocity.y + " | Fall Time: " + fallTime + " | Fall Multiplier: " + currentFallMultiplier);
        }
        else if (rb.velocity.y > 0 && !Input.GetButton("Jump"))
        {
            rb.velocity += Vector2.up * Physics2D.gravity.y * (lowJumpMultiplier - 1) * Time.fixedDeltaTime; // Skraca skok jeœli gracz puœci przycisk
        }
        else
        {
            fallTime = 0f;
        }
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
