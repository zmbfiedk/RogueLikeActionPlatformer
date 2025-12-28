using UnityEngine;

public class GroundCheck : MonoBehaviour
{
    [Header("Ground Check")]
    [SerializeField] private float groundCheckDistance = 0.2f;
    [SerializeField] private float groundCheckRadius = 0.25f;
    [SerializeField] private LayerMask groundLayer;

    [Header("Gravity")]
    [SerializeField] private float gravity = -25f;
    private float verticalVelocity;

    private bool isGrounded;

    private void Awake()
    {
        groundLayer = LayerMask.GetMask("Ground");
    }

    private void FixedUpdate()
    {
        CheckGround();
        ApplyGravity();
    }

    private void CheckGround()
    {
        RaycastHit2D hit = Physics2D.CircleCast(
            transform.position,
            groundCheckRadius,
            Vector2.down,
            groundCheckDistance,
            groundLayer
        );
        isGrounded = hit.collider != null;

        if (isGrounded && verticalVelocity < 0f)
        {
            verticalVelocity = 0f;
            Debug.Log("Grounded");
        }
        else if (!isGrounded)
        {
            Debug.Log("Not Grounded");
        }
    }

    private void ApplyGravity()
    {
        if (!isGrounded)
        {
            verticalVelocity += gravity * Time.fixedDeltaTime;
        }

        transform.position += Vector3.up * verticalVelocity * Time.fixedDeltaTime;
    }

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
