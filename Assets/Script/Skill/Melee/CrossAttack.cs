using UnityEngine;
using System.Collections;

public class CrossAttack : MeleeSkillEffect
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
        base.CharacterTransformChage(model);
    }

    public override float GetCurDamage(DamageType type, out bool isCritical, out float additionHit)
    {
        float dmg = user.GetCurDamage(EffectDamageType, out isCritical, out additionHit);
        return dmg * skillSetting.AdjustedEffectDamage;
    }
}
