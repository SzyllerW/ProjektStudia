using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class DoubleJump : MonoBehaviour
{
    [SerializeField] private float jumpPower = 14f; // Si³a pierwszego skoku
    [SerializeField] private float doubleJumpPower = 12f; // Si³a podwójnego skoku
    [SerializeField] private Transform groundCheck; // Punkt sprawdzania ziemi
    [SerializeField] private LayerMask groundLayer; // Warstwa ziemi

    private Rigidbody2D rb;
    private bool canDoubleJump = false; // Czy podwójny skok jest mo¿liwy

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        // Sprawdzanie, czy postaæ jest na ziemi
        bool isGrounded = IsGrounded();

        // Reset podwójnego skoku po wyl¹dowaniu
        if (isGrounded)
        {
            canDoubleJump = true;
        }

        // Skok
        if (Input.GetButtonDown("Jump"))
        {
            if (isGrounded)
            {
                PerformJump(jumpPower); // Pierwszy skok
            }
            else if (canDoubleJump)
            {
                PerformJump(doubleJumpPower); // Podwójny skok
                canDoubleJump = false; // Zu¿ycie podwójnego skoku
            }
        }
    }

    private void PerformJump(float power)
    {
        rb.velocity = new Vector2(rb.velocity.x, power); // Ustaw pionow¹ prêdkoœæ skoku
    }

    private bool IsGrounded()
    {
        // Sprawdza, czy postaæ dotyka ziemi
        return Physics2D.OverlapCircle(groundCheck.position, 0.2f, groundLayer);
    }
}