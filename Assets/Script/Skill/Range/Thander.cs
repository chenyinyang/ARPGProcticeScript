using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

public class Thander : RangeBuffedSkillEffect
{
    public override DamageType EffectDamageType
    {
        get
        {
            return DamageType.Magic;
        }
    }

    public override float MinRange
    {
        get { return 0; }
    }

    public override float Radius
    {
        get { return 1.5f; }
    }

    public override float Range
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

    protected override bool ImmediateMoveToTarget
    {
        get
        {
            return true;
        }
    }
    private BaseCharacterBehavior toTarget;
    private float directDamege;
    public override void Play(Transform model, BaseSkill skillSetting, BaseCharacterBehavior toTarget = null)
    {
        this.toTarget = toTarget;
        base.Play(model, skillSetting, toTarget);
        directDamege = skillSetting.AdjustedDamage * user.status.GetSecondaryAttrubute(SecondaryAttributeName.MagicalDamage).AdjustedValue;
    }
    protected override BuffSkill Createbuff()
    {
        
        BuffSkill sk = new BuffSkill(SkillName.Thander, 0, 0, 0, 0, 0, ConsumedAttributeName.Health, 0, 6f, new Buff[] {
            new ConsumedStatusBuff(ConsumedAttributeName.Health,Buff.BuffType.Absolute,0,
                0,
                0,0f),
            new Buff(SecondaryAttributeName.MoveSpeed, Buff.BuffType.Relative, -.9f)
        }, false);
        sk.effect = DebuffEffectPrefab;
        return sk;
    }

    protected override List<BaseCharacterBehavior> GetTargetInRadious(List<BaseCharacterBehavior> npcInArea)
    {   
        var random3Tar = new List<BaseCharacterBehavior>();
        for (int i = 0; i < 3; i++)
        {
            if (npcInArea.Count > 0)
            {
                if (toTarget != null)
                {
                    npcInArea.Sort((a, b) => a.status.GetConsumedAttrubute(ConsumedAttributeName.Health).LossValue.CompareTo(b.status.GetConsumedAttrubute(ConsumedAttributeName.Health).LossValue));
                    random3Tar.Add(npcInArea[0]);
                    npcInArea.RemoveAt(0);
                    //Debug.Log("Third");
                }
                else
                {
                    var t = npcInArea[UnityEngine.Random.Range(0, npcInArea.Count - 1)];
                    random3Tar.Add(t);
                    npcInArea.Remove(t);
                }
            }
        }
        if (user is Player && toTarget ==null)
        {
            foreach (var item in random3Tar)
            {
                BaseSkill sk = new BaseSkill(SkillName.Thander, 1.5f, 0, 0, 0, 0, ConsumedAttributeName.Health, 0);
                sk.effect = ResourceLoader.Skill.GetSkillPrefab(SkillName.Thander);
                sk.SetCaster(user);
                //Debug.Log("tRAINING");
                sk.Cast(user,item);
            }
        }
        return random3Tar;

    }

    protected override BaseCharacterBehavior GetTargetInRange(List<BaseCharacterBehavior> npcInArea)
    {
        if (this.toTarget != null)
            return this.toTarget;
        BaseCharacterBehavior t;
        //取得作用對象
        if (npcInArea.Count == 0)
            t = null;
        else
        {               
            npcInArea.Sort((a, b)
                => b.status.GetConsumedAttrubute(ConsumedAttributeName.Health).LossValue
                            .CompareTo(a.status.GetConsumedAttrubute(ConsumedAttributeName.Health).LossValue));
            t = npcInArea[0];
            //Debug.Log(npcInArea.Count);
            
        }
        if (user is Player)
        {
            var temp = npcInArea;
            if(npcInArea.Count>0)
                for (int i = 0; i < 5; i++)
                {
                
                    var t2 = npcInArea[UnityEngine.Random.Range(0, npcInArea.Count - 1)];
                    BaseSkill sk = Createbuff();
                    sk.effect = DebuffEffectPrefab;
                    sk.SetCaster(user);
                    //Debug.Log("First");
                    sk.Cast(t2);
                    t2.Damage(EffectDamageType, this, user);
                }
            
            
        }
        return t;
    }

    public override bool ShouldCast(NPCController caster, List<Transform> TargetsInVision, BaseSkill skillSetting)
    {
        return ShouldCastRangeAttackPredefine(caster, TargetsInVision,skillSetting);
    }
    public override float GetCurDamage(DamageType type, out bool isCritical, out float additionHit)
    {
        float dmg = user.GetCurDamage(EffectDamageType, out isCritical, out additionHit);
        return dmg  * skillSetting.AdjustedDamage;
    }
}
