using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    [Header("Speed")]
    [SerializeField] private float walkSpeed = 4f;
    [SerializeField] private float sprintSpeed = 7f;
    [SerializeField] private float acceleration = 12f;
    [SerializeField] private float deceleration = 8f;

    [Header("Rotation")]
    [SerializeField] private float rotationSpeed = 10f;
    [SerializeField] private float sprintLeanAngle = 15f;

    private Quaternion startRotation;

    private float moveInput;
    private float currentSpeed;
    private float facingDirection = 1f;

    private bool isDecelerating;
    private bool sprintHeld;

    public float CurrentSpeed => Mathf.Abs(currentSpeed);
    public bool IsSprinting => sprintHeld && Mathf.Abs(currentSpeed) > walkSpeed * 0.9f;
    public float RawHorizontalInput { get; private set; }
    
    public float SprintSpeed => sprintSpeed;

    private void Awake()
    {
        startRotation = transform.rotation;
    }

    private void Update()
    {
        HandleInput();
        MovePlayer();
        ApplyRotation();
        HandleFlip();
    }

    private void HandleInput()
    {
        RawHorizontalInput = Input.GetAxisRaw("Horizontal");
        sprintHeld = Input.GetKey(KeyCode.LeftShift);

        // ---- STANCE MODE (RMB) ----
        if (Input.GetMouseButton(1))
        {
            moveInput = 0f;
            currentSpeed = Mathf.MoveTowards(currentSpeed, 0f, deceleration * Time.deltaTime);
            isDecelerating = true;
            return;
        }

        moveInput = RawHorizontalInput;

        float targetFacing = facingDirection;
        if (Mathf.Abs(moveInput) > 0f)
            targetFacing = Mathf.Sign(moveInput);

        if (Mathf.Abs(moveInput) > 0f && Mathf.Sign(moveInput) != Mathf.Sign(facingDirection))
            isDecelerating = true;
        else
            isDecelerating = moveInput == 0f;

        facingDirection = Mathf.MoveTowards(facingDirection, targetFacing, 8f * Time.deltaTime);

        float targetSpeed = moveInput != 0f ? (sprintHeld ? sprintSpeed : walkSpeed) : 0f;
        float accel = isDecelerating ? deceleration : acceleration;

        currentSpeed = Mathf.MoveTowards(currentSpeed, targetSpeed, accel * Time.deltaTime);
    }

    private void MovePlayer()
    {
        if (Mathf.Abs(currentSpeed) < 0.01f) return;
        transform.Translate(Vector3.right * facingDirection * currentSpeed * Time.deltaTime);
    }

    private void ApplyRotation()
    {
        Quaternion targetRotation = startRotation;

        if (IsSprinting)
            targetRotation = Quaternion.Euler(0f, 0f, -sprintLeanAngle * Mathf.Sign(facingDirection));

        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
    }

    private void HandleFlip()
    {
        Vector3 scale = transform.localScale;
        scale.x = Mathf.Abs(scale.x) * Mathf.Sign(facingDirection);
        transform.localScale = scale;
    }
}
