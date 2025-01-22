using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private float horizontal;
    public float speed = 16f;
    private bool isFacingRight = true;
    AudioSource audioSource;
    Rigidbody2D rb2D;
    float x;

    [SerializeField] private float verticalJumpSpeed = 10f; 
    [SerializeField] private float horizontalJumpDistance = 8f; 
    [SerializeField] private float fallAcceleration = 2.5f;
    [SerializeField] private float airAcceleration = 1f;
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private Animator animator;

    [SerializeField] private GameObject sideView;
    [SerializeField] private GameObject frontView;

    private bool isJumping = false;
    private bool wasGrounded = true;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        rb2D = GetComponent<Rigidbody2D>();
    }
    
    void Update()
    {
        horizontal = Input.GetAxisRaw("Horizontal");
        animator.SetFloat("Speed", Mathf.Abs(horizontal));

        if (rb2D.velocity.x != 0 && IsGrounded())
        {
            if (!audioSource.isPlaying)
            {
                audioSource.Play();
            }
        }
        else
        {
            audioSource.Stop();
        }

        if (Input.GetButtonDown("Jump") && IsGrounded())
        {
            Debug.Log("Skok rozpoczêty");
            rb.velocity = new Vector2(horizontal * horizontalJumpDistance, verticalJumpSpeed); 
            isJumping = true;
            animator.SetBool("IsJumping", true);
            animator.SetBool("IsFalling", false);
            UpdateVisibility();
        }

        if (IsGrounded())
        {
            if (isJumping || animator.GetBool("IsFalling"))
            {
                Debug.Log("Postaæ wyl¹dowa³a");
                isJumping = false;
                animator.SetBool("IsJumping", false);
                animator.SetBool("IsFalling", false);
                animator.speed = 1f;

                rb.velocity = new Vector2(rb.velocity.x, 0f);
            }
        }
        else
        {
            if (rb.velocity.y > 0)
            {
                Debug.Log("Postaæ wznosi siê");
                animator.SetBool("IsJumping", true);
                animator.SetBool("IsFalling", false);
                animator.speed = 1f;
            }
            else if (rb.velocity.y < 0)
            {
                Debug.Log("Postaæ spada");
                ApplyFallAcceleration();
                animator.SetBool("IsJumping", false);
                animator.SetBool("IsFalling", true);
                animator.speed = 1f;
            }
        }

        wasGrounded = IsGrounded();
        Flip();
        UpdateVisibility();
    }

    public bool IsGrounded()
    {
        return Physics2D.OverlapCircle(groundCheck.position, 0.2f, groundLayer) && rb.velocity.y <= 0f;
    }

    private void FixedUpdate()
    {
        if (IsGrounded())
        {
            rb.velocity = new Vector2(horizontal * speed, rb.velocity.y);
        }
        else
        {
            rb.velocity = new Vector2(Mathf.Lerp(rb.velocity.x, horizontal * speed, airAcceleration * Time.deltaTime), rb.velocity.y);
        }
    }

    private void ApplyFallAcceleration()
    {
        rb.velocity += Vector2.up * Physics2D.gravity.y * (fallAcceleration - 1) * Time.deltaTime;
    }

    private void Flip()
    {
        if ((horizontal < 0f && isFacingRight) || (horizontal > 0f && !isFacingRight))
        {
            isFacingRight = !isFacingRight;
            Vector3 localScale = transform.localScale;
            localScale.x = Mathf.Abs(localScale.x) * (isFacingRight ? 1 : -1);
            transform.localScale = localScale;
        }
    }

    private void UpdateVisibility()
    {
        if (Mathf.Abs(horizontal) > 0)
        {
            sideView.SetActive(true);
            frontView.SetActive(false);
        }
        else if (IsGrounded())
        {
            sideView.SetActive(false);
            frontView.SetActive(true);
        }
        else
        {
            sideView.SetActive(true);
            frontView.SetActive(false);
        }
    }
}
