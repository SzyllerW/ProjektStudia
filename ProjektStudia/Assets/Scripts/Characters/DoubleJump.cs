using UnityEngine;

[RequireComponent(typeof(PlayerMovement))]
public class DoubleJump : MonoBehaviour
{
    private PlayerMovement playerMovement;
    private Rigidbody2D rb;

    private bool canDoubleJump = false;
    private bool jumpKeyWasPressed = false; 
    [SerializeField] private float doubleJumpPower = 12f;

    void Start()
    {
        playerMovement = GetComponent<PlayerMovement>();
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
     
        if (playerMovement.IsGrounded())
        {
            canDoubleJump = true;
            jumpKeyWasPressed = false; 
        }


        if (Input.GetButtonDown("Jump") && !jumpKeyWasPressed)
        {
            if (!playerMovement.IsGrounded() && canDoubleJump)
            {
                rb.velocity = new Vector2(rb.velocity.x, doubleJumpPower);
                canDoubleJump = false; 
            }

            jumpKeyWasPressed = true; 
        }


        if (Input.GetButtonUp("Jump"))
        {
            jumpKeyWasPressed = false;
        }
    }
}