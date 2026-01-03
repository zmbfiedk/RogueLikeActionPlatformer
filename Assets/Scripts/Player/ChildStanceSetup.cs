using UnityEngine;

public class ChildStanceSetup : MonoBehaviour
{
    [SerializeField] private BaseStanceSetup baseStance;

    [Header("Positions")]
    public Vector3 chievedPosition;
    public Vector3 forwardPosition;
    public Vector3 backwardPosition;
    public Vector3 runningPosition;
    public Vector3 upwardPosition;
    public Vector3 downwardPosition;

    [Header("Rotations")]
    public Vector3 chievedRotation;
    public Vector3 forwardRotation;
    public Vector3 backwardRotation;
    public Vector3 runningRotation;
    public Vector3 upwardRotation;
    public Vector3 downwardRotation;

    [SerializeField] private float lerpSpeed = 12f;

    private Vector3 targetPosition;
    private Quaternion targetRotation;

    void Awake()
    {
        if (baseStance == null)
            baseStance = GetComponentInParent<BaseStanceSetup>();
    }

    void Update()
    {
        if (baseStance == null) return;

        switch (baseStance.stance)
        {
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
            default:
                targetPosition = chievedPosition;
                targetRotation = Quaternion.Euler(chievedRotation);
                break;
        }

        transform.localPosition = Vector3.Lerp(transform.localPosition, targetPosition, Time.deltaTime * lerpSpeed);
        transform.localRotation = Quaternion.Lerp(transform.localRotation, targetRotation, Time.deltaTime * lerpSpeed);
    }

    public Vector3 GetCurrentTargetPosition() => targetPosition;
    public Quaternion GetCurrentTargetRotation() => targetRotation;
}
