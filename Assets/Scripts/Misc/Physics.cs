using UnityEngine;

public class PlayerPhysics : MonoBehaviour
{
    [Header("Ground Check")]
    [SerializeField] private float groundCheckDistance = 0.2f;
    [SerializeField] private float groundCheckRadius = 0.25f;
    [SerializeField] private LayerMask groundLayer;

    [Header("Gravity")]
    [SerializeField] private float gravity = -9.81f;
    [SerializeField] private float fallMultiplier = 2f;

    public float VerticalVelocity { get; private set; }
    public bool IsGrounded { get; private set; }

    private void Awake()
    {
        groundLayer = LayerMask.GetMask("Ground");
    }

    private void FixedUpdate()
    {
        CheckGround();
        ApplyGravity();
        ApplyMovement();
    }

    // ---------- PUBLIC API (used by Jump.cs) ----------

    public void SetVerticalVelocity(float value)
    {
        VerticalVelocity = value;
    }

    public void CutJump()
    {
        if (VerticalVelocity > 0f)
            VerticalVelocity *= 0.5f;
    }

    // ---------- PHYSICS ----------

    private void CheckGround()
    {
        RaycastHit2D hit = Physics2D.CircleCast(
            transform.position,
            groundCheckRadius,
            Vector2.down,
            groundCheckDistance,
            groundLayer
        );

        if (hit.collider != null && VerticalVelocity <= 0f)
        {
            IsGrounded = true;
            VerticalVelocity = 0f;
        }
        else
        {
            IsGrounded = false;
        }
    }

    private void ApplyGravity()
    {
        if (!IsGrounded)
        {
            float multiplier = VerticalVelocity < 0f ? fallMultiplier : 1f;
            VerticalVelocity += gravity * multiplier * Time.fixedDeltaTime;
        }
    }

    private void ApplyMovement()
    {
        transform.position += Vector3.up * VerticalVelocity * Time.fixedDeltaTime;
    }

    // ---------- DEBUG ----------

    private void OnDrawGizmos()
    {
        Gizmos.color = IsGrounded ? Color.green : Color.red;

        Vector3 start = transform.position;
        Vector3 end = start + Vector3.down * groundCheckDistance;

        Gizmos.DrawWireSphere(start, groundCheckRadius);
        Gizmos.DrawWireSphere(end, groundCheckRadius);
        Gizmos.DrawLine(start, end);
    }
}
