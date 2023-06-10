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

    
    public float fallMultipler = 2.5f;
    public float lowJumpMultiplier = 2f;

    /* animation */
    private string _currentAnimation;
    const string PLAYER_IDLE = "player_idle";
    const string PLAYER_RUNNING = "player_running";

    /* dash i att mus */
    private Vector3 cursorPosition;
    public Vector2 cursorOffset;

    /* wall jump */
    private bool on_wall = false;


    [Header("Objects")]
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private TrailRenderer tr;
    [SerializeField] private Transform wall_checker;
    [SerializeField] private LayerMask jump_wall;
    [SerializeField] private Transform Cursor;
    private Animator anim;
    private BoxCollider2D collider;


    private void Start()
    {
        anim = gameObject.GetComponent<Animator>();
        collider = gameObject.GetComponent<BoxCollider2D>();
    }
    

    void Update()
    {
        
        handleInAirGravity();
        handleMovement();

        // !!!!!!!!!! Tumi, útskýra hvað þetta gerir og hvort við þurfum að hafa þetta

        /* Debug.Log(wall_checker.GetComponent<Collider>());  */
        /* if (Physics2D.OverlapCircle(wall_checker.position, 0.2f, jump_wall) == true){
            Debug.Log("touched wall"); 
            fallMultipler = 0.5f;
        } else {
            fallMultipler = 2.5f;
        } */

        Flip();
    }

    private void FixedUpdate()
    {
        handleCursor();
        if (isDashing == true)
        {
            return;
        }

        rb.velocity = new Vector2(horizontal * speed, rb.velocity.y);
    }

    private bool isGrounded()
    {
        /* enda partur þarf að breyta allt eftir fyrsta and */
        var topLeft = new Vector2(collider.bounds.min.x, collider.bounds.min.y + 0.1f);
        var bottomRight = new Vector2(collider.bounds.max.x, collider.bounds.min.y - 0.1f);
        return Physics2D.OverlapArea(topLeft, bottomRight, groundLayer | jump_wall) || Physics2D.OverlapCircle(wall_checker.position, 0.2f, jump_wall);
    }

    /* private bool is_on_wall()
    {
        if (Physics2D.OverlapCircle(wall_checker.position, 0.2f, jump_wall) == true){
            return Debug.Log("touched wall");
        }
    } */

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

    private void handleMovement()
    {
        if (isDashing == true)
        {
            return;
        }

        horizontal = Input.GetAxisRaw("Horizontal");

        if (rb.velocity.x != 0)
        {
            //Debug.Log("Should be running");
            ChangeAnimationState(PLAYER_RUNNING);
        }
        else
        {
            ChangeAnimationState(PLAYER_IDLE);
        }

        if (Input.GetButtonDown("Jump") && isGrounded())
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpingPower);
        }

        if (Input.GetButtonDown("Jump") && rb.velocity.y > 0f)
        {
            rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y * 0.5f);
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
        if (newAnimation != _currentAnimation) {
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
