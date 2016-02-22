using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

public class IceBlade : EmissionSkillEffect
{
    public override DamageType DirectDamageType
    {
        get
        {
            return DamageType.Skill;
        }
    }

    public override DamageType EffectDamageType
    {
        get
        {
            return DamageType.Skill;
        }
    }

    public override float ExistTime
    {
        get
        {
            return 3f;
        }
    }

    public override float MinRange
    {
        get
        {
            return 0;
        }
    }

    public override float MoveSpeed
    {
        get
        {
            return 2.2f;
        }
    }

    public override float Radius
    {
        get
        {
            return .1f;
        }
    }

    public override float Range
    {
        get
        {
            return 2.2f;
        }
    }

    protected override bool Movable
    {
        get
        {
            return false;
        }
    }
    Transform blade1;
    Vector3 blade1Dir;
    Transform blade2;
    Vector3 blade2Dir;
    bool blade2Start = false;
    float blade2Delay;
    Transform blade3;
    Vector3 blade3Dir;
    bool blade3Start = false;
    float blade3Delay;
    void Awake() {
        blade1 = transform.FindChild("iceBlade");
        blade2 = transform.FindChild("iceBlade2");
        blade2.GetComponent<Collider>().enabled = false;
        blade2Delay = blade2.GetComponent<ParticleSystem>().startDelay;
        blade3 = transform.FindChild("iceBlade3");
        blade3.GetComponent<Collider>().enabled = false;
        blade3Delay = blade3.GetComponent<ParticleSystem>().startDelay;
    }
    protected override void SkillStart()
    {
        base.SkillStart();
        blade1Dir = user.model.Find("BodyCenter").forward;
        Invoke("SetBlade2Pos", blade2Delay);
        Invoke("SetBlade3Pos", blade3Delay);
    }
    void SetBlade2Pos()
    {
        blade2.position = user.model.Find("BodyCenter").position;        
        blade2Dir = user.model.Find("BodyCenter").forward;
        blade2.Rotate(Vector3.up, Vector3.Angle(blade2.right, blade2Dir)* (Vector3.Dot(blade2.right, user.model.Find("BodyCenter").right) <=0?1:-1), Space.World);
        blade2.GetComponent<Collider>().enabled = true;
        blade2Start = true;
    }
    void SetBlade3Pos()
    {
        blade3.position = user.model.Find("BodyCenter").position;
        blade3Dir = user.model.Find("BodyCenter").forward;
        blade3.Rotate(Vector3.up,Vector3.Angle(blade3.right, blade3Dir) * (Vector3.Dot(blade3.right, user.model.Find("BodyCenter").right) <= 0 ? 1 : -1), Space.World);
        
        blade3.GetComponent<Collider>().enabled = true;
        blade3Start = true;
    }
    protected override void Update()
    {
        if (Time.time - timer > ExistTime)
            Destroy(gameObject);
        if (IsPlay)
        {
            if(blade1!=null)
                blade1.position += blade1Dir * Time.deltaTime * MoveSpeed;
            if (blade2 != null && blade2Start)
                blade2.position += blade2Dir * Time.deltaTime * MoveSpeed;
            if (blade3 != null && blade3Start)
                blade3.position += blade3Dir * Time.deltaTime * MoveSpeed;           
        }
    }
    public override float GetCurDamage(DamageType type, out bool isCritical, out float additionHit)
    {
        return user.GetCurDamage(DamageType.Skill,out isCritical,out additionHit) * skillSetting.AdjustedDamage;
    }

    public override bool ShouldCast(NPCController caster, List<Transform> TargetsInVision, BaseSkill skillSetting)
    {
        return ShouldCastRangeAttackPredefine(caster, TargetsInVision, skillSetting);
    }
}
