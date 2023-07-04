using System.Collections;
using UnityEngine;

public class PlayerMovementScript2 : MonoBehaviour
{
    private float horizontal;
    private float speed = 8f;
    public float jumpingPower = 16f;
    private bool isFacingRight = false;

    // wall jumping
    private bool isWallSliding;
    private float wallSlidingSpeed = 2f;
    private bool isWallJumping;
    private float wallJumpingDirection;
    private float wallJumpingTime = 0.2f;
    private float wallJumpingCounter;
    private float wallJumpingDuration = 0.4f;
    private Vector2 wallJumpingPower = new Vector2(8f, 16f);

    
    public float fallMultipler = 2.5f;
    public float lowJumpMultiplier = 2f;

    /* animation */
    private string _currentAnimation;
    const string PLAYER_IDLE = "player_idle";
    const string PLAYER_RUN = "player_running";
    const string PLAYER_ATTACK = "player_attack";
    const string PLAYER_JUMP = "player_jump";
    const string PLAYER_DASH = "player_dash";
    const string PLAYER_BLOCK = "player_blocking";

    /* dash i att mus */
    private Vector3 cursorPosition;
    public Vector2 cursorOffset;
    private Vector2 cursorDir;

    /* wall jump */
    private bool on_wall = false;

    /* Attack thingy majingy */
    private bool isAttacking = false;
    private float attackTime = 0.6f;

    /* Blocking */

    private bool isBlocking = false;

    [Header("Dash Settings")]
    /* dash */
    public float dashingPower = 35f;
    public float dashingTime = 0.1f;
    public float dashingCooldown = 1f;
    private bool canDash = true;
    private bool isDashing;
    

    [Header("Ground Check")]
    // wall og ground check
    public Vector2 groundBoxSize;
    public float groundCastDistance;
    public Vector3 groundCastOffset;
    [SerializeField] private LayerMask groundLayer;

    [Header("Wall Check")]
    public Vector2 wallBoxSize;
    public float wallCastDistance;
    //public Vector3 wallCastOffsetLeft;
    //public Vector3 wallCastOffsetRight;
    public Vector3 wallCastOffset;
    private Vector3 wallCastOffsetRight;
    private Vector3 wallCastOffsetLeft;

    [SerializeField] private LayerMask wallLayer;

    [Header("Objects")]
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private TrailRenderer tr;
    [SerializeField] private Transform Cursor;
    public Animator anim;


    private void Start()
    {
        // anim = gameObject.GetComponent<Animator>();
        // wallCastOffset = wallCastOffsetLeft;

        wallCastOffsetLeft = wallCastOffset;
        Vector3 wallCastOffsetTemp = wallCastOffset;
        wallCastOffsetTemp.x *= -1f;
        wallCastOffsetRight = wallCastOffsetTemp;
    }
    

    void Update()
    {
        handleAnimations();
        handleInAirGravity();
        handleMovement();
        updateCursorDir();
        Debug.Log(cursorDir);

        
        WallSlide();
        wallJump();
        

        if (!isWallJumping)
        {
            Flip();
        }
    }

    private void FixedUpdate()
    {
        // handleCursor();
        if (isDashing == true || isWallJumping)
        {
            return;
        }

        rb.velocity = new Vector2(horizontal * speed, rb.velocity.y);
    }

    private bool isGrounded()
    {
        /* enda partur þarf að breyta allt eftir fyrsta and */
        // return Physics2D.OverlapCircle(groundCheck.position, 0.2f, groundLayer) || Physics2D.OverlapCircle(groundCheck.position, 0.2f, wallLayer);

        if (Physics2D.BoxCast((transform.position + groundCastOffset), groundBoxSize, 0, -transform.up, groundCastDistance, groundLayer))
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireCube((transform.position - transform.up * wallCastDistance) + wallCastOffset, wallBoxSize);
        Gizmos.DrawWireCube((transform.position - transform.up * groundCastDistance) + groundCastOffset, groundBoxSize);
        
    }

    private void handleInAirGravity()
    {
        if (!isDashing)
        {
            if (rb.velocity.y < 0)
            {
                rb.velocity += Vector2.up * Physics2D.gravity.y * (fallMultipler - 1) * Time.deltaTime;
            }
            else if (rb.velocity.y > 0 /*&& !Input.GetButton("Jump")*/)
            {
                rb.velocity += Vector2.up * Physics2D.gravity.y * (lowJumpMultiplier - 1) * Time.deltaTime;
            }
        }
    }

    private void handleAnimations()
    {
        if (isDashing)
        {
            ChangeAnimationState(PLAYER_DASH);
        }
        else if (isAttacking)
        {
            ChangeAnimationState(PLAYER_ATTACK);
        }
        else if (isBlocking)
        {
            ChangeAnimationState(PLAYER_BLOCK);
        }
        else if (!isGrounded())
        {
            ChangeAnimationState(PLAYER_JUMP);
        }
        else if (rb.velocity.x != 0)
        {
            ChangeAnimationState(PLAYER_RUN);
        }
        else
        {
            ChangeAnimationState(PLAYER_IDLE);
        }
    }

