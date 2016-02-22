using UnityEngine;
using System.Collections.Generic;

public class SkillGenerator:MonoBehaviour
{    
    private static SkillGenerator _skillGenerator = null;
    public static SkillGenerator Instance() {
        return _skillGenerator;
    }
    void Awake() {
        _skillGenerator = this;
        
    }
   
    public BaseSkill GetSkill(SkillName name) {
        BaseSkill sk;

        sk= typeof(SkillGenerator).GetMethod(name.ToString(), System.Reflection.BindingFlags.Instance| System.Reflection.BindingFlags.NonPublic).Invoke(this,null) as BaseSkill;                
        return sk;
    }
    private BaseSkill WindSlash() {
        return new BaseSkill(
            skillName:      SkillName.WindSlash,
            baseDamage:     1.5f,
            effectDamage:   0,
            buffedDamage:   0,
            baseCostValue:  50,
            buffedCostValue: 0,
            costType:       ConsumedAttributeName.Energy,
            skillCoolDown:  10f);
    }
    private BaseSkill CrossAttack()
    {
        return new BaseSkill(
         skillName:         SkillName.CrossAttack, 
         baseDamage:        2f,
         effectDamage:      .5f,
         buffedDamage:      0,
         baseCostValue:     50,
         buffedCostValue:   0,
         costType:          ConsumedAttributeName.Energy,
         skillCoolDown:     5f
         );
    }
    private BaseSkill WillOfFight()
    {        
        return new BuffSkill(
            skillName:      SkillName.WillOfFight,
            baseDamage:     0f,
            effectDamage:   0,
            buffedDamage:   0,
            baseCostValue:  200,
            buffedCostValue:0,
            costType:       ConsumedAttributeName.Energy,
            skillCoolDown:  10f,
            buffTime:       5f,
            buffs:          new Buff[] {
            new Buff(SecondaryAttributeName.PhysicalDeffence, Buff.BuffType.Relative,1f),
            new Buff(SecondaryAttributeName.MagicalDeffence, Buff.BuffType.Relative,1f),
            new Buff(SecondaryAttributeName.SkillDeffence, Buff.BuffType.Relative,1f),
            new Buff(SecondaryAttributeName.Dodge, Buff.BuffType.Relative,.5f),
            new Buff(StaticAttributeName.HealTakeFix, Buff.BuffType.Relative,.3f),
        });
    }
    private BaseSkill Recover()
    {
        return new BuffSkill(
            skillName:      SkillName.Recover,
            baseDamage:     0f,
            effectDamage:   0,
            buffedDamage:   0,
            baseCostValue:  50,
            buffedCostValue:0,
            costType:       ConsumedAttributeName.Mana,
            skillCoolDown:  15f,
            buffTime:       6,
            buffs:          new Buff[] {
            new ConsumedStatusBuff(ConsumedAttributeName.Health, Buff.BuffType.Absolute,0,300,150,.2f)            
        });
    }
    private BaseSkill FireTornadal()
    {
        return new BaseSkill(
            skillName:      SkillName.FireTornadal,
            baseDamage:     10f,
            effectDamage:   .5f,
            buffedDamage:   0,
            baseCostValue:  350,
            buffedCostValue:0,
            costType:       ConsumedAttributeName.Mana,
            skillCoolDown:  5f,
            castTime:       2f);
    }
    private BaseSkill ArrowRain()
    {
        return new BaseSkill(
            skillName:      SkillName.ArrowRain,
            baseDamage:     1.5f,
            effectDamage:   0f,
            buffedDamage:   0,
            baseCostValue:  250,
            buffedCostValue:0,
            costType:       ConsumedAttributeName.Energy,
            skillCoolDown:  12f);
    }
    private BaseSkill ImpactArrow()
    {
        return new BaseSkill(
            skillName:      SkillName.ImpactArrow,
            baseDamage:     1f,
            effectDamage:   .2f,
            buffedDamage:   0,
            baseCostValue:  350,
            buffedCostValue:0,
            costType:       ConsumedAttributeName.Energy,
            skillCoolDown:  20f,
            castTime:       2f);
    }
    private BaseSkill Taunt()
    {
        return new BaseSkill(
            skillName:      SkillName.Taunt, 
            baseDamage:     0,
            effectDamage:   0,
            buffedDamage:   0,
            baseCostValue:  100,
            buffedCostValue:0,
            costType:       ConsumedAttributeName.Energy, 
            skillCoolDown:  8f);
    }
    private BaseSkill ArmorBreak()
    {
        return new BaseSkill(
            skillName:      SkillName.ArmorBreak,
            baseDamage:     1.5f,
            effectDamage:   0,
            buffedDamage:   0,
            baseCostValue:  150,
            buffedCostValue:0,
            costType:       ConsumedAttributeName.Energy,
            skillCoolDown:  15f);
    }
    private BaseSkill Heal()
    {
        return new BaseSkill(
            skillName:      SkillName.Heal,
            baseDamage:     3f,
            effectDamage:   0,
            buffedDamage:   0,
            baseCostValue:  600,
            buffedCostValue:0,
            costType:       ConsumedAttributeName.Mana,
            skillCoolDown:  10f,
            castTime:       2.5f);
    }
    private BaseSkill Thander()
    {
        return new BaseSkill(
            skillName:  SkillName.Thander,
            baseDamage:     2.5f,
            effectDamage:   0,
            buffedDamage:   0,
            baseCostValue:  250,
            buffedCostValue:0,
            costType:       ConsumedAttributeName.Mana,
            skillCoolDown:  20f,
            castTime:       1f);
    }

    private BaseSkill HealingShield()
    {
        return new BaseSkill(
            skillName:      SkillName.HealingShield,
            baseDamage:     0,
            effectDamage:   0,
            buffedDamage:   0,
            baseCostValue:  350,
            buffedCostValue:0,
            costType:       ConsumedAttributeName.Mana, 
            skillCoolDown:  60f,
            castTime:       0);
    }

    private BaseSkill Disperse()
    {
        return new BaseSkill(
            skillName: SkillName.Disperse,
            baseDamage: 0,
            effectDamage: 0,
            buffedDamage: 0,
            baseCostValue: 350,
            buffedCostValue: 0,
            costType: ConsumedAttributeName.Mana,
            skillCoolDown: 15f,
            castTime: 0);
    }
    private BaseSkill FireWave()
    {
        return new BaseSkill(
            skillName: SkillName.FireWave,
            baseDamage: 1.5f,
            effectDamage: 0.2f,
            buffedDamage: 0,
            baseCostValue: 350,
            buffedCostValue: 0,
            costType: ConsumedAttributeName.Mana,
            skillCoolDown: 8f,
            castTime: 2f);
    }
    private BaseSkill IceBlade()
    {
        return new BaseSkill(
            skillName: SkillName.IceBlade,
            baseDamage: 1.5f,
            effectDamage: 0f,
            buffedDamage: 0,
            baseCostValue: 250,
            buffedCostValue: 0,
            costType: ConsumedAttributeName.Energy,
            skillCoolDown: 8f,
            castTime: 0f);
    }
}

