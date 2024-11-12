using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private float horizontal;
    private float speed = 8f;
    private float airControlFactor = 1f;
    private float airDragFactor = 0.98f;
    private bool isFacingRight = true;

    [SerializeField] private float jumpingPower = 14f;
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private LayerMask groundLayer;

    void Update()
    {
        horizontal = Input.GetAxisRaw("Horizontal");

        if (Input.GetButtonDown("Jump") && IsGrounded())
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpingPower);
        }

        if (Input.GetButtonUp("Jump") && rb.velocity.y > 0f)
        {
            rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y * 0.5f);
        }

        Flip();
    }

    public bool IsGrounded()
    {
        bool grounded = Physics2D.OverlapCircle(groundCheck.position, 0.2f, groundLayer);
        Debug.Log("IsGrounded: " + grounded);
        return grounded;
    }

    private void FixedUpdate()
    {
        if (IsGrounded())
        {
            rb.velocity = new Vector2(horizontal * speed, rb.velocity.y);
        }
        else
        {
            float targetHorizontalSpeed = horizontal * speed * airControlFactor;
            float currentHorizontalSpeed = rb.velocity.x;

            if (horizontal == 0)
            {
                rb.velocity = new Vector2(currentHorizontalSpeed * airDragFactor, rb.velocity.y);
            }
            else
            {
                rb.velocity = new Vector2(targetHorizontalSpeed, rb.velocity.y);
            }
        }
    }

    private void Flip()
    {
        if (isFacingRight && horizontal < 0f || !isFacingRight && horizontal > 0f)
        {
            isFacingRight = !isFacingRight;
            Vector3 localScale = transform.localScale;
            localScale.x = Mathf.Abs(localScale.x) * (isFacingRight ? 1 : -1);
            transform.localScale = localScale;
        }
    }
}