using UnityEngine;

public class PlayerBaseAttack : MonoBehaviour
{
    private BaseStanceSetup baseStance;
    private ChildStanceSetup childStance;

    [Header("Speeds")]
    [SerializeField] private float attackSpeed = 8f;
    [SerializeField] private float returnSpeed = 12f;
    [SerializeField] private float rotationSpeed = 12f;

    [Header("Attack Offsets")]
    public Vector3 attackForward;
    public Vector3 attackBackward;
    public Vector3 attackUpward;
    public Vector3 attackDownward;

    [Header("Attack Rotations")]
    public Vector3 rotForward;
    public Vector3 rotBackward;
    public Vector3 rotUpward;
    public Vector3 rotDownward;

    private bool attacking;
    private bool returning;

    private Vector3 attackTargetPos;
    private Quaternion attackTargetRot;

    private Vector3 returnPos;
    private Quaternion returnRot;

    void Awake()
    {
        baseStance = GetComponentInParent<BaseStanceSetup>();
        childStance = GetComponent<ChildStanceSetup>();
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0) || Input.GetKey(KeyCode.V) && !attacking && !returning) {
            StartAttack();
        }


        if (attacking)
            DoAttack();
        else if (returning)
            DoReturn();

    }

    void StartAttack()
    {
        if (baseStance.stance == BaseStanceSetup.Stance.chieved ||
            baseStance.stance == BaseStanceSetup.Stance.running)
            return;

        childStance.Locked = true;

        returnPos = childStance.GetCurrentTargetPosition();
        returnRot = childStance.GetCurrentTargetRotation();

        attackTargetPos = GetAttackPosition(baseStance.stance);
        attackTargetRot = GetAttackRotation(baseStance.stance);

        attacking = true;
    }


    void DoAttack()
    {
        transform.localPosition = Vector3.MoveTowards(
            transform.localPosition,
            attackTargetPos,
            attackSpeed * Time.deltaTime
        );

        transform.localRotation = Quaternion.Lerp(
            transform.localRotation,
            attackTargetRot,
            rotationSpeed * Time.deltaTime
        );

        if (Vector3.Distance(transform.localPosition, attackTargetPos) < 0.01f)
        {
            attacking = false;
            returning = true;
        }
    }

    void DoReturn()
    {
        transform.localPosition = Vector3.MoveTowards(
            transform.localPosition,
            returnPos,
            returnSpeed * Time.deltaTime
        );

        transform.localRotation = Quaternion.Lerp(
            transform.localRotation,
            returnRot,
            rotationSpeed * Time.deltaTime
        );

        if (Vector3.Distance(transform.localPosition, returnPos) < 0.01f)
        {
            returning = false;

            childStance.Locked = false;
        }
    }


    Vector3 GetAttackPosition(BaseStanceSetup.Stance stance)
    {
        Vector3 basePos = childStance.GetCurrentTargetPosition();

        return stance switch
        {
            BaseStanceSetup.Stance.forward => basePos + attackForward,
            BaseStanceSetup.Stance.backward => basePos + attackBackward,
            BaseStanceSetup.Stance.upward => basePos + attackUpward,
            BaseStanceSetup.Stance.downward => basePos + attackDownward,
            _ => basePos
        };
    }

    Quaternion GetAttackRotation(BaseStanceSetup.Stance stance)
    {
        return Quaternion.Euler(stance switch
        {
            BaseStanceSetup.Stance.forward => rotForward,
            BaseStanceSetup.Stance.backward => rotBackward,
            BaseStanceSetup.Stance.upward => rotUpward,
            BaseStanceSetup.Stance.downward => rotDownward,
            _ => transform.localEulerAngles
        });
    }
}
