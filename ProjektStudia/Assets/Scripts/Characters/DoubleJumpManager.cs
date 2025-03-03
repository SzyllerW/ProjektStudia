using UnityEngine;

public class DoubleJumpManager : MonoBehaviour
{
    [Header("Double Jump Settings")]
    public int maxJumps = 2; // Ilo�� mo�liwych skok�w (1 = zwyk�y skok, 2 = podw�jny skok)
    public float doubleJumpForce = 30f; // Si�a drugiego skoku
    public AudioClip doubleJumpSound; // Opcjonalny d�wi�k podw�jnego skoku

    private int jumpCount = 0;
    private bool isGrounded;
    private Rigidbody2D rb;
    private PlayerMovement playerMovement;
    private AudioSource audioSource;

    [SerializeField] private Transform groundCheck;
    [SerializeField] private LayerMask groundLayer;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        playerMovement = GetComponent<PlayerMovement>();
        audioSource = GetComponent<AudioSource>();
    }

    void Update()
    {
        CheckGrounded();
        HandleDoubleJump();
    }

    private void CheckGrounded()
    {
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, 0.2f, groundLayer);
        if (isGrounded)
        {
            jumpCount = 0; // Resetujemy ilo�� skok�w po wyl�dowaniu
        }
    }

    private void HandleDoubleJump()
    {
        if (Input.GetButtonDown("Jump"))
        {
            if (isGrounded || jumpCount < maxJumps - 1)
            {
                PerformJump();
            }
        }
    }

    private void PerformJump()
    {
        jumpCount++;
        float jumpForce = (jumpCount == 1) ? playerMovement.jumpForce : doubleJumpForce;
        rb.velocity = new Vector2(rb.velocity.x, jumpForce);

        if (jumpCount > 1 && doubleJumpSound)
        {
            audioSource.PlayOneShot(doubleJumpSound);
        }

        Debug.Log("Jump performed! Jump count: " + jumpCount);
    }
}
