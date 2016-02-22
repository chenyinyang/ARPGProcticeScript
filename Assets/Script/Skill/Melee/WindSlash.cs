using UnityEngine;
using System.Collections;
using System.Linq;
using System;
using System.Collections.Generic;

public class WindSlash : MeleeSkillEffect
{
    public override DamageType EffectDamageType
    {
        get
        {
            return DamageType.Skill;
        }
    }
    protected override bool Movable
    {
        get
        {
            return true;
        }
    }
    public override bool IsMultiAttack
    {
        get
        {
            return true;
        }
    }

    protected override void CharacterTransformChage(Transform model)
    {
        
        model.parent.Rotate(Vector3.up, 360 / animate.Sum(a=>a.length) * Time.deltaTime * 6);
    }


    public override float GetCurDamage(DamageType type, out bool isCritical, out float additionHit)
    {
        float dmg = user.GetCurDamage(EffectDamageType, out isCritical, out additionHit);
        return dmg * skillSetting.AdjustedDamage;
    }
}

