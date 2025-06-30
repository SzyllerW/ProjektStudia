using UnityEngine;
using System.Diagnostics;

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
    private Vector2 platformVelocity = Vector2.zero;

    [HideInInspector] public bool wasLaunchedFromMound = false;
    private bool ignoreJumpRelease = false;

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

    private WallClingJump wallClingJump;

    [HideInInspector]
    public bool removeControl
    {
        get => !playerHasControl;
        set
        {
            playerHasControl = !value;

            StackTrace stackTrace = new StackTrace();
            string className = stackTrace.GetFrame(1)?.GetMethod()?.DeclaringType?.Name;
            UnityEngine.Debug.LogWarning($"{className} has removed control from the player");
        }
    }

    void Start()
    {
        rb.gravityScale = 5;
        wallClingJump = GetComponent<WallClingJump>();
    }

    void Update()
    {
        if (!playerHasControl) return;

        horizontal = Input.GetAxisRaw("Horizontal");
        animator.SetFloat("Speed", Mathf.Abs(horizontal));

        HandleJumpInput();
        Flip();
        UpdateJumpAnimation();
        animator.SetBool("IsGrounded", IsGrounded());

        trailTargetTime = IsGrounded() ? 0f : 1f;
        trailRenderer.time = Mathf.Lerp(trailRenderer.time, trailTargetTime, Time.deltaTime * 2f);

        if (IsGrounded() && wasLaunchedFromMound)
        {
            wasLaunchedFromMound = false;
        }

        bool shouldPlayWalk = IsGrounded() && Mathf.Abs(horizontal) > 0.1f;

        if (shouldPlayWalk && !isPlayingWalkingSound)
        {
            walkingAudioSource.loop = true;
            walkingAudioSource.Play();
            isPlayingWalkingSound = true;
        }
        else if (!shouldPlayWalk && isPlayingWalkingSound)
        {
            walkingAudioSource.Stop();
            isPlayingWalkingSound = false;
        }
    }

    private void FixedUpdate()
    {
        ApplyMovement();

        if (externalJumpRequested)
        {
            float modifiedHorizontalVelocity = rb.velocity.x * horizontalJumpReduction;

            rb.velocity = new Vector2(modifiedHorizontalVelocity, rb.velocity.y);
            rb.velocity += Vector2.up * externalJumpForce;

            UnityEngine.Debug.Log($"[FIXEDUPDATE] External jump executed. New Velocity: {rb.velocity}");

            coyoteTimeCounter = 0;
            externalJumpRequested = false;
            return;
        }

        ApplyJumpPhysics();
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
            UnityEngine.Debug.Log("[INPUT] Jump button pressed");
            if (IsGrounded()) ResetTrailRenderer();
        }
        else
        {
            jumpBufferCounter -= Time.deltaTime;
        }

        if (jumpBufferCounter > 0 &&
            (coyoteTimeCounter > 0 || (wallClingJump != null && wallClingJump.IsClinging)) &&
            !wasLaunchedFromMound)
        {
            UnityEngine.Debug.Log("[JUMP] Conditions met: Coyote=" + coyoteTimeCounter + ", Clinging=" + wallClingJump?.IsClinging);
            Jump();
            jumpBufferCounter = 0;
            SoundFXManager.instance.PlaySoundFXClip(jumpSound, transform, jumpVolume);
        }
    }

    private void Jump()
    {
        float modifiedJumpForce = jumpForce * ascentSpeedMultiplier;
        float modifiedHorizontalVelocity = rb.velocity.x * horizontalJumpReduction;
        rb.velocity = new Vector2(modifiedHorizontalVelocity, 0f);
        rb.velocity += Vector2.up * modifiedJumpForce;
        coyoteTimeCounter = 0;
        UnityEngine.Debug.Log("[JUMP] Jump executed");
    }

    private void ApplyMovement()
    {
        if (!playerHasControl || (wallClingJump != null && wallClingJump.IsClinging)) return;

        float targetSpeed = horizontal * (IsGrounded() ? speed : jumpHorizontalSpeed);
        float accelerationRate = IsGrounded() ? (Mathf.Abs(horizontal) > 0.1f ? acceleration : deceleration) : airAcceleration;

        if (!IsGrounded())
            accelerationRate = Mathf.Abs(horizontal) > 0.1f ? airAcceleration : airDeceleration;

        float newX = Mathf.Lerp(rb.velocity.x, targetSpeed, accelerationRate * Time.fixedDeltaTime);
        rb.velocity = new Vector2(newX + platformVelocity.x, rb.velocity.y);
    }

    private void ApplyJumpPhysics()
    {
        if (wallClingJump != null && wallClingJump.IsClinging) return;

        if (rb.velocity.y < 0)
        {
            ignoreJumpRelease = false; // reset po rozpoczêciu spadania

            fallTime += Time.fixedDeltaTime;
            float currentFallMultiplier = Mathf.Clamp(baseFallMultiplier + (fallTime * fallAccelerationRate), baseFallMultiplier, maxFallMultiplier);

            rb.velocity += Vector2.up * Physics2D.gravity.y * (currentFallMultiplier - 1) * Time.fixedDeltaTime;

            if (rb.velocity.y < maxFallSpeed)
                rb.velocity = new Vector2(rb.velocity.x, maxFallSpeed);

            isFalling = true;
        }
        else if (rb.velocity.y > 0 && !Input.GetButton("Jump") && !ignoreJumpRelease)
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
        bool isClinging = wallClingJump != null && wallClingJump.IsClinging;
        bool grounded = IsGrounded();

        if (isClinging) return;

        animator.SetBool("IsJumping", !grounded);
    }

    private void Flip()
    {
        if ((horizontal < 0f && isFacingRight) || (horizontal > 0f && !isFacingRight))
        {
            isFacingRight = !isFacingRight;
            transform.localScale = new Vector3(isFacingRight ? 1 : -1, 1, 1);
        }
    }

    public bool IsGrounded()
    {
        Collider2D groundCollider = Physics2D.OverlapCircle(groundCheck.position, 0.2f, groundLayer);
        bool grounded = groundCollider != null;
        UnityEngine.Debug.DrawRay(groundCheck.position, Vector2.down * 0.2f, grounded ? Color.green : Color.red);

        if (grounded)
        {
            PlatformVelocity pv = groundCollider.GetComponentInParent<PlatformVelocity>();
            if (pv != null)
                platformVelocity = pv.Velocity;
            else
                platformVelocity = Vector2.zero;
        }
        else
        {
            platformVelocity = Vector2.zero;
        }

        return grounded;
    }

    private void ResetTrailRenderer()
    {
        trailRenderer.Clear();
    }

    public void ResetCoyoteTime()
    {
        coyoteTimeCounter = coyoteTime;
    }

    public void RequestExternalJump(float force, bool allowHold)
    {
        externalJumpRequested = true;
        externalJumpForce = force;
        ignoreJumpRelease = !allowHold;
        UnityEngine.Debug.Log("[REQUEST JUMP] Requested external jump with force: " + force);
    }
}

