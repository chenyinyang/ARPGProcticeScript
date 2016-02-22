using UnityEngine;
using System.Collections;

public class RangeAttackAI : AttackAI {
    public override float attackRange
    {
        get
        {
            return movementAI.VisionRadious * .75f;
        }

        protected set
        {
            base.attackRange = value;
        }
    }
    protected override float SetAttackRange()
    {
        return movementAI.VisionRadious*.75f;
    }
    BaseNPCMovementAI movementAI;
    protected override void Awake()
    {
        base.Awake();
        movementAI = GetComponent<BaseNPCMovementAI>();
    }
    protected override bool ShouldAttack(Transform target)
    {
        if (target.GetComponentInParent<Build>() != null)
        {
            return Vector3.Distance(myTransform.position, target.position) < attackRange + target.GetComponentInParent<MeshFilter>().mesh.bounds.size.x*target.transform.localScale.x;
        }
        return Vector3.Distance(myTransform.position, target.position) < attackRange;
    }
}
