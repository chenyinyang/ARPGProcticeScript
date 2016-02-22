using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public abstract class DelayDestroiedSkillEffect : SkillEffect {
  
    public abstract float MinRange { get; }
    public abstract float Radius { get; }
  
    
    protected override void SkillStart()
    {
        base.SkillStart();
    }
    protected override void SkillStop()
    {
        model = null;
    }
    protected bool ShouldCastRangeAttackPredefine(NPCController caster, List<Transform> TargetsInVision, BaseSkill skillSetting)
    {
        //往前方向量的投影
        Vector3 targetDir = caster.attackTarget.position - caster.transform.position;
        //水平距離
        float distanceToTarget = Vector3.ProjectOnPlane(targetDir, Vector3.up).magnitude;
        //值線上跟目標最近的方向點(<怪-我>在正前方的投影像量
        Vector3 tarPos = Vector3.Project(targetDir,caster.transform.forward);
        if (MinRange < distanceToTarget)
        {
            //如果點的距離超過範圍,就用最大範圍來當目標點
            if (tarPos.magnitude > Range)
            {
                tarPos = caster.transform.forward * Range;
            }
            //換算成作標
            tarPos += caster.transform.position;
            //目標作標與敵人座標小於半徑
            return Vector3.Distance(tarPos, caster.attackTarget.position) < Radius;
        }
        return false;
    }
    protected bool ShouldCastSelfHealingPredefine(NPCController caster, List<Transform> TargetsInVision, BaseSkill skillSetting)
    {
        //技能總治療量
        float healingMount = 0;
        if (skillSetting is BuffSkill)
        {
            foreach (Buff buff in (skillSetting as BuffSkill).buffs)
            {
                if (buff is ConsumedStatusBuff)
                {
                    healingMount += (buff as ConsumedStatusBuff).TotalMount((skillSetting as BuffSkill).buffTime);
                }
            }
        }
        healingMount += caster.status.GetSecondaryAttrubute(SecondaryAttributeName.MagicalDamage).AdjustedValue * skillSetting.AdjustedDamage;
        return caster.status.GetConsumedAttrubute(ConsumedAttributeName.Health).LossValue > healingMount / 2;
    }
    protected bool ShouldCastRangeHealingPredefine(NPCController caster, List<Transform> TargetsInVision, BaseSkill skillSetting)
    {
        //技能總治療量
        float healingMount = 0;
        if (skillSetting is BuffSkill)
        {
            foreach (Buff buff in (skillSetting as BuffSkill).buffs)
            {
                if (buff is ConsumedStatusBuff)
                {
                    healingMount += (buff as ConsumedStatusBuff).TotalMount((skillSetting as BuffSkill).buffTime);
                }
            }
        }
        healingMount += caster.status.GetSecondaryAttrubute(SecondaryAttributeName.MagicalDamage).AdjustedValue * skillSetting.AdjustedDamage;
        foreach (Transform target in TargetsInVision)
        {
            if (target.GetComponent<BaseCharacterBehavior>().status.GetConsumedAttrubute(ConsumedAttributeName.Health).LossValue > healingMount / 2)
                return true;
        }
        return false;
    }
}
