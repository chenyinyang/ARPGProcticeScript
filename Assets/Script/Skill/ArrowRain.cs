using UnityEngine;
using System.Collections.Generic;
//[RequireComponent(typeof(ParticleSystem))]
//[ExecuteInEditMode]
public class ArrowRain : DelayDestroiedSkillEffect {


    public override DamageType EffectDamageType
    {
        get
        {
            return DamageType.Skill;
        }
    }
    
    ParticleSystem _ps;
    float _gravity = 9.8f*.3f;
    float flyTime;
    float destroyTimer;
    float startSpeed = .5f;
    float randomStartVFix = .05f;
    void Awake() {
        _ps = transform.Find("Arrow").GetComponent<ParticleSystem>();
    }
    public override void Play(Transform model, BaseSkill skillSetting, BaseCharacterBehavior castTo = null)
    {
        base.Play(model, skillSetting);
        if (user is NPCController)
        {
            Vector3 targetPos = (user as NPCController).attackTarget.position + (user as NPCController).attackTarget.GetComponent<CharacterController>().center;
            float dis = Vector3.ProjectOnPlane(targetPos - user.transform.position,Vector3.up ).magnitude;
            //Debug.Log(targetPos);
            //水平速度固定計算出飛行時間,飛行時間+重力計算出垂直速度
            var speedZ = startSpeed;
            // T = dX / (startSpeedX )
            // y = -_gravity / 2 * t * t + a * t + c;
            // v = -2gt + a
            // a = (y-c)/t + g/2*t
            flyTime = dis / speedZ;
            var speedY = (targetPos.y - transform.position.y) / flyTime + _gravity / 2 * flyTime; ;
            Vector3 sp = new Vector3(0, speedY, speedZ);            
            //轉軸 速度方向成為水平平面,改變起使速度為算出來的速度量
            _ps.transform.localRotation = Quaternion.LookRotation(sp);
            _ps.startSpeed = sp.magnitude;
        }
        else {
            flyTime = _ps.startLifetime;
        }
        
        destroyTimer = Time.time;
       
    }
    // Use this for initialization
 
    public override float Range
    {
        get
        {
            //水平速度 * 箭矢持續時間 -1秒
            return startSpeed *4;
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
            
            Vector3 maxVec = new Vector3(0,.5f- randomStartVFix, .5f).normalized;
            Vector3 lowestVec = new Vector3(0,.5f- randomStartVFix, .5f).normalized;
            //最大的飛行時間下,z的速度差
            return 4* randomStartVFix * startSpeed ;
        }
    }
    protected override bool Movable
    {
        get
        {
            return false;
        }
    }
    // Update is called once per frame
    void Update () {

        if (destroyTimer == 0)
        {
            return;
        }
        if (_ps.particleCount <= 10)
        {
            _ps.Emit(CreateNewParticle());
        }

        ParticleSystem.Particle[] particles = new ParticleSystem.Particle[_ps.particleCount];
        int particleCount = _ps.GetParticles(particles);
        for (int i=0;i<particles.Length;i++)
        {   
            Vector3 v = particles[i].velocity;
            if (v != Vector3.zero)
            {
                particles[i].axisOfRotation = Vector3.Cross(Vector3.down, v);
                particles[i].rotation = Vector3.Angle(Vector3.down, v);
                
                particles[i].velocity -=new Vector3(0, _gravity*Time.deltaTime,0);
               
            }
        }
        _ps.SetParticles(particles, particleCount);

        if (Time.time - destroyTimer > flyTime + 1f)
            Destroy(gameObject);
    }    

    private ParticleSystem.Particle CreateNewParticle() {
        ParticleSystem.Particle p = new ParticleSystem.Particle();
        p.size = 0.01f;
        p.lifetime = flyTime+1f;
        p.position = _ps.transform.position;

        Vector3 v = new Vector3(
            _ps.transform.forward.x + Random.Range(-1* randomStartVFix, randomStartVFix),
            _ps.transform.forward.y + Random.Range(-randomStartVFix, randomStartVFix),
            _ps.transform.forward.z + Random.Range(-randomStartVFix, randomStartVFix));
        p.velocity = v.normalized * _ps.startSpeed;

        p.axisOfRotation = Vector3.Cross(Vector3.down, p.velocity);
        p.rotation = Vector3.Angle(Vector3.down, p.velocity);
        p.startLifetime = p.lifetime;
        return p;
    }
    public override bool ShouldCast(NPCController caster, List<Transform> TargetsInVision, BaseSkill skillSetting)
    {
        return ShouldCastRangeAttackPredefine(caster, TargetsInVision, skillSetting);
    }

    public override float GetCurDamage(DamageType type, out bool isCritical, out float additionHit)
    {
        float dmg = user.GetCurDamage(EffectDamageType, out isCritical, out additionHit);
        return dmg * skillSetting.AdjustedDamage;
    }
}
