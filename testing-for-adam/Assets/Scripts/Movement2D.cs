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
    public float jumpHeight;
    private float coyoteTime;
    private float jumpBuffer;
    [SerializeField] private float maxFallSpeed;

    [Header("Walls")]
    private bool isWallSliding;
    private float wallSlidingSpeed = 1f;

    // Start is called before the first frame update
    void Start()
    {
        
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

        if ((!isRight && horizontal > 0f) || (isRight && horizontal < 0f)) { Flip(); }
    }

    public void Move(InputAction.CallbackContext context) { horizontal = context.ReadValue<Vector2>().x; }

    public void Jump(InputAction.CallbackContext context)
    {

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
    private void Flip()
    {
        isRight = !isRight;
        transform.rotation = Quaternion.Euler(0, (transform.rotation.y != 0) ? 0 : 180, 0);
    }
}
