using UnityEngine;
using System.Collections.Generic;
using System;

public class ImpactArrow : EmissionSkillEffect
{
    protected override bool Movable
    {
        get
        {
            return true;
        }
    }
    public override float ExistTime
    {
        get
        {
            return 6f;
        }
    }
    private float moveSpeedAddPerSec = 1f;
    private float moveSpeedToAdditionDmgRatio = 2f;
    private float baseDmgRatio = .5f;
    private float moveSpeed = 0f;
    private float rotationSpeed = 1f;
    public override float Range
    {
        get
        {
            return 50f;
        }
    }
    public override float MinRange
    {
        get
        {
            return 0;//GetComponentInChildren<MeshCollider>().sharedMesh.bounds.size.y * GetComponentInChildren<MeshCollider>().transform.localScale.y;
        }
    }
    public override float Radius
    {
        get
        {
            return .1f;
        }
    }
    public override float MoveSpeed { get { return moveSpeed; } }
    public override DamageType EffectDamageType { get { return DamageType.Nature; } }
    public override DamageType DirectDamageType { get { return DamageType.Skill; } }
    private float angleToTarget;
    private Vector3 startPos;
    public override void Prepare(Transform model, float castimeTime)
    {
        base.Prepare(model, castimeTime);
        if(user is NPCController)
            angleToTarget = Vector3.Angle(
                       //使用者的方向
                       user.transform.forward,
                       //目標方向在 使用者方向與y軸的平面的投影
                       Vector3.ProjectOnPlane(
                           (user as NPCController).attackTarget.position - transform.position,

                           Vector3.Cross(user.transform.forward, Vector3.up))
                   );
        startPos = transform.position;
    }

    protected override void Start()
    {
        
        base.Start();
        transform.localPosition += new Vector3(0f, .2f,0);
        transform.localScale = new Vector3(1, 1, 0);
    }
    protected override void Update()
    {
        if (!IsPlay && CastTime != 0)
        {
            if (transform.localScale.z < 1)
            {
                transform.localScale += new Vector3(0, 0, 1) * Time.deltaTime / (CastTime / 4);
                //transform.localScale = new Vector3(1, 1, 1);// * Time.deltaTime / (CastTime / 4);
            }
            if (user is NPCController)
                transform.RotateAround(startPos, Vector3.ProjectOnPlane( transform.right,Vector3.up),
                    (Time.deltaTime/CastTime) * angleToTarget//,Space.World
                );
        }//else        
            transform.Find("ParticleBody").transform.RotateAround(transform.position - Vector3.up*.2f, transform.forward, 360 * rotationSpeed * Time.deltaTime);
        rotationSpeed += Time.deltaTime;
        base.Update();
        moveSpeed += Time.deltaTime* moveSpeedAddPerSec;
    }
    public override bool ShouldCast(NPCController caster, List<Transform> TargetsInVision, BaseSkill skillSetting)
    {
        return ShouldCastRangeAttackPredefine(caster, TargetsInVision, skillSetting);
    }
    //粒子傷害
    public override float GetCurDamage(DamageType type, out bool isCritical, out float additionHit)
    {
        float dmg = user.GetCurDamage(EffectDamageType, out isCritical, out additionHit);
        return dmg * (baseDmgRatio + MoveSpeed * moveSpeedToAdditionDmgRatio) *skillSetting.AdjustedEffectDamage;
    }
}
