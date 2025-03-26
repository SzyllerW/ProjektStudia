using UnityEngine;

public class DoubleJumpManager : MonoBehaviour
{
    [Header("Double Jump Settings")]
    public int maxJumps = 2;
    public float doubleJumpForce = 25f;

    private int jumpCount = 0;
    private bool isGrounded;
    private Rigidbody2D rb;
    private AudioSource audioSource;

    [SerializeField] private Transform groundCheck;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private AudioClip doubleJumpSound;
    [SerializeField] private float doubleJumpVolume = 1f;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        audioSource = GetComponent<AudioSource>();
    }

    private void Update()
    {
        CheckGrounded();

        if (Input.GetButtonDown("Jump") && jumpCount < maxJumps)
        {
            if (!isGrounded)
            {
                PerformDoubleJump();
            }
        }
    }

    private void CheckGrounded()
    {
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, 0.2f, groundLayer);
        if (isGrounded)
        {
            jumpCount = 0;
        }
    }

    private void PerformDoubleJump()
    {
        jumpCount++;

        float force = GetComponent<PlayerMovement>().jumpForce * GetComponent<PlayerMovement>().ascentSpeedMultiplier;

        rb.velocity = new Vector2(rb.velocity.x, 0f);
        rb.AddForce(Vector2.up * force, ForceMode2D.Impulse);

        if (doubleJumpSound && audioSource)
        {
            audioSource.PlayOneShot(doubleJumpSound, doubleJumpVolume);
        }

    }
}


