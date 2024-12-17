using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class DoubleJump : MonoBehaviour
{
    [SerializeField] private float jumpPower = 14f; 
    [SerializeField] private float doubleJumpPower = 12f; 
    [SerializeField] private Transform groundCheck; 
    [SerializeField] private LayerMask groundLayer; 

    private Rigidbody2D rb;
    private bool canDoubleJump = false; 

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        bool isGrounded = IsGrounded();

        if (isGrounded)
        {
            canDoubleJump = true;
        }

        if (Input.GetButtonDown("Jump"))
        {
            if (isGrounded)
            {
                PerformJump(jumpPower); 
            }
            else if (canDoubleJump)
            {
                PerformJump(doubleJumpPower); 
                canDoubleJump = false; 
            }
        }
    }

    private void PerformJump(float power)
    {
        rb.velocity = new Vector2(rb.velocity.x, power); 
    }

    private bool IsGrounded()
    {
        return Physics2D.OverlapCircle(groundCheck.position, 0.2f, groundLayer);
    }
}