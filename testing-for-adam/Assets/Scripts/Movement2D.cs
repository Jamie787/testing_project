using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Movement2D : MonoBehaviour
{
    
    [Header("Components")]
    [SerializeField] private Animator animator;
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private Transform wallCheck;
    [SerializeField] private LayerMask groundLayer;
    [Range(0f, 1f)]
    public float frictionCoefficent;

    [Header("Horizontal Movement")]
    public float horizontalSpeed;
    public float maxSpeed;
    private bool isRight = true;
    private float horizontal;

    [Header("Jump Height")]
    public float jumpVelocity;
    public float jumpPhysics;
    private float coyoteTime;
    private float jumpBuffer;
    [SerializeField] private float maxFallSpeed;

    [Header("Walls")]
    private bool isWallSliding;
    private float wallSlidingSpeed = 1f;

    private bool wallJumping;
    private float wallJumpingDirection;
    private float wallJumpingDirE;
    private float wallJumpingTime = 0.2f;
    private float wallJumpingCounter;
    private float wallJumpingDuration = 0.4f;
    private float wallJumpPush = 9.5f;

    [Header("Upgrades")]
    public bool enableWallJump;
    

    // Start is called before the first frame update
    void Awake()
    {
        rb.gravityScale = jumpPhysics;
    }

    // Update is called once per frame
    void Update()
    {
        animator.SetFloat("Speed", Mathf.Abs(rb.velocity.x));
        animator.SetFloat("JumpVel", rb.velocity.y);

        WallSlide();
    }

    void FixedUpdate()
    {
        if (Mathf.Abs(rb.velocity.x) < maxSpeed) { rb.velocity = new Vector2((horizontal * horizontalSpeed ) + (rb.velocity.x * 0.8f), rb.velocity.y); }

        if (((!isRight && horizontal > 0f) || (isRight && horizontal < 0f)) && !wallJumping ) { Flip(); }
    }

    public void Move(InputAction.CallbackContext context) { horizontal = context.ReadValue<Vector2>().x; }

    public void Jump(InputAction.CallbackContext context)
    {
        if (context.performed && IsGrounded())
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpVelocity);
        }
        if (context.canceled && rb.velocity.y > 0)
        {
            rb.velocity = new Vector2(rb.velocity.x, 0);
        }
    }

    private bool IsGrounded() { return Physics2D.OverlapCircle(groundCheck.position, 0.2f, groundLayer); }

    private bool IsWalled() { return Physics2D.OverlapCircle(wallCheck.position, 0.2f, groundLayer); }

    private void WallSlide()
    {
        
        if (!IsGrounded() && IsWalled() && (horizontal != 0f)) 
        {
            isWallSliding = true;
            rb.velocity = new Vector2(rb.velocity.x, Mathf.Clamp(rb.velocity.y, -wallSlidingSpeed, float.MaxValue));
            animator.SetBool("WallSlide", true);
        }
        else { isWallSliding = false; animator.SetBool("WallSlide", false); }
    }

    public void WallJump(InputAction.CallbackContext context)
    {
        if (isWallSliding)
        {
            wallJumping = false;
            wallJumpingCounter = wallJumpingTime;
            wallJumpingDirection = (transform.rotation.y != 0) ? -1 : 1;
            wallJumpingDirE = (transform.rotation.y != 0) ? 0 : 180;

            CancelInvoke(nameof(StopWallJump));
        }
        else
        {
            wallJumpingCounter -= Time.deltaTime;
        }
        if (context.performed && wallJumpingCounter > 0f && enableWallJump)
        {
            wallJumping = true;

            rb.velocity = new Vector2(wallJumpPush * wallJumpingDirection, jumpVelocity);
            wallJumpingCounter = 0f;
        }
        if (context.canceled && rb.velocity.y > 0 && wallJumping)
        {
            rb.velocity = new Vector2(rb.velocity.x, 0);
        }

        if (transform.rotation.x != wallJumpingDirE) { Flip(); }

        Invoke(nameof(StopWallJump), wallJumpingDuration);
    }

    private void StopWallJump() { wallJumping = false; }
    private void Flip()
    {
        isRight = !isRight;
        transform.rotation = Quaternion.Euler(0, (transform.rotation.y != 0) ? 0 : 180, 0);
    }
}
