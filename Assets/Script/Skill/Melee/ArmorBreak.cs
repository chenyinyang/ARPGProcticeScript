using UnityEngine;
using System.Collections.Generic;
using System;

public class ArmorBreak : MeleeSkillEffect
{
    public GameObject DebuffEffectPrefab;
    public override bool IsMultiAttack
    {
        get
        {
            return false;
        }
    }
    protected override bool Movable
    {
        get
        {
            return true;
        }
    }
    public override DamageType EffectDamageType
    {
        get
        {
            return DamageType.Skill;
        }
    }
    
    void Start() {
        
    }
   
    BuffSkill CreateArmorBreakDebuff()
    {       
        BuffSkill sk = new BuffSkill(SkillName.ArmorBreak, 0, 0, 0, 0, 0, ConsumedAttributeName.Health, 0, 9f, new Buff[] {            
            new Buff(SecondaryAttributeName.PhysicalDeffence, Buff.BuffType.Relative,-.5f),
            new Buff(SecondaryAttributeName.MagicalDeffence, Buff.BuffType.Relative,-.5f),
            new Buff(SecondaryAttributeName.SkillDeffence, Buff.BuffType.Relative,-.5f),
            new Buff(PrimaryAttributeName.Agility, Buff.BuffType.Relative,-.5f)
        });
        sk.effect = DebuffEffectPrefab;
        return sk;
    }
    protected override void AddEffectOnWeaponHit(Damagable character)
    {
        if (character.GetComponent<BaseCharacterBehavior>() != null)
        {
            BuffSkill sk = CreateArmorBreakDebuff();
            sk.SetCaster(user);
            //Debug.Log(target.name + "on debuff");
            sk.Cast(character.GetComponent<BaseCharacterBehavior>());
        }
        
    }
    public override float GetCurDamage(DamageType type, out bool isCritical, out float additionHit)
    {
        float dmg = user.GetCurDamage(EffectDamageType, out isCritical, out additionHit);
        return dmg * skillSetting.AdjustedDamage;
    }
}
