using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovementScript2 : MonoBehaviour
{
    private float horizontal;
    private float speed = 8f;
    public float jumpingPower = 16f;
    private bool isFacingRight = false;

    private bool canDash = true;
    private bool isDashing;
    public float dashingPower = 50f;
    private float dashingTime = 0.1f;
    private float dashingCooldown = 1f;

    public float fallMultipler = 2.5f;
    public float lowJumpMultiplier = 2f;

    private string _currentAnimation;
    const string PLAYER_IDLE = "player_idle";
    const string PLAYER_RUNNING = "player_running";


    private bool on_wall = false;


    [Header("Objects")]
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private TrailRenderer tr;
    [SerializeField] private Transform wall_checker;
    [SerializeField] private LayerMask jump_wall;
    private Animator anim;


    private void Start()
    {
        anim = gameObject.GetComponent<Animator>();
    }
    

    void Update()
    {
        /* Debug.Log(wall_checker.GetComponent<Collider>());  */
        if (Physics2D.OverlapCircle(wall_checker.position, 0.2f, jump_wall) == true){
            Debug.Log("touched wall"); 
            fallMultipler = 0.5f;
        } else {
            fallMultipler = 2.5f;
        }



        if ( rb.velocity.y < 0)
        {
            rb.velocity += Vector2.up * Physics2D.gravity.y * (fallMultipler - 1) * Time.deltaTime;
        }
        else if ( rb.velocity.y > 0 /*&& !Input.GetButton("Jump")*/)
        {
            rb.velocity += Vector2.up * Physics2D.gravity.y * (lowJumpMultiplier - 1) * Time.deltaTime;
        }

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

        Flip();
    }

    private void FixedUpdate()
    {
        if (isDashing == true)
        {
            return;
        }

        rb.velocity = new Vector2(horizontal * speed, rb.velocity.y);
    }

    private bool isGrounded()
    {
        return Physics2D.OverlapCircle(groundCheck.position, 0.2f, groundLayer) || Physics2D.OverlapCircle(wall_checker.position, 0.2f, jump_wall) || Physics2D.OverlapCircle(groundCheck.position, 0.2f, jump_wall) ;
    }

    /* private void is_on_wall()
    {
        if (Physics2D.OverlapCircle(wall_checker.position, 0.2f, jump_wall) == true){
            return Debug.Log("touched wall");
        }
    } */

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
        rb.velocity = new Vector2(transform.localScale.x * -dashingPower, 0f);
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
