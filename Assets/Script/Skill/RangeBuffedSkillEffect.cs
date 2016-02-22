using UnityEngine;
using System.Collections.Generic;
[RequireComponent(typeof(SphereCollider))]


public abstract class RangeBuffedSkillEffect : DelayDestroiedSkillEffect {
    public enum SkillTarget {
        Friend,
        Enemy,
        All
    }
    protected override bool Movable
    {
        get
        {
            return true;
        }
    }
    public static bool SkillToTarget(BaseCharacterBehavior user, BaseCharacterBehavior other, SkillTarget type) {
        switch (type)
        {
            //友好
            case SkillTarget.Friend:
                if (user.tag == Tags.Player || user.tag == Tags.Friend)
                {
                    //不是玩家跟友方
                    if (other.tag != Tags.Friend && other.tag != Tags.Player)
                        return false;
                }
                else
                {
                    //不是自己人
                    if (other.tag != user.tag)
                        return false;
                }
                break;
            case SkillTarget.Enemy:
                //不能互相攻擊的不算
                if (!user.CanDamageTarget(other.GetComponent<Damagable>()) || !other.CanDamageTarget(user.GetComponent<Damagable>()))
                    return false;
                break;
            case SkillTarget.All:
            default:
                break;
        }
        return true;
    }
    
    protected abstract bool ImmediateMoveToTarget { get; }
    SphereCollider skillCollider;
    BaseCharacterBehavior targetInRange;
    List<BaseCharacterBehavior> npcInArea;
    public GameObject DebuffEffectPrefab;
    public GameObject SkillHitOnTargerEffectPrefab;
    public abstract SkillTarget targetType { get; }
    float waitColliderRangeIniTimer = 0f;
    float waitColliderRangeIniTime = 0.1f;
    float waitColliderRadiouIniTimer = 0f;
    float waitColliderRadiouIniTime = 0.1f;
    float minToTargetDistance = 0.01f;
    Vector3 moveDir;
    Vector3 targetPos;
    public override void Play(Transform model, BaseSkill skillSetting, BaseCharacterBehavior toTarget = null)
    {
        base.Play(model, skillSetting, toTarget);
        
    }
    void Awake()
    {
        npcInArea = new List<BaseCharacterBehavior>();
        skillCollider = GetComponent<SphereCollider>();
        if(skillCollider==null)
            skillCollider = gameObject.AddComponent<SphereCollider>();
        skillCollider.isTrigger = true;
        skillCollider.radius = Range;
        playingTime = (ImmediateMoveToTarget ?0:1f) + 0.1f+ waitColliderRangeIniTime+ waitColliderRadiouIniTime;
        
        
    }
    public override void Prepare(Transform model, float castTime)
    {
        base.Prepare(model, castTime);
        //playingTime += castTime;
    }
    protected virtual void Start() {
        transform.parent = null;
    }
    protected virtual void Update() {
        if (waitColliderRangeIniTimer != 0 && Time.time - waitColliderRangeIniTimer > waitColliderRangeIniTime) {
            //Debug.Log("collider蒐集完資料惹");
            waitColliderRangeIniTimer = 0;
            skillCollider.radius = 0;
            skillCollider.enabled = false;
            targetInRange = GetTargetInRange(npcInArea);
            npcInArea.Clear();
            if (targetInRange == null)
                return;
            //    targetInRange = user;
            targetPos = targetInRange.transform.position;
            moveDir = targetPos - transform.position;
            
        }

        
        if (targetInRange != null) {            
            if ((targetPos - transform.position).magnitude > moveDir.magnitude * Time.deltaTime)
            {
                if (!ImmediateMoveToTarget)
                    transform.position += moveDir * Time.deltaTime;//一秒到達
                else
                    transform.position += moveDir;
            }
            else {
                //已到達, 展開area radious
                skillCollider.radius = Radius;
                skillCollider.enabled = true;
                waitColliderRadiouIniTimer = Time.time;
                targetInRange = null;
                if (SkillHitOnTargerEffectPrefab != null) {
                    GameObject.Instantiate(SkillHitOnTargerEffectPrefab, targetPos, Quaternion.identity);
                }
                //Debug.Log("開始蒐集radious 候過waitColliderRadiouIniTime時間");
            }
        }
        
        if (waitColliderRadiouIniTimer != 0 && Time.time - waitColliderRadiouIniTimer > waitColliderRadiouIniTime) {            
            //Debug.Log("幫npc上效果");
            waitColliderRadiouIniTimer = 0;
            List<BaseCharacterBehavior> targetsInRadious = GetTargetInRadious(npcInArea);
            foreach (BaseCharacterBehavior target in targetsInRadious)
            {
                BuffSkill sk = Createbuff();
                if (sk != null)
                {
                    sk.SetCaster(user);
                    //Debug.Log("幫 "+ target.CharacterName+ " 上效果");
                    sk.Cast(target);
                    target.Damage(EffectDamageType, this, user);
                }
            }
        }
    }
    protected override void SkillStart()
    {
        base.SkillStart();
        //Debug.Log("Waiting collider deteck target in range");
        waitColliderRangeIniTimer = Time.time;

    }
    protected override void SkillStop()
    {
        //Debug.Log("Skill Stop " + Time.time);
        base.SkillStop();
        Destroy(this.gameObject);
    }
    protected abstract BuffSkill Createbuff();
    
    //TriggerInRadious
   protected virtual void OnTriggerEnter(Collider other)
    {
        //Debug.Log("OnTriggerEnter : " + other.name);
        BaseCharacterBehavior npc = other.GetComponent<BaseCharacterBehavior>();
        if (npc != null)
        {
            if (!npcInArea.Contains(npc))
            {
                if (!SkillToTarget(user, npc, targetType))
                    return;                
                npcInArea.Add(other.GetComponent<BaseCharacterBehavior>());
                if (waitColliderRangeIniTimer != 0)
                    waitColliderRangeIniTimer = Time.time;
                if (waitColliderRadiouIniTimer != 0)
                    waitColliderRadiouIniTimer = Time.time;
            }
        }
    }
    protected abstract BaseCharacterBehavior GetTargetInRange(List<BaseCharacterBehavior> npcInArea);
    protected abstract List<BaseCharacterBehavior> GetTargetInRadious(List<BaseCharacterBehavior> npcInArea);
    protected virtual void HitOnTarget() { }
   
}
