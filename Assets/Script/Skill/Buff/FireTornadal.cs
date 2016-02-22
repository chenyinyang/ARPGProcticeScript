using UnityEngine;
using System.Collections;

using System;
using System.Collections.Generic;

public class FireTornadal : EmissionSkillEffect
{
    protected override bool Movable
    {
        get
        {
            return true;
        }
    }
    Transform targetEnemy;
    
    float GroundFireSubEmission;
    float startEmissionRate;
    float bodyStartEmissionRate;
    List<Transform> hitByUperStream;
    public override DamageType EffectDamageType { get { return DamageType.Magic; } }
    public override DamageType DirectDamageType { get { return DamageType.Magic; } }
    public override float Radius
    {
        get

        {
            
            return 0.03f;
        }
    }
    public override float MoveSpeed { get { return 1f; } }
    private float fBodyExistTime;
    public override float ExistTime { get { return GetComponent<ParticleSystem>().duration + transform.Find("GroundFireSubEmission").GetComponent<ParticleSystem>().duration; } }
    public override void Prepare(Transform model, float castimeTime)
    {
        base.Prepare(model, castimeTime);
        fBodyExistTime = GetComponent<ParticleSystem>().duration;
        GetComponent<ParticleSystem>().startDelay = castimeTime;
        GroundFireSubEmission = transform.Find("GroundFireSubEmission").GetComponent<ParticleSystem>().duration;
        startEmissionRate = GetComponent<ParticleSystem>().emissionRate;
        bodyStartEmissionRate = transform.Find("FireTornadalBady").GetComponent<ParticleSystem>().emissionRate;
    }
    void Awake() {
        hitByUperStream = new List<Transform>();
    }
    protected override void Start()
    {

        transform.localRotation = Quaternion.Euler(-90, 0, 0);
        transform.position = new Vector3(transform.position.x, 0, transform.position.z);
        base.Start();
        moveDir = transform.up*-1;
    }
    protected override void Update()
    {
        transform.Rotate(Vector3.forward, 720 * Time.deltaTime);
        base.Update();
        //停止了
        if (moveDir == Vector3.zero)
        {
            //噴火 停下來時噴火直到剩下2秒+火燒時間
            if (ExistTime - (Time.time - costTimer) > GroundFireSubEmission + 2f)
            {
                //GetComponent<ParticleSystem>().startSpeed = MoveSpeed;
                //GetComponent<ParticleSystem>().startLifetime = 1f;
                GetComponent<ParticleSystem>().Emit(200);
            }
            if (ExistTime - (Time.time - costTimer) < (GroundFireSubEmission + 2f))
            {
                transform.Find("FireTornadalBady").GetComponent<ParticleSystem>().emissionRate = 0;//-= bodyStartEmissionRate * Time.time/ transform.Find("FireTornadalBady").GetComponent<ParticleSystem>().startLifetime;
                hitByUperStream.Clear();
            }
            //身體淡出
            if (ExistTime - (Time.time - costTimer) < 3f + GroundFireSubEmission)
                transform.Find("FireTornadalBady").localScale -= new Vector3(1, 1, 0) * Time.deltaTime * GroundFireSubEmission;
            else
                transform.Find("FireTornadalBady").localScale += new Vector3(2, 2, 0) * Time.deltaTime * 1;
            GetComponent<ParticleSystem>().emissionRate -= startEmissionRate * Time.time / GroundFireSubEmission;
        }
        foreach (Transform hitTar in hitByUperStream)
        {
            hitTar.position += new Vector3(0, 1.3F, 0) * Time.deltaTime;
            hitTar.position += (hitTar.position -transform.position) * Time.deltaTime;
        }
    }
    public override void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(Tags.Monster))
        {
            targetEnemy = other.transform;
        }
        base.OnTriggerEnter(other);
    }

    protected override Vector3? ModifyMoveDir(Vector3? moveDir)
    {
        //還有三秒結束
        if (ExistTime - (Time.time - costTimer) < 3f + GroundFireSubEmission)
        {
            //還具有速度,停止並且設定噴火
            if (moveDir != Vector3.zero)
            {
                GetComponent<ParticleSystem>().startSpeed = MoveSpeed;
                GetComponent<ParticleSystem>().startLifetime = 1f;
                transform.position += Vector3.up * .1f;
                transform.Find("FireTornadalBady").transform.position -= Vector3.up * .1f;
                transform.Find("FireTornadalBady").GetComponent<ParticleSystem>().startSpeed = MoveSpeed;
            }
            return Vector3.zero;
        }
        if (targetEnemy != null)
        {
            if (user.CanDamageTarget(targetEnemy.GetComponent<Damagable>()))
                return (targetEnemy.position - transform.position).normalized;
            else
            {
                if (moveDir != Vector3.zero)
                {
                    GetComponent<ParticleSystem>().startSpeed = MoveSpeed;
                    GetComponent<ParticleSystem>().startLifetime = 1f;
                    transform.position += Vector3.up * .1f;
                    transform.Find("FireTornadalBady").transform.position -= Vector3.up * .1f;
                    transform.Find("FireTornadalBady").GetComponent<ParticleSystem>().startSpeed = MoveSpeed;
                }
                return Vector3.zero;
            }
        }
        return base.ModifyMoveDir(moveDir);
    }
    
    public override void ParticleCollision(GameObject particle, Damagable character)
    {
        base.ParticleCollision(particle, character);
        if (particle.name == "FireTornadalBady") {
            
            if (moveDir == Vector3.zero) {
                if (!hitByUperStream.Contains(character.transform))
                    hitByUperStream.Add(character.transform);
                Debug.Log("Body hit");
                //character.transform.position += new Vector3(0, .3f, 0);
            }
        }
    }
    public override bool ShouldCast(NPCController caster, List<Transform> TargetsInVision, BaseSkill skillSetting)
    {
        return ShouldCastRangeAttackPredefine(caster, TargetsInVision, skillSetting);
    }

    public override float GetCurDamage(DamageType type, out bool isCritical, out float additionHit)
    {
        float dmg = user.GetCurDamage(EffectDamageType, out isCritical, out additionHit);
        return dmg * skillSetting.AdjustedEffectDamage;
    }
}
