using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    [Header("Speed")]
    [SerializeField] private float walkSpeed = 4f;
    [SerializeField] private float sprintSpeed = 7f;
    [SerializeField] private float acceleration = 12f;
    [SerializeField] private float deceleration = 8f;

    [Header("Collision")]
    [SerializeField] private LayerMask wallLayer;
    [SerializeField] private float wallBounceForce = 2f;
    [SerializeField] private float wallInputLockTime = 0.15f;

    [Header("Direction Lock / Facing")]
    [Range(0f, 1f)][SerializeField] private float directionLockThreshold = 0.5f;
    [SerializeField] private float facingChangeSpeed = 8f; 

    [Header("Rotation")]
    [SerializeField] private float rotationSpeed = 10f;
    [SerializeField] private float sprintLeanAngle = 15f;

    private Quaternion startRotation;

    private float moveInput;
    private float currentSpeed; 
    private float facingDirection = 1f; 

    private bool isDecelerating;
    private float inputLockTimer;
    private bool sprintHeld;

    public float CurrentSpeed => Mathf.Abs(currentSpeed);
    public float SprintSpeed => sprintSpeed;
    public bool IsSprinting => sprintHeld && !isDecelerating && Mathf.Abs(currentSpeed) > walkSpeed * 0.9f;

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
        if (Input.GetMouseButton(1))
        {
            moveInput = 0f;
            sprintHeld = false;
            facingDirection = Mathf.MoveTowards(facingDirection, 1f, facingChangeSpeed * Time.deltaTime);
            return;
        }

        sprintHeld = Input.GetKey(KeyCode.LeftShift);

        if (inputLockTimer > 0f)
        {
            inputLockTimer -= Time.deltaTime;
            moveInput = 0f;
            return;
        }

        float rawInput = Input.GetAxisRaw("Horizontal");
        moveInput = rawInput;

        float targetFacing = facingDirection;
        if (Mathf.Abs(rawInput) > 0f)
            targetFacing = Mathf.Sign(rawInput);

        // --------- Deceleration trigger when pressing opposite ----------
        if (Mathf.Abs(rawInput) > 0f && Mathf.Sign(rawInput) != Mathf.Sign(facingDirection) && currentSpeed > 0.01f)
        {
            isDecelerating = true;
        }
        else if (Mathf.Abs(rawInput) > 0f)
        {
            isDecelerating = false;
        }
        else
        {
            isDecelerating = true;
        }

        if (!sprintHeld && currentSpeed > walkSpeed)
            isDecelerating = true;

        if (currentSpeed <= walkSpeed)
            isDecelerating = false;

        facingDirection = Mathf.MoveTowards(facingDirection, targetFacing, facingChangeSpeed * Time.deltaTime);

        // ---------- Speed smoothing ----------
        float targetSpeed = moveInput != 0f ? (sprintHeld ? sprintSpeed : walkSpeed) : 0f;
        float accel = isDecelerating ? deceleration : acceleration;
        currentSpeed = Mathf.MoveTowards(currentSpeed, targetSpeed, accel * Time.deltaTime);
    }

    private void MovePlayer()
    {
        float moveSign = Mathf.Sign(facingDirection);
        transform.Translate(currentSpeed * moveSign * Time.deltaTime, 0f, 0f);
    }

    private void ApplyRotation()
    {
        Quaternion targetRotation = startRotation;

        int faceSign = Mathf.Sign(facingDirection) >= 0 ? 1 : -1;

        if (IsSprinting)
            targetRotation = Quaternion.Euler(0f, 0f, -sprintLeanAngle * faceSign);

        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
    }

    private void HandleFlip()
    {
        Vector3 scale = transform.localScale;

        if (Mathf.Abs(facingDirection) > 0.01f)
            scale.x = Mathf.Abs(scale.x) * (Mathf.Sign(facingDirection) >= 0 ? 1f : -1f);

        transform.localScale = scale;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (((1 << collision.gameObject.layer) & wallLayer) != 0)
        {
            currentSpeed = wallBounceForce;
            inputLockTimer = wallInputLockTime;
            isDecelerating = false;
        }
    }
}
