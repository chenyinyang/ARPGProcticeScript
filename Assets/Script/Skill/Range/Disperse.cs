using UnityEngine;
using System.Collections;
using System.Linq;
using System.Collections.Generic;

public class Disperse : RangeBuffedSkillEffect
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
            return .5f;
        }
    }

    public override float Range
    {
        get
        {
            return 2f;
        }
    }

    public override SkillTarget targetType
    {
        get
        {
            return SkillTarget.All;
        }
    }

    protected override bool ImmediateMoveToTarget
    {
        get
        {
            return true;
        }
    }

    public override bool ShouldCast(NPCController caster, List<Transform> TargetsInVision, BaseSkill skillSetting)
    {
        
        int buffOrDebuffCount = 0;
        foreach (var t in TargetsInVision)
        {
            bool isFriend = SkillToTarget(caster, t.GetComponent<BaseCharacterBehavior>(), SkillTarget.Friend);
            bool isEnemy = SkillToTarget(caster, t.GetComponent<BaseCharacterBehavior>(), SkillTarget.Enemy);
            foreach (BuffSkill buff in t.GetComponent<BaseCharacterBehavior>().buffs)
            {
                if (isEnemy && SkillType.BuffSkills.Contains(buff.skillName))
                    buffOrDebuffCount++;
                if (isFriend && SkillType.DeBuffSkills.Contains(buff.skillName))
                    buffOrDebuffCount++;
            }
        }
        return buffOrDebuffCount > 5;
    }

    protected override BuffSkill Createbuff()
    {
        return null;
    }

    protected override List<BaseCharacterBehavior> GetTargetInRadious(List<BaseCharacterBehavior> npcInArea)
    {
        foreach (BaseCharacterBehavior character in npcInArea)
        {
            bool isFriend = SkillToTarget(user, character, SkillTarget.Friend);
            bool isEnemy = SkillToTarget(user, character, SkillTarget.Enemy);
            List<SkillName> buffToRemove = new List<SkillName>();
            foreach (BuffSkill buff in character.buffs)
            {
                if(isEnemy && SkillType.BuffSkills.Contains(buff.skillName))
                    buffToRemove.Add(buff.skillName);
                if (isFriend && SkillType.DeBuffSkills.Contains(buff.skillName))
                    buffToRemove.Add(buff.skillName);
            }
            foreach (SkillName name in buffToRemove)
            {
                character.RemoveBuff(name);
            }
        }
        return new List<BaseCharacterBehavior>() ;
    }

    protected override BaseCharacterBehavior GetTargetInRange(List<BaseCharacterBehavior> npcInArea)
    {
        BaseCharacterBehavior target=null;
        int maxInRadiousCount = 0;
        foreach (var t in npcInArea)
        {
            int inRadiousCount = 0;
            foreach (var near in npcInArea)
            {
                if (Vector3.Distance(near.transform.position, t.transform.position) < Radius)
                    inRadiousCount++;
            }
            if (inRadiousCount > maxInRadiousCount)
                target = t;
        }
        return target;
    }
    public override float GetCurDamage(DamageType type, out bool isCritical, out float additionHit)
    {
        isCritical = false;
        additionHit = 0;
        return 0;
    }
}
