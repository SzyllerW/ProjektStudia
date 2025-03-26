using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float horizontal { get; private set; }
    private bool isFalling;
    private float coyoteTimeCounter;
    private float jumpBufferCounter;
    private bool isFacingRight = true;
    private float fallTime = 0f;
    private bool hasPlayedImpactAnimation = false;
    private bool playerHasControl = true;
    private float trailTargetTime = 0f;

    [Header("Movement Settings")]
    public float speed = 45f;
    public float acceleration = 30f;
    public float deceleration = 35f;
    public float airAcceleration = 15f;
    public float airDeceleration = 18f;

    [Header("Jump Movement Settings")]
    public float jumpHorizontalSpeed = 20f; // Prêdkoœæ pozioma w powietrzu, niezale¿na od biegania

    [Header("Jump Settings")]
    public float jumpForce = 35f;
    public float ascentSpeedMultiplier = 1.5f;
    public float baseFallMultiplier = 4f;
    public float maxFallMultiplier = 30f;
    public float fallAccelerationRate = 8f;
    public float lowJumpMultiplier = 6f;
    public float maxFallSpeed = -80f;
    public float coyoteTime = 0.1f;
    public float jumpBufferTime = 0.15f;
    public float peakPauseTime = 0.1f;
    public float peakSpeedReduction = 0.5f;
    public float horizontalJumpReduction = 0.6f;

    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private Animator animator;
    [SerializeField] private TrailRenderer trailRenderer;

    void Start()
    {
        rb.gravityScale = 5;
    }

    void Update()
    {
        if (!playerHasControl) return;

        horizontal = Input.GetAxisRaw("Horizontal");
        animator.SetFloat("Speed", Mathf.Abs(horizontal));
        HandleJumpInput(); // Obs³uguje wykrywanie skoku
        Flip(); // Obraca postaæ w zale¿noœci od kierunku ruchu
        UpdateJumpAnimation();

        // Ustawienia smugi
        if (IsGrounded())
        {
            trailTargetTime = 0f;
        }
        else
        {
            trailTargetTime = 1f;
        }

        // P³ynna zmiana d³ugoœci smugi
        trailRenderer.time = Mathf.Lerp(trailRenderer.time, trailTargetTime, Time.deltaTime * 2f);
    }

    private void FixedUpdate()
    {
        ApplyMovement(); // Obs³uguje ruch poziomy
        ApplyJumpPhysics(); // Kontroluje fizykê skoku i opadania
    }

    private void HandleJumpInput()
    {
        if (!playerHasControl) return;

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

            if (IsGrounded())
            {
                ResetTrailRenderer(); // Wyczyszczenie smugi
            }
        }
        else
        {
            jumpBufferCounter -= Time.deltaTime;
        }

        if (jumpBufferCounter > 0 && coyoteTimeCounter > 0)
        {
            Jump();
            jumpBufferCounter = 0;
        }
    }

    private void Jump()
    {
        float modifiedJumpForce = jumpForce * ascentSpeedMultiplier;
        float modifiedHorizontalVelocity = rb.velocity.x * horizontalJumpReduction; // Redukuje d³ugoœæ skoku

        rb.velocity = new Vector2(modifiedHorizontalVelocity, modifiedJumpForce);
        coyoteTimeCounter = 0;
    }

    private void ApplyMovement()
    {
        if (!playerHasControl) return;

        float targetSpeed = horizontal * (IsGrounded() ? speed : jumpHorizontalSpeed); // Oddzielna prêdkoœæ dla ziemi i powietrza
        float accelerationRate = IsGrounded() ? (Mathf.Abs(horizontal) > 0.1f ? acceleration : deceleration) : airAcceleration;

        if (!IsGrounded())
        {
            accelerationRate = Mathf.Abs(horizontal) > 0.1f ? airAcceleration : airDeceleration;
        }

        rb.velocity = new Vector2(Mathf.Lerp(rb.velocity.x, targetSpeed, accelerationRate * Time.fixedDeltaTime), rb.velocity.y);
    }

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

    private void UpdateJumpAnimation()
    {
        if (IsGrounded())
        {
            animator.SetBool("IsJumping", false);
        }
        else
        {
            animator.SetBool("IsJumping", true);
        }
    }

    private void PlayImpactAnimation()
    {
        animator.SetBool("Impact", true); // Wyzwolenie animacji uderzenia
        playerHasControl = false; // Odbiera kontrolê gracza nad postaci¹
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
        bool grounded = Physics2D.OverlapCircle(groundCheck.position, 0.2f, groundLayer);

        if (grounded)
        {
            // SprawdŸ, czy prêdkoœæ spadania osi¹gnê³a maxFallSpeed i animacja nie zosta³a odtworzona
            if (rb.velocity.y <= maxFallSpeed && !hasPlayedImpactAnimation)
            {
                PlayImpactAnimation(); // Wywo³aj animacjê uderzenia
                hasPlayedImpactAnimation = true; // Ustaw zmienn¹ na true po odtworzeniu animacji
            }
        }
        else
        {
            hasPlayedImpactAnimation = false; // Zresetuj zmienn¹, gdy postaæ opuœci ziemiê
        }

        return grounded;
    }

    public void RestoreControl()
    {
        animator.SetBool("Impact", false);
        playerHasControl = true; // Przywróæ kontrolê graczowi
    }

    private void ResetTrailRenderer()
    {
        trailRenderer.Clear(); // Usuñ wszystkie aktywne cz¹steczki smugi
    }
}