using System;
using UnityEngine;

public class BaseStanceSetup : MonoBehaviour
{

    public enum Stance
    {
        chieved,
        forward,
        backward,
        running,
        upward,
        downward
    }

    public Stance stance = Stance.chieved;

    private float hDirection;
    private float vDirection;

    private PlayerMove playerMove;

    [Header("Stance Switching Cooldown")]
    [SerializeField] private float stanceCooldown = 0.5f;
    private float stanceTimer = 0f;

    [Header("Running override")]
    [SerializeField] private float stopThreshold = 0.05f;
    private bool runningOverride = false;

    void Start()
    {
        playerMove = GetComponentInParent<PlayerMove>();
    }

    void Update()
    {
        if (stanceTimer > 0f)
        {
            stanceTimer -= Time.deltaTime;
        }

        hDirection = Input.GetAxis("Horizontal");
        vDirection = Input.GetAxis("Vertical");

        if (playerMove != null && playerMove.IsSprinting)
        {
            if (stance != Stance.running)
            {
                stance = Stance.running;
            }

            runningOverride = true;
            stanceTimer = 0f;
            return;
        }

        if (runningOverride)
        {
            if (playerMove == null || Mathf.Abs(playerMove.CurrentSpeed) <= stopThreshold)
            {
                runningOverride = false;
                stance = Stance.chieved;
                stanceTimer = stanceCooldown;
            }
            else
            {
                stance = Stance.running;
                return;
            }
        }

        if (Input.GetMouseButton(1))
        {

            if (stanceTimer <= 0f)
            {

                Stance newStance = Stance.forward; 

                if (Mathf.Abs(hDirection) > 0.1f)
                {
                    newStance = hDirection > 0 ? Stance.forward : Stance.backward;
                }
                else if (Mathf.Abs(vDirection) > 0.1f)
                {
                    newStance = vDirection > 0 ? Stance.upward : Stance.downward;
                }

                if (newStance != stance)
                {
                    stance = newStance;
                    stanceTimer = stanceCooldown;
                }
            }
        }
        else
        {
            // Reset to idle when button is not held
            if (!runningOverride)
            {
                stance = Stance.chieved;
            }
        }
    }
}
