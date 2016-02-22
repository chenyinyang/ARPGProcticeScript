using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

public class HealingShield : RangeBuffedSkillEffect
{
    public override DamageType EffectDamageType
    {
        get
        {
            return DamageType.Nature;
        }
    }

    public override float MinRange
    {
        get
        {
            return 0;
        }
    }

    public override float Radius
    {
        get
        {
            return 1f;
        }
    }

    public override float Range
    {
        get
        {
            return 0;
        }
    }

    public override SkillTarget targetType
    {
        get
        {
            return SkillTarget.Friend;
        }
    }

    protected override bool ImmediateMoveToTarget
    {
        get
        {
            return true;
        }
    }

    protected override BuffSkill Createbuff()
    {
        Buff b = new Buff(PrimaryAttributeName.Luck, Buff.BuffType.Absolute, 0);
        b.onBuff = (tar) => {
            if (tar is BaseCharacterBehavior)
                (tar as BaseCharacterBehavior).OnDamaged += HealingShield_OnDamaged; ;
        };
        b.endBuff = (tar) => {
            if (tar && user && tar is BaseCharacterBehavior)
                (tar as BaseCharacterBehavior).OnDamaged -= HealingShield_OnDamaged;
        };
        BuffSkill sk = new BuffSkill(SkillName.HealingShield, 0, 0, 0, 0, 0, ConsumedAttributeName.Health, 0, 3f, new Buff[] {
            b,
            new Buff(StaticAttributeName.DamageTakeFix, Buff.BuffType.Absolute,-.5f)
        });
        sk.effect = DebuffEffectPrefab;
        return sk;
    }

    private void HealingShield_OnDamaged(float originalDamage, DamageType type, float damageCause, BaseCharacterBehavior attackTo, BaseCharacterBehavior attackFrom)
    {

        if (type != DamageType.Nature) {
            attackTo.Heal(originalDamage + damageCause,user);
        }
    }

    protected override List<BaseCharacterBehavior> GetTargetInRadious(List<BaseCharacterBehavior> npcInArea)
    {
        npcInArea.Sort(
            (a, b) => (a.status.GetConsumedAttrubute(ConsumedAttributeName.Health).CurValue
                        / a.status.GetConsumedAttrubute(ConsumedAttributeName.Health).AdjustedValue )
                    .CompareTo(b.status.GetConsumedAttrubute(ConsumedAttributeName.Health).CurValue / b.status.GetConsumedAttrubute(ConsumedAttributeName.Health).AdjustedValue));
        List<BaseCharacterBehavior> targets = new List<BaseCharacterBehavior>();
        for (int i = 0; i < (npcInArea.Count < 1 ? 0 : npcInArea.Count); i++)
        {
            targets.Add(npcInArea[0]);
        }

        return targets;
    }

    protected override BaseCharacterBehavior GetTargetInRange(List<BaseCharacterBehavior> npcInArea)
    {
        return user;
    }

    public override bool ShouldCast(NPCController caster, List<Transform> TargetsInVision,BaseSkill skillSetting) {        
        foreach (var t in TargetsInVision)
        {
            //視野忠每個友好目標是否在範圍中
            if (t != null 
                && RangeBuffedSkillEffect.SkillToTarget(caster, t.GetComponent<BaseCharacterBehavior>(), targetType) 
                && (t.position - caster.transform.position).magnitude < Radius)
            {
                float hpRatio = t.GetComponent<BaseCharacterBehavior>().status
                    .GetConsumedAttrubute(ConsumedAttributeName.Health).CurValue / t.GetComponent<BaseCharacterBehavior>().status
                    .GetConsumedAttrubute(ConsumedAttributeName.Health).AdjustedValue;
                if (hpRatio < .2f)
                {                   
                    return true;
                }
            }
        }
        return false;
    }
    public override float GetCurDamage(DamageType type, out bool isCritical, out float additionHit)
    {
        isCritical = false;
        additionHit = 0;
        return 0;
    }
}
