using UnityEngine;
using System.Collections.Generic;
using System;

public class Recover : DelayDestroiedSkillEffect {

    public override DamageType EffectDamageType
    {
        get
        {
            return DamageType.Nature;
        }
    }
    protected override bool Movable
    {
        get
        {
            return true;
        }
    }
    public override float MinRange
    {
        get { return 0; }
    }

    public override float Radius
    {
        get { return 0; }
    }

    public override float Range
    {
        get { return 0; }
    }
    public override bool ShouldCast(NPCController caster, List<Transform> TargetsInVision, BaseSkill skillSetting)
    {
        return ShouldCastSelfHealingPredefine(caster, TargetsInVision, skillSetting);
    }
    public override float GetCurDamage(DamageType type, out bool isCritical, out float additionHit)
    {
        isCritical = false;
        additionHit = 0;
        return 0;
    }
}