    private bool isWalled()
    {
        // return Physics2D.OverlapCircle(wallCheck.position, 0.2f, wallLayer);
        if (isFacingRight)
        {
            if (Physics2D.BoxCast((transform.position + wallCastOffsetRight), wallBoxSize, 0, -transform.right, wallCastDistance, wallLayer))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        else
        {
            if (Physics2D.BoxCast((transform.position + wallCastOffsetLeft), wallBoxSize, 0, transform.right, wallCastDistance, wallLayer))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }

    private void WallSlide()
    {
        if (isWalled() && !isGrounded() && horizontal != 0f)
        {
            isWallSliding = true;
            rb.velocity = new Vector2(rb.velocity.x, Mathf.Clamp(rb.velocity.y, -wallSlidingSpeed, float.MaxValue));
        }
        else
        {
            isWallSliding = false;
        }
    }

    private void wallJump()
    {
        if (isWallSliding)
        {
            isWallSliding = false;
            wallJumpingDirection = -transform.localScale.x;
            wallJumpingCounter = wallJumpingTime;

            // CancelInvoke(nameof(StopWallJumping));
        }
        else
        {
            wallJumpingCounter -= Time.deltaTime;
        }

        if (Input.GetButtonDown("Jump") && wallJumpingCounter > 0f)
        {
            isWallJumping = true;
            rb.velocity = new Vector2(wallJumpingDirection * -wallJumpingPower.x, wallJumpingPower.y);
            wallJumpingCounter = 0f;

            if (transform.localScale.x != wallJumpingDirection)
            {
                isFacingRight = !isFacingRight;
                Vector3 localScale = transform.localScale;
                localScale.x *= -1f;
                transform.localScale = localScale;
            }

           Invoke(nameof(StopWallJumping), wallJumpingDuration);
        }
    }

    private void StopWallJumping()
    {
        isWallJumping = false;
    }

    private void handleMovement()
    {
        if (isDashing == true)
        {
            return;
        }

        horizontal = Input.GetAxisRaw("Horizontal");
        if ((isFacingRight && horizontal < 0f) || (!isFacingRight && horizontal > 0f)) 
        {
            horizontal *= 0.8f;
        }

        if (Input.GetButtonDown("Jump"))
        {
            if (isGrounded())
            {
                rb.velocity = new Vector2(rb.velocity.x, jumpingPower);
            }

            if (rb.velocity.y > 0f)
            {
                rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y * 0.5f);
            }
        }

        if (Input.GetKeyDown(KeyCode.LeftShift) && canDash)
        {
            StartCoroutine(Dash());
        }
        if (Input.GetMouseButton(0) && !isAttacking)
        {
            StartCoroutine(Attack());
        }
        else if (Input.GetMouseButton(1))
        {
            isBlocking = true;
        }
        else
        {
            isBlocking = false;
        }
    }

    private void Flip()
    {
        if ((cursorDir.x < 0f && isFacingRight) || (cursorDir.x > 0f && !isFacingRight))
        {
            isFacingRight = !isFacingRight;
            Vector2 localScale = transform.localScale;
            localScale.x *= -1f;
            transform.localScale = localScale;
        }
    }

    private IEnumerator Attack()
    {
        isAttacking = true;
        yield return new WaitForSeconds(attackTime);
        isAttacking = false;
    }

    private IEnumerator Dash()
    {
        canDash = false;
        isDashing = true;
        float originalGravity = rb.gravityScale;
        rb.gravityScale = 0f;

        // rb.velocity = new Vector2(transform.localScale.x * -dashingPower, 0f);
        /*cursorPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        cursorPosition.z = 0f;
        Vector2 dashingDir = (cursorPosition - transform.position).normalized;
        */
        rb.velocity = cursorDir * dashingPower;

        tr.emitting = true;
        yield return new WaitForSeconds(dashingTime);
        tr.emitting = false;
        rb.gravityScale = originalGravity;
        isDashing = false;
        float newYVelocity = rb.velocity.y * 0.5f;
        rb.velocity = new Vector2(rb.velocity.x, newYVelocity);
        yield return new WaitForSeconds(dashingCooldown);
        canDash = true;
    }

    private void updateCursorDir()
    {
        cursorPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        cursorPosition.z = 0f;
        cursorDir = (cursorPosition - transform.position).normalized;
    }

    private void ChangeAnimationState(string newAnimation)
    {
        if (newAnimation != _currentAnimation)
        {
            anim.Play(newAnimation);
        }
    }

    bool IsAnimationPlaying(Animator animator, string AnimationName)
    {
        if (animator.GetCurrentAnimatorStateInfo(0).IsName(AnimationName) &&
            animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1.0f)
        {
            return true;
        }
        return false;
    }
}
