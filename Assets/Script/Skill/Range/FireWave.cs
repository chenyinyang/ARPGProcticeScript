using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

public class FireWave : EmissionSkillEffect
{
    public override DamageType DirectDamageType
    {
        get
        {
            return DamageType.Magic;
        }
    }

    public override DamageType EffectDamageType
    {
        get
        {
            return DamageType.Magic;
        }
    }

    public override float ExistTime
    {
        get
        {
            return 5f;
        }
    }

    public override float MoveSpeed
    {
        get
        {
            return 1f;
        }
    }
    public override float Radius
    {
        get
        {
            return .2f;
        }
    }
    protected override bool Movable
    {
        get
        {
            return false;
        }
    }
    void OnTriggerStay(Collider other) {
        if (other.transform != user.transform)
        {
            Damagable hitTarget = other.transform.GetComponent<Damagable>();
            if (IsPlay && hitTarget != null && user.CanDamageTarget(hitTarget))
            {
                hitTarget.Damage(DirectDamageType, this, user);
            }
            //Destroy(gameObject);
        }
    }
    public override float GetCurDamage(DamageType type, out bool isCritical, out float additionHit)
    {
        float dmg = user.GetCurDamage(EffectDamageType, out isCritical, out additionHit);
        return dmg * skillSetting.AdjustedEffectDamage;
    }

    public override bool ShouldCast(NPCController caster, List<Transform> TargetsInVision, BaseSkill skillSetting)
    {
        return ShouldCastRangeAttackPredefine(caster, TargetsInVision, skillSetting);
    }
}
