using UnityEngine;
using System.Collections.Generic;
using System;

public class Taunt : RangeBuffedSkillEffect// DelayDestroiedSkillEffect
{

    public override DamageType EffectDamageType
    {
        get
        {
            return DamageType.Nature;
        }
    }

    public override float Range
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

    public override SkillTarget targetType
    {
        get
        {
            return SkillTarget.Enemy;
        }
    }

    public override float MinRange
    {
        get { return 0; }
    }

    protected override bool ImmediateMoveToTarget
    {
        get
        {
            return false;
        }
    }

    protected override BuffSkill Createbuff()
    {
        Buff b = new Buff(PrimaryAttributeName.Luck, Buff.BuffType.Absolute, 0);
        b.onBuff = (tar) => {
            if (tar is NPCController)
                (tar as NPCController).AttackTo(user.transform);
        };
        b.endBuff = (tar) => {
            if (tar && user && tar is NPCController)
                (tar as NPCController).RemoveAttackTo(user.transform);
        };
        BuffSkill sk = new BuffSkill(SkillName.Taunt, 0, 0, 0, 0, 0, ConsumedAttributeName.Health, 0, 3f, new Buff[] {
            b,
            new Buff(SecondaryAttributeName.PhysicalDeffence, Buff.BuffType.Relative,-.2f),
            new Buff(SecondaryAttributeName.PhysicalDamage, Buff.BuffType.Relative,-.2f)
        });
        sk.effect = DebuffEffectPrefab;
        return sk;
    }
    protected override BaseCharacterBehavior GetTargetInRange(List<BaseCharacterBehavior> npcInArea)
    {
        return user;
    }

    protected override List<BaseCharacterBehavior> GetTargetInRadious(List<BaseCharacterBehavior> npcInArea)
    {
        npcInArea.Sort((a, b) =>(a.transform.position-user.transform.position).magnitude.CompareTo((b.transform.position - user.transform.position).magnitude)*-1 );
        List<BaseCharacterBehavior> targets = new List<BaseCharacterBehavior>();
        for (int i = 0; i < (npcInArea.Count>2?3:npcInArea.Count); i++)
        {
            targets.Add(npcInArea[i]);
        }
        return targets;
    }
    public override bool ShouldCast(NPCController caster, List<Transform> TargetsInVision, BaseSkill skillSetting)
    {
        return ShouldCastRangeAttackPredefine(caster, TargetsInVision,skillSetting);
    }
    public override float GetCurDamage(DamageType type, out bool isCritical, out float additionHit)
    {
        isCritical = false;
        additionHit = 0;
        return 0;
    }
}
