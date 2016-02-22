using UnityEngine;
using System.Collections;
//using UnityEditor;
using System;

public abstract class EmissionSkillEffect : DelayDestroiedSkillEffect {
    
    protected Vector3? moveDir;
    protected float timer = 0;
    public abstract DamageType DirectDamageType { get; }
    public abstract float MoveSpeed { get; }
    public abstract float ExistTime { get; }
    public override float Range
    {
        get
        {
            return MoveSpeed * ExistTime;
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
            return 0;
        }
    }
    public bool IsPlay { get; protected set; }
    Collider myCollider;
    public override void Prepare(Transform model, float castimeTime)
    {
        IsPlay = false;
        base.Prepare(model, castimeTime);
        //moveDir = model.forward;
        myCollider = GetComponent<Collider>();
        if (myCollider != null)
            myCollider.enabled = false;
    }
    public override void Play(Transform model, BaseSkill skillSetting,BaseCharacterBehavior castTo=null)
    {
        base.Play(model, skillSetting);
        IsPlay = true;
        if (myCollider != null)
            myCollider.enabled = true;
        if (moveDir==null)
            moveDir = transform.forward;
    }
    protected virtual void Start()
    {
        timer = Time.time;
        //transform.localRotation = Quaternion.Euler(-90, 0, 0);
        //transform.position = new Vector3(transform.position.x, 0, transform.position.z);
        //transform.Find("FireTornadalBady").localRotation = Quaternion.Euler(-90, 0, 0);
        transform.parent = null;
        
    }
    protected virtual void Update()
    {
        if (Time.time - timer > ExistTime)
            Destroy(gameObject);      
        if (IsPlay)
        {
            moveDir = ModifyMoveDir(moveDir);
            if (moveDir != null)
                transform.position += (Vector3)moveDir * Time.deltaTime * MoveSpeed;
        }
    }
    protected virtual Vector3? ModifyMoveDir(Vector3? moveDir) {
        return moveDir;
    }
    public virtual void OnTriggerEnter(Collider other)
    {
        //Debug.Log("OnTriggerEnter" + other.gameObject.name);
        if (other.transform != user.transform)
        {
            Damagable hitTarget = other.transform.GetComponent<Damagable>();
            if (IsPlay && hitTarget != null && user.CanDamageTarget(hitTarget))
            {   
                hitTarget.Damage(DirectDamageType, new DirectDamageMaker(user,DirectDamageType,skillSetting.AdjustedDamage),user);
            }
            //Destroy(gameObject);
        }
    }
    public class DirectDamageMaker : IDamageMaker
    {
        DamageType type;
        float damageRatio;
        BaseCharacterBehavior caster;
        public DirectDamageMaker(BaseCharacterBehavior caster,DamageType type,float damageRatio) {
            this.caster = caster;
            this.damageRatio = damageRatio;
            this.type = type;
        }
        public float GetCurDamage(DamageType type, out bool isCritical, out float additionHit)
        {
            float dmg = caster.GetCurDamage(this.type, out isCritical, out additionHit);
            return dmg * damageRatio;
        }
    }
    
}

