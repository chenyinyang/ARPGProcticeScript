using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System;

public abstract class SkillEffect : MonoBehaviour,IDamageMaker {
    public enum EffectPosition {
        Weapon,
        BodyCenter,
        BodyGround
    }
    protected abstract bool Movable { get; }
    public abstract float Range { get; }
    public AnimationClip[] animate;        
    public EffectPosition effectPosition;

    protected BaseSkill skillSetting;
    public abstract DamageType EffectDamageType { get; }
    public BaseCharacterBehavior user { get; protected set; }
    protected float costTimer = 0;
    protected float playingTime;
    protected Transform model;
    public float CastTime { get; protected set; }
    public void SetCaster(BaseCharacterBehavior caster)
    {
        this.user = caster;
        
    }
    public virtual void Prepare(Transform model, float castTime)
    {
        CastTime = castTime;
    }
    public virtual void Play(Transform model, BaseSkill skillSetting,BaseCharacterBehavior toTarget = null)
    {
        this.skillSetting = skillSetting;
        this.model = model;     
        costTimer = Time.time;
        SkillStart();
        if (playingTime < animate.Sum(a => a.length))
            playingTime = animate.Sum(a => a.length);
        foreach (AnimationClip anim in animate)
        {
            if (model.GetComponent<Animation>()!=null)
            {

                model.GetComponent<Animation>()[anim.name].layer = 6;
                model.GetComponent<Animation>()[anim.name].AddMixingTransform(transform);
                model.GetComponent<Animation>().PlayQueued(anim.name);
            }
        }
        
        if (!Movable)
            user.IsSkilling = true;
        
        
    }
    protected virtual void FixedUpdate()
    {        
        if (model != null)
        {
            if (Time.time - costTimer < playingTime)
            {
                CharacterTransformChage(model);
            }
            else
            {
                if (!Movable)
                    user.IsSkilling = false;
                SkillStop();
            }
        }
    }
    protected virtual void CharacterTransformChage(Transform model) {
        //user.Rotate(Vector3.up, 360 / animate.length * Time.deltaTime * 3);
    }


    protected virtual void SkillStart() { }
    protected virtual void SkillStop() {
        GameObject.Destroy(gameObject);
        model = null;
    }
    public virtual void ParticleCollision(GameObject particle, Damagable character) { }
    public abstract bool ShouldCast(NPCController caster, List<Transform> TargetsInVision,BaseSkill skillSetting);

    public abstract float GetCurDamage(DamageType type, out bool isCritical, out float additionHit);
}
