using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System;

public abstract class MeleeSkillEffect : SkillEffect {
    public virtual float DamageRatio { get; set; }
    public abstract bool IsMultiAttack { get; }
    protected List<Damagable> targetsHit;
    protected virtual void Awake()
    {
        targetsHit = new List<Damagable>();

    }
    void Start() {
       
    }
    protected override void SkillStart()
    {
        base.SkillStart();
        user.meleeWeapon.hitOnCharacter += MeleeWeapon_hitOnCharacter;
    }
    protected override void SkillStop()
    {
        base.SkillStop();
        user.meleeWeapon.hitOnCharacter -= MeleeWeapon_hitOnCharacter;
    }
    protected virtual void AddEffectOnWeaponHit(Damagable character) {

    }

    private void MeleeWeapon_hitOnCharacter(Damagable character)
    {
        if (!targetsHit.Contains(character))
        {
            targetsHit.Add(character);
        }
        else {
            if (!IsMultiAttack)
                return;
        }
        AddEffectOnWeaponHit(character);        
        character.Damage( EffectDamageType, this, user);
    }


    public override float Range
    {
        get
        {
            if ( user.GetComponent<AttackAI>() != null)
                return user.GetComponent<AttackAI>().attackRange;
            return 0;
        }
    }
    public override bool ShouldCast(NPCController caster, List<Transform> TargetsInVision,BaseSkill skillSetting)
    {
        return true;
    }
}
