using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System;
public class Heal : RangeBuffedSkillEffect
{
    public override DamageType EffectDamageType
    {
        get
        {
            return DamageType.Magic;
        }
    }

    public override float Range
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
            return .5f;
        }
    }
    public override SkillTarget targetType
    {
        get
        {
            return SkillTarget.Friend;
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

    private BaseCharacterBehavior toTarget;
    private float healingMount;
    public override void Play(Transform model, BaseSkill skillSetting, BaseCharacterBehavior toTarget = null)
    {
        this.toTarget = toTarget;
        healingMount = skillSetting.AdjustedDamage * user.status.GetSecondaryAttrubute(SecondaryAttributeName.MagicalDamage).AdjustedValue * user.status.GetStaticAttribute(StaticAttributeName.HealMake).AdjustedValue;

        base.Play(model, skillSetting, toTarget);
    }
    protected override BuffSkill Createbuff()
    {       
        BuffSkill sk = new BuffSkill(SkillName.Heal, 0, 0, 0, 0, 0, ConsumedAttributeName.Health, 0, 6f, new Buff[] {            
            new ConsumedStatusBuff(ConsumedAttributeName.Health,Buff.BuffType.Absolute,0,
            healingMount,
            healingMount/12,.5f),            
        },true);
        sk.effect = DebuffEffectPrefab;
        return sk;
    }
    protected override BaseCharacterBehavior GetTargetInRange(List<BaseCharacterBehavior> npcInArea)
    {
        if (this.toTarget != null)
            return this.toTarget;
        BaseCharacterBehavior t;

        //取得作用對象
        if (npcInArea.Count == 0)
            t = user;
        else
        {
            if (npcInArea.Contains(user) &&
                user.status.GetConsumedAttrubute(ConsumedAttributeName.Health).LossValue > healingMount)
                t = user;
            else
            {
                npcInArea.Sort((a, b)
                    => {
                        int r = (b.status.GetConsumedAttrubute(ConsumedAttributeName.Health).LossValue)
                            .CompareTo(a.status.GetConsumedAttrubute(ConsumedAttributeName.Health).LossValue);                       
                        if (r == 0)
                        {
                            
                            if (a is Player)
                                return 1;
                            if (b is Player)
                                return -1;
                        }
                        return r;
                    });
                t = npcInArea[0];

            }
        }
        if (user is Player)
        {
            foreach (BaseCharacterBehavior t2 in npcInArea)
            {
                if (t2 == user)
                    continue;
                BaseSkill sk = new BaseSkill(SkillName.Heal, 3f, 0, 0, 0, 0, ConsumedAttributeName.Health, 0);
                sk.effect = this.gameObject;
                sk.SetCaster(user);
                //Debug.Log("幫 " + target.CharacterName + " 上效果");
                sk.Cast(user, t2);
            }
        }
        return t;
    }

    protected override List<BaseCharacterBehavior> GetTargetInRadious(List<BaseCharacterBehavior> npcInArea)
    {
        return npcInArea;
    }
    public override bool ShouldCast(NPCController caster, List<Transform> TargetsInVision, BaseSkill skillSetting)
    {
        return ShouldCastRangeHealingPredefine(caster, TargetsInVision, skillSetting);
    }
    public override float GetCurDamage(DamageType type, out bool isCritical, out float additionHit)
    {
        isCritical = false;
        additionHit = 0;
        return 0;
    }
}
