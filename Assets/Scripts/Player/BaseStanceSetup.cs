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

    private PlayerMove playerMove;
    private float stanceTimer;

    [SerializeField] private float stanceCooldown = 0.5f;

    void Awake()
    {
        playerMove = GetComponentInParent<PlayerMove>();
    }

    void Update()
    {
        if (stanceTimer > 0f)
            stanceTimer -= Time.deltaTime;

        if (playerMove == null) return;

        float h = playerMove.RawHorizontalInput;
        float v = Input.GetAxisRaw("Vertical");

        // Running override
        if (playerMove.IsSprinting)
        {
            stance = Stance.running;
            stanceTimer = 0f;
            return;
        }

        // Stance mode
        if (Input.GetMouseButton(1) && stanceTimer <= 0f)
        {
            Stance newStance = Stance.forward;

            if (Mathf.Abs(h) > 0.1f)
                newStance = h > 0 ? Stance.forward : Stance.backward;
            else if (Mathf.Abs(v) > 0.1f)
                newStance = v > 0 ? Stance.upward : Stance.downward;

            if (newStance != stance)
            {
                stance = newStance;
                stanceTimer = stanceCooldown;
            }
        }
        else if (!Input.GetMouseButton(1))
        {
            stance = Stance.chieved;
        }
    }
}
