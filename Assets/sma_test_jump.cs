using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class sma_test_jump : MonoBehaviour
{
    public float jumpForce = 5f; // Force applied when jumping
    public float tapJumpForce = 7f; // Force applied for a tap jump
    public float jumpHoldDuration = 0.15f; // Duration the jump button must be held for a higher jump
    public Transform groundCheck; // Ground check object
    public float groundCheckRadius = 0.1f; // Radius for ground check
    public LayerMask groundLayer; // Layer(s) representing the ground

    private Rigidbody2D rb;
    private bool isGrounded;
    private bool isJumping;
    private float jumpTimeCounter;


    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    } 

    private void Update()
    {
        // Check for ground contact
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
        
        
        
        // Jump when grounded and jump input is detected
        if (isGrounded && Input.GetButtonDown("Jump"))
        {
            StartJump();
        }

        // Continue jumping if jump button is held
        if (Input.GetButton("Jump") && isJumping)
        {
            ContinueJump();
        }

        // Stop jumping if jump button is released or jump time is over
        if (Input.GetButtonUp("Jump") || jumpTimeCounter <= 0)
        {
            StopJump();
        }
    }

    private void StartJump()
    {
        isJumping = true;
        jumpTimeCounter = jumpHoldDuration;

        // Apply initial jump force based on whether the jump button is tapped or held
        if (Input.GetButtonDown("Jump"))
        {
            rb.velocity = new Vector2(rb.velocity.x, tapJumpForce);
        }
    }

    private void ContinueJump()
    {
        // Apply additional force while the jump button is held
        if (jumpTimeCounter > 0)
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
            jumpTimeCounter -= Time.deltaTime;
        }
    }

    private void StopJump()
    {
        isJumping = false;
        jumpTimeCounter = 0;
    }
}
