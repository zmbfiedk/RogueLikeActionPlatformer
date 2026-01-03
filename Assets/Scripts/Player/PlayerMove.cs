using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    [Header("Speed")]
    [SerializeField] private float walkSpeed = 4f;
    [SerializeField] private float sprintSpeed = 7f;
    [SerializeField] private float acceleration = 12f;
    [SerializeField] private float deceleration = 6f;

    [Header("Collision")]
    [SerializeField] private LayerMask wallLayer;
    [SerializeField] private float wallBounceForce = 2f;
    [SerializeField] private float wallInputLockTime = 0.15f;

    [Header("Direction Lock")]
    [Range(0f, 1f)]
    [SerializeField] private float directionLockThreshold = 0.5f;

    [Header("Rotation")]
    [SerializeField] private float rotationSpeed = 10f; // how fast the player returns to start rotation
    private Quaternion startRotation;

    [SerializeField] private float currentSpeed;
    private float moveInput;
    private int facingDirection = 1;

    private bool isDecelerating;
    private float inputLockTimer;
    private bool sprintHeld;

    public bool IsSprinting => sprintHeld && !isDecelerating && Mathf.Abs(currentSpeed) > walkSpeed * 0.9f;
    public float CurrentSpeed => currentSpeed * facingDirection;
    public float SprintSpeed => sprintSpeed;

    void Awake()
    {
        startRotation = transform.rotation;
    }

    void Update()
    {
        // Handle input lock after wall hit
        if (inputLockTimer > 0f)
        {
            inputLockTimer -= Time.deltaTime;
            moveInput = 0;
        }

        float rawInput = Input.GetAxisRaw("Horizontal");
        sprintHeld = Input.GetKey(KeyCode.LeftShift);

        if (inputLockTimer <= 0f)
        {
            if (rawInput != 0)
            {
                int inputDir = (int)Mathf.Sign(rawInput);
                float speedPercent = Mathf.Abs(currentSpeed) / sprintSpeed;

                if (inputDir != facingDirection && speedPercent >= directionLockThreshold)
                {
                    isDecelerating = true;
                    moveInput = facingDirection; // keep moving in old facing direction while decelerating
                }
                else
                {
                    moveInput = rawInput;

                    // Only change facing direction if sprinting
                    if (IsSprinting)
                        facingDirection = inputDir;
                }
            }
            else
            {
                moveInput = 0;
            }
        }

        if (!sprintHeld && Mathf.Abs(currentSpeed) > walkSpeed)
        {
            isDecelerating = true;
        }

        if (Mathf.Abs(currentSpeed) <= walkSpeed)
        {
            isDecelerating = false;
        }

        float targetSpeed = 0f;
        if (moveInput != 0)
            targetSpeed = sprintHeld ? sprintSpeed : walkSpeed;

        float accelRate = isDecelerating ? deceleration : acceleration;
        currentSpeed = Mathf.MoveTowards(currentSpeed, targetSpeed, accelRate * Time.deltaTime);

        transform.Translate(moveInput * currentSpeed * Time.deltaTime, 0, 0);

        // Handle flipping
        HandleFlip();

        // Apply rotation (return to start rotation when not sprinting)
        ApplyRotation();
    }


    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (((1 << collision.gameObject.layer) & wallLayer) != 0)
        {
            // bounce away from wall
            currentSpeed = wallBounceForce;
            moveInput = -facingDirection;
            inputLockTimer = wallInputLockTime;
            isDecelerating = false;
        }
    }

    private void ApplyRotation()
    {
        if (!IsSprinting)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, startRotation, rotationSpeed * Time.deltaTime);
        }
        else
        {
            // If you want a sprint lean, you can set a target rotation here based on facingDirection.
        }
    }


    private void HandleFlip()
    {
        Vector3 scale = transform.localScale;

        if (IsSprinting)
        {
            scale.x = Mathf.Abs(scale.x) * facingDirection; // flip according to facingDirection
        }
        else
        {
            scale.x = Mathf.Abs(scale.x); // always face right when not sprinting
        }

        transform.localScale = scale;
    }
}
