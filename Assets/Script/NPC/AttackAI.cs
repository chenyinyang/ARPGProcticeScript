using UnityEngine;
using System.Collections.Generic;
using System.Linq;
[RequireComponent(typeof(NPCController))]
public class AttackAI : MonoBehaviour {
    public virtual float attackRange { get; protected set; }
    protected NPCController npc;
    protected Transform myTransform;
    protected float healThread;
    const float UPDATE_INTERVAL = .1f;
    float updateTimer = 0;
    protected virtual void Awake() {
        healThread = .8f;
        myTransform = transform;
    }
    // Use this for initialization
    protected virtual void Start () {
        npc = GetComponent<NPCController>();
        
        attackRange = SetAttackRange();

    }

    // Update is called once per frame
    protected virtual void Update () {
        if (Time.time - updateTimer > UPDATE_INTERVAL)
        {
            updateTimer = Time.time;
            if (npc.IsDead)
                this.enabled = false;
            if (Time.time - buildingHitTimer > buildingHitContinueTime)
            {
                BuildCollisioned = null;
            }
            if (npc.IsCasting)
                return;

            //如果血量低於多少,有回血技能可用
            if (CastHealing())
                return;
            //有可攻擊的對象,而且不在攻擊中
            if (npc.attackTarget && !npc.IsAttacking && !npc.IsSkilling)
            {
                if (npc.skills.Length > 0)
                {
                    //如果有buff技能可用,且身上無buff
                    if (CastBuff())
                        return;
                    //如果對象在可施放的進戰技能的距離中,
                    if (CastMeleeSkill())
                        return;
                    //如果對象在可施放的遠程技能的距離中,
                    if (CastRangeSkill())
                        return;
                }
                //普通攻擊
                if (ShouldAttack(npc.attackTarget))
                {
                    npc.IsAttacking = true;
                }
            }
            else
            {
                //Debug.Log("IsAttacking ? " + npc.IsAttacking + " IsSkilling ? " + npc.IsSkilling);
            }
        }
	}
    protected virtual bool CastMeleeSkill()
    {        
        //取出是進戰的 而且沒再CD的技能 而且還有資源可以使用
        //而且效果是進戰的  而且目標小於最大距離
        BaseSkill[] meleeSkills = npc.status.Skills.Where(
             s =>
                 SkillType.MeleeSkills.Contains(s.skillName)
                 && s.CooldownLeft == 0
                 && npc.status.GetConsumedAttrubute(s.costType).CurValue > s.AdjustCostValue
                 && s.effect.GetComponent<MeleeSkillEffect>() != null
                 && ShouldAttack(npc.attackTarget)
                 ).ToArray();
        
        if (meleeSkills.Length > 0)
        {
            npc.Skill(npc.status.Skills.IndexOf(meleeSkills[Random.Range(0, meleeSkills.Length - 1)]) + 1);            
            return true;
        }
        return false;
    }
    protected virtual bool CastRangeSkill()
    {   
        BaseSkill[] rangeSkills = npc.status.Skills.Where(
             s =>
                 SkillType.RangeSkills.Contains(s.skillName)
                 && s.CooldownLeft == 0
                 && npc.status.GetConsumedAttrubute(s.costType).CurValue > s.AdjustCostValue
                 && s.effect.GetComponent<SkillEffect>()
                    .ShouldCast(npc, GetComponent<BaseNPCMovementAI>().TargetsInVision,s)                
                 ).ToArray();
        
        if (rangeSkills.Length > 0)
        {
            npc.Skill(npc.status.Skills.IndexOf(rangeSkills[Random.Range(0, rangeSkills.Length - 1)]) + 1);
            return true;
        }
        return false;
    }
    protected virtual bool CastHealing()
    {
        //取出是治療的 而且沒再CD的技能 而且還有資源可以使用
        
        BaseSkill[] healSkills = npc.status.Skills.Where(
            s => 
                SkillType.HealingSkills.Contains(s.skillName) 
                && s.CooldownLeft == 0 
                && npc.status.GetConsumedAttrubute(s.costType).CurValue> s.AdjustCostValue
                ).ToArray();        
        if (healSkills.Length > 0)
        {                     
            foreach (BaseSkill h in healSkills)
            {
                if (h.effect.GetComponent<SkillEffect>().ShouldCast(npc, GetComponent<BaseNPCMovementAI>().TargetsInVision, h))
                {
                    npc.Skill(npc.status.Skills.IndexOf(h) + 1);
                    return true;
                }
            }
            
        }
        return false;

    }
    protected virtual bool CastBuff() {
        //取出是治療的 而且沒再CD的技能 而且還有資源可以使用
        //而且是BuffSkill 而且身上沒有這個buff
        BaseSkill[] buffSkills = npc.status.Skills.Where(
             s =>
                 SkillType.BuffSkills.Contains(s.skillName)
                 && !SkillType.HealingSkills.Contains(s.skillName)
                 && s.CooldownLeft == 0
                 && npc.status.GetConsumedAttrubute(s.costType).CurValue > s.AdjustCostValue
                 && s is BuffSkill
                 && !npc.buffs.Contains(s as BuffSkill)
                 ).ToArray();
        if (buffSkills.Length > 0)
        {
            npc.Skill(npc.status.Skills.IndexOf(buffSkills[Random.Range(0, buffSkills.Length - 1)]) + 1);            
            return true;
        }
        return false;
    }
    public WeaponCollider weapon;
    protected virtual float SetAttackRange() {
        if (weapon == null)
            weapon = GetComponentInChildren<WeaponCollider>();
        Vector3 size;
        if (weapon.GetComponent<Collider>() != null)
        {
            size = weapon.GetComponent<Collider>().bounds.size;
         
        }
        else
        {
            size = weapon.GetComponentInParent<Collider>().bounds.size;
        }
        float[] s = new float[] { size.x,size.y,size.z};
        
        return s.Max()/2 + GetComponent<CharacterController>().radius;
    }
    protected virtual bool ShouldAttack(Transform target) {
        if (target.GetComponentInParent<Build>() != null) {
            return BuildCollisioned == target;
        }
        
        return Vector3.Distance(myTransform.position,target.position) < attackRange+ target.GetComponent<CharacterController>().radius;
    }
    protected virtual void FindTarget(bool finded) {
        
    }
    protected Transform BuildCollisioned;
    float buildingHitTimer = 0f;
    float buildingHitContinueTime = 0.1f;
    void OnControllerColliderHit(ControllerColliderHit other) {
        if (other.transform.GetComponentInParent<Build>() != null && other.transform == npc.attackTarget) {
            Debug.Log(other.transform.name +" Build Enter ");
            BuildCollisioned = other.transform;
            buildingHitTimer = Time.time;
        }
    }
}
