using UnityEngine;

public class RunningWobble : MonoBehaviour
{
    private PlayerMove playerMove;
    private ChildStanceSetup childStance;

    void Start()
    {
        childStance = GetComponent<ChildStanceSetup>();
        playerMove = GetComponentInParent<PlayerMove>();
    }

    void Update()
    {
        if (playerMove == null || childStance == null) return;

        if (!playerMove.IsSprinting)
        {
            Vector3 pos = transform.localPosition;
            pos.y = childStance.GetCurrentTargetPosition().y;
            transform.localPosition = pos;
            return;
        }

        float speedPercent = Mathf.Abs(playerMove.CurrentSpeed) / playerMove.SprintSpeed;
        float wobbleAmount = Mathf.Lerp(0.05f, 0.15f, speedPercent);
        float wobbleSpeed = Mathf.Lerp(5f, 15f, speedPercent);
        float wobble = Mathf.Sin(Time.time * wobbleSpeed) * wobbleAmount;
        float targetY = childStance.GetCurrentTargetPosition().y;
        Vector3 localPos = transform.localPosition;
        localPos.y = targetY + wobble;
        transform.localPosition = localPos;
    }
}
