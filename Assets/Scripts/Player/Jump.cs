using UnityEngine;

public class Jump : MonoBehaviour
{
    [Header("Jump Settings")]
    [SerializeField] private float jumpForce = 12f;
    [SerializeField] private float coyoteTime = 0.15f;
    [SerializeField] private float jumpBufferTime = 0.15f;

    private PlayerPhysics physics;

    private float coyoteTimer;
    private float jumpBufferTimer;

    private void Awake()
    {
        physics = GetComponent<PlayerPhysics>();
    }

    private void Update()
    {
        HandleInput();
    }

    private void FixedUpdate()
    {
        HandleTimers();
        TryJump();
    }

    // ---------- INPUT ----------

    private void HandleInput()
    {
        if (Input.GetKeyDown(KeyCode.Space))
            jumpBufferTimer = jumpBufferTime;

        if (Input.GetKeyUp(KeyCode.Space))
            physics.CutJump();
    }

    // ---------- JUMP LOGIC ----------

    private void HandleTimers()
    {
        jumpBufferTimer -= Time.fixedDeltaTime;

        if (physics.IsGrounded)
            coyoteTimer = coyoteTime;
        else
            coyoteTimer -= Time.fixedDeltaTime;
    }

    private void TryJump()
    {
        if (jumpBufferTimer > 0f && coyoteTimer > 0f)
        {
            physics.SetVerticalVelocity(jumpForce);
            jumpBufferTimer = 0f;
            coyoteTimer = 0f;
        }
    }
}
