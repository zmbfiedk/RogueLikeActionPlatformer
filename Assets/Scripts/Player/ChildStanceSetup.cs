using UnityEngine;

public class ChildStanceSetup : MonoBehaviour
{
    public enum StancePosition
    {
        chieved,
        forward,
        backward,
        running,
        upward,
        downward
    }

    [Header("Reference")]
    [SerializeField] private BaseStanceSetup baseStance;

    [Header("Stance Positions")]
    [SerializeField] private Vector3 chievedPosition;
    [SerializeField] private Vector3 forwardPosition;
    [SerializeField] private Vector3 backwardPosition;
    [SerializeField] private Vector3 runningPosition;
    [SerializeField] private Vector3 upwardPosition;
    [SerializeField] private Vector3 downwardPosition;

    [Header("Stance Rotations")]
    [SerializeField] private Vector3 chievedRotation;
    [SerializeField] private Vector3 forwardRotation;
    [SerializeField] private Vector3 backwardRotation;
    [SerializeField] private Vector3 runningRotation;
    [SerializeField] private Vector3 upwardRotation;
    [SerializeField] private Vector3 downwardRotation;

    [Header("Lerp Settings")]
    [SerializeField] private float positionLerpSpeed = 12f;
    [SerializeField] private float rotationLerpSpeed = 12f;

    private Vector3 targetPosition;
    private Quaternion targetRotation;

    void Awake()
    {
        // Auto-assign if not set
        if (baseStance == null)
            baseStance = GetComponentInParent<BaseStanceSetup>();
    }

    void Update()
    {
        if (baseStance == null) return;

        switch (baseStance.stance)
        {
            case BaseStanceSetup.Stance.chieved:
                targetPosition = chievedPosition;
                targetRotation = Quaternion.Euler(chievedRotation);
                break;

            case BaseStanceSetup.Stance.forward:
                targetPosition = forwardPosition;
                targetRotation = Quaternion.Euler(forwardRotation);
                break;

            case BaseStanceSetup.Stance.backward:
                targetPosition = backwardPosition;
                targetRotation = Quaternion.Euler(backwardRotation);
                break;

            case BaseStanceSetup.Stance.running:
                targetPosition = runningPosition;
                targetRotation = Quaternion.Euler(runningRotation);
                break;

            case BaseStanceSetup.Stance.upward:
                targetPosition = upwardPosition;
                targetRotation = Quaternion.Euler(upwardRotation);
                break;

            case BaseStanceSetup.Stance.downward:
                targetPosition = downwardPosition;
                targetRotation = Quaternion.Euler(downwardRotation);
                break;
        }

        transform.localPosition = Vector3.Lerp(
            transform.localPosition,
            targetPosition,
            Time.deltaTime * positionLerpSpeed
        );

        transform.localRotation = Quaternion.Lerp(
            transform.localRotation,
            targetRotation,
            Time.deltaTime * rotationLerpSpeed
        );
    }

    public Vector3 GetCurrentTargetPosition()
    {
        return targetPosition;
    }

}
