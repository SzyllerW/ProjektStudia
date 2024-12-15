using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class DoubleJump : MonoBehaviour
{
    [SerializeField] private float jumpPower = 14f; // Si�a pierwszego skoku
    [SerializeField] private float doubleJumpPower = 12f; // Si�a podw�jnego skoku
    [SerializeField] private Transform groundCheck; // Punkt sprawdzania ziemi
    [SerializeField] private LayerMask groundLayer; // Warstwa ziemi

    private Rigidbody2D rb;
    private bool canDoubleJump = false; // Czy podw�jny skok jest mo�liwy

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        // Sprawdzanie, czy posta� jest na ziemi
        bool isGrounded = IsGrounded();

        // Reset podw�jnego skoku po wyl�dowaniu
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
                PerformJump(doubleJumpPower); // Podw�jny skok
                canDoubleJump = false; // Zu�ycie podw�jnego skoku
            }
        }
    }

    private void PerformJump(float power)
    {
        rb.velocity = new Vector2(rb.velocity.x, power); // Ustaw pionow� pr�dko�� skoku
    }

    private bool IsGrounded()
    {
        // Sprawdza, czy posta� dotyka ziemi
        return Physics2D.OverlapCircle(groundCheck.position, 0.2f, groundLayer);
    }
}