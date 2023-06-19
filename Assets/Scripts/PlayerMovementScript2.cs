using System.Collections;
using UnityEngine;

public class PlayerMovementScript2 : MonoBehaviour
{
    private float horizontal;
    private float speed = 8f;
    public float jumpingPower = 16f;
    private bool isFacingRight = false;

    /* dash */
    private bool canDash = true;
    private bool isDashing;
    public float dashingPower = 50f;
    private float dashingTime = 0.1f;
    private float dashingCooldown = 1f;

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
    const string PLAYER_RUNNING = "player_running";
    const string PLAYER_BASIC_ATTACK = "player_attack_animation";
    const string PLAYER_JUMPING = "player_jump_up";
    const string PLAYER_FALLING_DOWN = "player_fall_down";
    const string PLAYER_JUMP_LANDING = "player_jump_landing";


    /* dash i att mus */
    private Vector3 cursorPosition;
    public Vector2 cursorOffset;

    /* wall jump */
    private bool on_wall = false;


    
    //[SerializeField] private Transform wallCheck;
    //[SerializeField] private LayerMask wallLayer;
    //[SerializeField] private Transform groundCheck;
    //[SerializeField] private LayerMask groundLayer;

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
    private Animator anim;
    private BoxCollider2D collider;


    private void Start()
    {
        anim = gameObject.GetComponent<Animator>();
        // wallCastOffset = wallCastOffsetLeft;

        wallCastOffsetLeft = wallCastOffset;
        Vector3 wallCastOffsetTemp = wallCastOffset;
        wallCastOffsetTemp.x *= -1f;
        wallCastOffsetRight = wallCastOffsetTemp;
    }
    

    void Update()
    {
        
        handleInAirGravity();
        handleMovement();

        
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
        if (rb.velocity.y < 0)
        {
            rb.velocity += Vector2.up * Physics2D.gravity.y * (fallMultipler - 1) * Time.deltaTime;
        }
        else if (rb.velocity.y > 0 /*&& !Input.GetButton("Jump")*/)
        {
            rb.velocity += Vector2.up * Physics2D.gravity.y * (lowJumpMultiplier - 1) * Time.deltaTime;
        }

    }

    /*
    private void handleCursor()
    {
        // cursor Position
        cursorPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        cursorPosition.z = 0f;
        Vector2 cursorDir = (cursorPosition - transform.position).normalized;
        Cursor.transform.position = (rb.position + cursorDir) - cursorOffset;

        // Cursor Rotation
        Vector3 lookAt = rb.position;
        float AngleRad = Mathf.Atan2(lookAt.y - Cursor.position.y, lookAt.x - Cursor.position.x);
        float AngleDeg = (180 / Mathf.PI) * AngleRad;
        Cursor.rotation = Quaternion.Euler(0f, 0f, AngleDeg + 90);
    }
    */

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

            CancelInvoke(nameof(StopWallJumping));
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

        if (Input.GetMouseButtonDown(0))
        {
            ChangeAnimationState(PLAYER_BASIC_ATTACK);
        }

        if (rb.velocity.y > 0)
        {
            //Debug.Log("Should be running");
            ChangeAnimationState(PLAYER_JUMPING);
        }
        else if (rb.velocity.y < 0)
        {
            ChangeAnimationState(PLAYER_FALLING_DOWN);
        }
        else
        {
            Debug.Log(_currentAnimation);
            if (_currentAnimation == PLAYER_FALLING_DOWN)
            {
                ChangeAnimationState(PLAYER_JUMP_LANDING);
            }
            else
            {
                if (rb.velocity.x == 0f)
                {
                    ChangeAnimationState(PLAYER_IDLE);
                }
                else
                {
                    ChangeAnimationState(PLAYER_RUNNING);
                }
            }
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
    }

    private void Flip()
    {
        if (isFacingRight && horizontal < 0f || !isFacingRight && horizontal > 0f)
        {
            isFacingRight = !isFacingRight;
            Vector2 localScale = transform.localScale;
            localScale.x *= -1f;
            transform.localScale = localScale;
            
        }
    }

    private IEnumerator Dash()
    {
        canDash = false;
        isDashing = true;
        float originalGravity = rb.gravityScale;
        rb.gravityScale = 0f;

        // rb.velocity = new Vector2(transform.localScale.x * -dashingPower, 0f);
        cursorPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        cursorPosition.z = 0f;
        Vector2 dashingDir = (cursorPosition - transform.position).normalized;
        rb.velocity = dashingDir * dashingPower;

        tr.emitting = true;
        yield return new WaitForSeconds(dashingTime);
        tr.emitting = false;
        rb.gravityScale = originalGravity;
        isDashing = false;
        yield return new WaitForSeconds(dashingCooldown);
        canDash = true;
    }

    private void ChangeAnimationState(string newAnimation)
    {
        if (newAnimation != _currentAnimation || _currentAnimation != PLAYER_JUMP_LANDING) {
            anim.Play(newAnimation);
            _currentAnimation = newAnimation;
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
