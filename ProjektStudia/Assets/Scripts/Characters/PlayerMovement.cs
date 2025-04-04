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
    private bool isPlayingWalkingSound = false;
    private bool externalJumpRequested = false;
    private float externalJumpForce = 0f;

    [Header("Movement Settings")]
    public float speed = 45f;
    public float acceleration = 30f;
    public float deceleration = 35f;
    public float airAcceleration = 15f;
    public float airDeceleration = 18f;

    [Header("Jump Movement Settings")]
    public float jumpHorizontalSpeed = 20f;

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
    [SerializeField] private AudioSource walkingAudioSource;
    [SerializeField] private AudioClip impactSound;
    [SerializeField] private float impactVolume = 1f;
    [SerializeField] private AudioClip jumpSound;
    [SerializeField] private float jumpVolume = 1f;

    [SerializeField] private Rigidbody2D currentPlatformRb;

    void Start()
    {
        rb.gravityScale = 5;
    }

    void Update()
    {
        if (!playerHasControl) return;

        horizontal = Input.GetAxisRaw("Horizontal");
        animator.SetFloat("Speed", Mathf.Abs(horizontal));
        HandleJumpInput();
        Flip();
        UpdateJumpAnimation();
        HandleWalkingSound();

        trailTargetTime = IsGrounded() ? 0f : 1f;
        trailRenderer.time = Mathf.Lerp(trailRenderer.time, trailTargetTime, Time.deltaTime * 2f);
    }

    private void FixedUpdate()
    {
        ApplyMovement();
        ApplyJumpPhysics();

        if (externalJumpRequested)
        {
            float modifiedHorizontalVelocity = rb.velocity.x * horizontalJumpReduction;
            rb.velocity = new Vector2(modifiedHorizontalVelocity, externalJumpForce);
            animator.SetBool("IsJumping", true);
            coyoteTimeCounter = 0;

            externalJumpRequested = false;
        }
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
            if (IsGrounded()) ResetTrailRenderer();
        }
        else
        {
            jumpBufferCounter -= Time.deltaTime;
        }

        if (jumpBufferCounter > 0 && coyoteTimeCounter > 0)
        {
            Jump();
            jumpBufferCounter = 0;
            SoundFXManager.instance.PlaySoundFXClip(jumpSound, transform, jumpVolume);
        }
    }

    private void Jump()
    {
        Debug.Log(" Jump() wywo³any!");
        float platformVerticalVelocity = 0f;
        if (currentPlatformRb != null)
            platformVerticalVelocity = currentPlatformRb.velocity.y;

        float modifiedJumpForce = jumpForce * ascentSpeedMultiplier;

        if (platformVerticalVelocity < -0.1f)
        {
            modifiedJumpForce = Mathf.Min(modifiedJumpForce, 25f);
        }

        float modifiedHorizontalVelocity = rb.velocity.x * horizontalJumpReduction;

        if (transform.parent != null)
            transform.SetParent(null);

        rb.velocity = new Vector2(modifiedHorizontalVelocity, 0f); 
        rb.velocity += Vector2.up * modifiedJumpForce;

        coyoteTimeCounter = 0;

        Debug.Log("Platform velocity Y: " + platformVerticalVelocity);
        Debug.Log("Modified jump force: " + modifiedJumpForce);
    }

    private void ApplyMovement()
    {
        if (!playerHasControl) return;

        float targetSpeed = horizontal * (IsGrounded() ? speed : jumpHorizontalSpeed);
        float accelerationRate = IsGrounded() ? (Mathf.Abs(horizontal) > 0.1f ? acceleration : deceleration) : airAcceleration;

        if (!IsGrounded())
            accelerationRate = Mathf.Abs(horizontal) > 0.1f ? airAcceleration : airDeceleration;

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
                rb.velocity = new Vector2(rb.velocity.x, maxFallSpeed);

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
        animator.SetBool("IsJumping", !IsGrounded());
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
        Collider2D groundCollider = Physics2D.OverlapCircle(groundCheck.position, 0.2f, groundLayer);
        bool grounded = groundCollider != null;
        Debug.DrawRay(groundCheck.position, Vector2.down * 0.2f, grounded ? Color.green : Color.red);
        Debug.Log("Penguin IsGrounded: " + grounded);

        if (grounded && rb.velocity.y <= maxFallSpeed && !hasPlayedImpactAnimation && (groundCollider == null || !groundCollider.CompareTag("DirtMound")))
        {
            PlayImpactAnimation();
            hasPlayedImpactAnimation = true;
        }
        else if (!grounded)
        {
            hasPlayedImpactAnimation = false;
        }

        return grounded;
    }

    private void PlayImpactAnimation()
    {
        animator.SetBool("Impact", true);
        playerHasControl = false;
        SoundFXManager.instance.PlaySoundFXClip(impactSound, transform, impactVolume);
    }

    private void HandleWalkingSound()
    {
        if (Mathf.Abs(horizontal) > 0.1f && IsGrounded() && !animator.GetBool("Impact"))
        {
            if (!walkingAudioSource.isPlaying)
                walkingAudioSource.Play();
        }
        else
        {
            if (walkingAudioSource.isPlaying)
                walkingAudioSource.Stop();
        }
    }

    private void ResetTrailRenderer()
    {
        trailRenderer.Clear();

    }

    public void RestoreControl()
    {
        animator.SetBool("Impact", false);
        playerHasControl = true;
    }

    public void ExternalJump(float force)
    {
        Debug.Log(" RequestExternalJump() force: " + force);
        float modifiedHorizontalVelocity = rb.velocity.x * horizontalJumpReduction;
        rb.velocity = new Vector2(modifiedHorizontalVelocity, force);
        animator.SetBool("IsJumping", true);
        coyoteTimeCounter = 0;
    }

    public void RequestExternalJump(float force)
    {
        externalJumpRequested = true;
        externalJumpForce = force;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("MovingPlatform"))
        {
            currentPlatformRb = collision.rigidbody;
            Debug.Log(" Player stepped on platform: " + currentPlatformRb.name);
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("MovingPlatform"))
        {
            currentPlatformRb = null;
        }
    }

    public void ResetAfterRespawn()
    {
        Transform rp = GameObject.Find("RespawnPoint")?.transform;
        if (rp != null)
        {
            transform.position = rp.position + Vector3.up * 0.05f;
            Debug.Log(" PlayerMovement — Reset position to: " + transform.position);
        }

        // Zresetuj fizykê
        rb.velocity = Vector2.zero;
        rb.angularVelocity = 0f;
        rb.isKinematic = false;
        rb.simulated = true;
        rb.gravityScale = 5;
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;

        // Zresetuj statusy
        trailRenderer.Clear();
        transform.SetParent(null);
        externalJumpRequested = false;
        externalJumpForce = 0f;
        isFalling = false;
        coyoteTimeCounter = coyoteTime;
        jumpBufferCounter = 0;
        hasPlayedImpactAnimation = false;
        playerHasControl = true;

        // Zresetuj animator
        animator.SetBool("IsJumping", false);
        animator.SetBool("Impact", false);
        animator.SetFloat("Speed", 0);

        Debug.Log(" Parent = " + (transform.parent != null ? transform.parent.name : "null"));
        Debug.Log(" localPosition = " + transform.localPosition);
        Debug.Log(" worldPosition = " + transform.position);
    }
}
