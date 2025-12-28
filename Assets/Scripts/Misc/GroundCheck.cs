using UnityEngine;

public class PlayerPhysics: MonoBehaviour
{
    [Header("Ground Check")]
    [SerializeField] private float groundCheckDistance = 0.2f;
    [SerializeField] private float groundCheckRadius = 0.25f;
    [SerializeField] private LayerMask groundLayer;

    [Header("Jump & Gravity")]
    [SerializeField] private float gravity = -9.81f;
    [SerializeField] private float jumpForce = 12f;
    [SerializeField] private float fallMultiplier = 2f;

    [Header("Jump Assist")]
    [SerializeField] private float coyoteTime = 0.15f;
    [SerializeField] private float jumpBufferTime = 0.15f;

    private float verticalVelocity;
    private bool isGrounded;

    private float coyoteTimer;
    private float jumpBufferTimer;

    private void Awake()
    {
        groundLayer = LayerMask.GetMask("Ground");
    }

    private void Update()
    {
        HandleInput();
    }

    private void FixedUpdate()
    {
        CheckGround();
        HandleJumpTimers();
        ApplyGravity();
        ApplyMovement();
    }

    // ---------------- INPUT ----------------

    private void HandleInput()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            jumpBufferTimer = jumpBufferTime;
        }

        if (Input.GetKeyUp(KeyCode.Space) && verticalVelocity > 0f)
        {
            verticalVelocity *= 0.5f;
        }
    }

    // ---------------- JUMP LOGIC ----------------

    private void HandleJumpTimers()
    {
        jumpBufferTimer -= Time.fixedDeltaTime;

        if (isGrounded)
        {
            coyoteTimer = coyoteTime;
        }
        else
        {
            coyoteTimer -= Time.fixedDeltaTime;
        }

        if (jumpBufferTimer > 0f && coyoteTimer > 0f)
        {
            Jump();
            jumpBufferTimer = 0f;
            coyoteTimer = 0f;
        }
    }

    private void Jump()
    {
        verticalVelocity = jumpForce;
        isGrounded = false;
    }

    // ---------------- PHYSICS ----------------

    private void CheckGround()
    {
        RaycastHit2D hit = Physics2D.CircleCast(
            transform.position,
            groundCheckRadius,
            Vector2.down,
            groundCheckDistance,
            groundLayer
        );

        if (hit.collider != null && verticalVelocity <= 0f)
        {
            isGrounded = true;
            verticalVelocity = 0f;
        }
        else
        {
            isGrounded = false;
        }
    }

    private void ApplyGravity()
    {
        if (!isGrounded)
        {
            if (verticalVelocity < 0f)
            {
                verticalVelocity += gravity * fallMultiplier * Time.fixedDeltaTime;
            }
            else
            {
                verticalVelocity += gravity * Time.fixedDeltaTime;
            }
        }
    }

    private void ApplyMovement()
    {
        transform.position += Vector3.up * verticalVelocity * Time.fixedDeltaTime;
    }

    // ---------------- DEBUG ----------------

    private void OnDrawGizmos()
    {
        Gizmos.color = isGrounded ? Color.green : Color.red;

        Vector3 start = transform.position;
        Vector3 end = start + Vector3.down * groundCheckDistance;

        Gizmos.DrawWireSphere(start, groundCheckRadius);
        Gizmos.DrawWireSphere(end, groundCheckRadius);
        Gizmos.DrawLine(start, end);
    }
}
