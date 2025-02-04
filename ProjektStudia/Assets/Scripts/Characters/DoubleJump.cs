using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class DoubleJump : MonoBehaviour
{
    [SerializeField] private float jumpPower = 14f;
    [SerializeField] private float doubleJumpPower = 12f;
    [SerializeField] private float fallAcceleration = 2.5f; 
    [SerializeField] private Transform groundCheck;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private Animator animator;
    [SerializeField] private AudioClip jumpSoundClip;

    private Rigidbody2D rb;
    private bool canDoubleJump = false;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        animator.SetFloat("Speed", Mathf.Abs(rb.velocity.x));
        animator.SetBool("IsJumping", !IsGrounded() && rb.velocity.y > 0);
        animator.SetBool("IsFalling", !IsGrounded() && rb.velocity.y < 0);

        if (IsGrounded())
        {
            canDoubleJump = true;
            animator.SetBool("IsJumping", false);
            animator.SetBool("IsFalling", false);
        }

        if (Input.GetButtonDown("Jump"))
        {
            if (IsGrounded())
            {
                PerformJump(jumpPower);
                animator.SetBool("IsJumping", true);
                animator.Play("Jump", 0, 0f);
            }
            else if (canDoubleJump)
            {
                PerformJump(doubleJumpPower);
                canDoubleJump = false;
                animator.SetBool("IsJumping", true);
                animator.Play("Jump", 0, 0f);

                //play sound FX
                SoundFXManager.instance.PlaySoundFXClip(jumpSoundClip, transform, 0.7f);
            }
        }

        ApplyFallAcceleration();
    }

    private void PerformJump(float power)
    {
        rb.velocity = new Vector2(rb.velocity.x, power);
    }

    private bool IsGrounded()
    {
        return Physics2D.OverlapCircle(groundCheck.position, 0.2f, groundLayer);
    }

    private void ApplyFallAcceleration()
    {
        if (!IsGrounded() && rb.velocity.y < 0) 
        {
            rb.velocity += Vector2.up * Physics2D.gravity.y * (fallAcceleration - 1) * Time.deltaTime;
        }
    }
}