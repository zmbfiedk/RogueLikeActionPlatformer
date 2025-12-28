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

    [SerializeField] private float currentSpeed;
    private float moveInput;
    private int facingDirection = 1;

    private bool isDecelerating;
    private float inputLockTimer;

    void Update()
    {
        // Handle input lock after wall hit
        if (inputLockTimer > 0f)
        {
            inputLockTimer -= Time.deltaTime;
            moveInput = 0;
        }

        float rawInput = Input.GetAxisRaw("Horizontal");
        bool sprintHeld = Input.GetKey(KeyCode.LeftShift);

        if (inputLockTimer <= 0f)
        {
            if (rawInput != 0)
            {
                int inputDir = (int)Mathf.Sign(rawInput);
                float speedPercent = Mathf.Abs(currentSpeed) / sprintSpeed;

                if (inputDir != facingDirection && speedPercent >= directionLockThreshold)
                {
                    isDecelerating = true;
                    moveInput = facingDirection; 
                }
                else
                {
                    moveInput = rawInput;
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
        {
            targetSpeed = sprintHeld ? sprintSpeed : walkSpeed;
        }

        float accelRate = isDecelerating ? deceleration : acceleration;

        currentSpeed = Mathf.MoveTowards(
            currentSpeed,
            targetSpeed,
            accelRate * Time.deltaTime
        );

        transform.Translate(moveInput * currentSpeed * Time.deltaTime, 0, 0);

        HandleFlip();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (((1 << collision.gameObject.layer) & wallLayer) != 0)
        {
            currentSpeed = wallBounceForce;
            moveInput = -facingDirection;
            inputLockTimer = wallInputLockTime;
            isDecelerating = false;
        }
    }

    private void HandleFlip()
    {
        transform.localScale = new Vector3(facingDirection, 1, 1);
    }
}
