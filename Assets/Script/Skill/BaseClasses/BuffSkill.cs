using UnityEngine;
using System.Linq;
using System.Collections.Generic;
using System;

public class BuffSkill : BaseSkill,IEqualityComparer<BuffSkill> {
    public float buffTime { get; private set; }
    public Buff[] buffs { get; private set; }
    public Texture2D BuffIcon { get; private set; }
    public bool Stackable { get; private set; }
    public int stackLayer { get; private set; }
    private BaseCharacterState status;
    private Transform model;
    public BuffSkill(SkillName skillName) : base(skillName) { }
    public BuffSkill(SkillName skillName, float baseDamage, float effectDamage, int buffedDamage,
        int baseCostValue, int buffedCostValue, ConsumedAttributeName costType, float skillCoolDown,float buffTime,Buff[] buffs,bool stackable = false) 
        : base(skillName, baseDamage, effectDamage, buffedDamage, baseCostValue, buffedCostValue, costType, skillCoolDown)
    {
        this.buffTime = buffTime;
        this.buffs = buffs;
        this.BuffIcon = ResourceLoader.Skill.GetIcon(skillName);
        this.Stackable = stackable;
        this.stackLayer = 0;
    }
    private object castLocker = new object();
    float buffTimer;
    public float TimeLeft
    {
        get
        {
            float timeLeft = buffTime - (Time.time - buffTimer);
            return timeLeft > 0 ? timeLeft : 0;
        }

    }

    public override bool Cast(BaseCharacterBehavior target, BaseCharacterBehavior castTo = null)
    {
        lock (castLocker)
        {
            if (target.buffs.Contains(this, this))
            {              
                return target.buffs.Find(b => b.skillName == this.skillName).Rebuff(target,this.buffs);
            }
            bool result = base.Cast(target);
            this.status = target.status;
            this.model = target.model.transform;
            this.stackLayer++;
            buffTimer = Time.time;
            if (result)
            {
                for (int i = 0; i < buffs.Length; i++)
                {
                    buffs[i].OnBuff(target,caster,Stackable);
                }
            }
            target.AddBuff(this);
            return result;
        }        
    }
    public virtual bool Check(BaseCharacterBehavior character) {
        lock (castLocker)
        {
            for (int i = 0; i < buffs.Length; i++)
            {
                if (buffs[i] is ConsumedStatusBuff)
                    (buffs[i] as ConsumedStatusBuff).OnUpdate(character);
            }
            if (TimeLeft <= 0 || character.IsDead)
            {
                for (int i = 0; i < buffs.Length; i++)
                {
                    buffs[i].OnDeBuff(character);
                }
                GameObject.Destroy(effectInstance,1f);
                return false;
            }
            return true;
        }
    }

    public bool Rebuff(BaseCharacterBehavior target,Buff[] buffUpdate = null)
    {        
        lock(castLocker)
        {
            if (Time.time - skillTimer < SkillCoolDown)
                return false;
            if (target.status.GetConsumedAttrubute(costType).CurValue >= AdjustCostValue)
            {
                buffTimer = Time.time;
                skillTimer = Time.time;
                if (Stackable)
                {
                    this.stackLayer++;
                }
                for (int i = 0; i < buffs.Length; i++)
                {                    
                    buffs[i].OnReBuff(target,caster,Stackable, buffUpdate==null?0:buffUpdate[i].BaseValue);
                }
                target.status.GetConsumedAttrubute(costType).CurValue -= AdjustCostValue;
                if (effectInstance != null)
                    GameObject.Destroy(effectInstance);
                CreateEffect(target);
                effectInstance.GetComponent<DelayDestroiedSkillEffect>().Play(model,this);
                if (target is NPCController)
                    (target as NPCController).addNewBuffFlag = true;
                return true;
            }
            return false;
        }
        
    }
    public virtual bool RemoveBuff(BaseCharacterBehavior character)
    {
        lock (castLocker)
        {
           
            for (int i = 0; i < buffs.Length; i++)
            {
                buffs[i].OnDeBuff(character);
            }
            character.buffs.Remove(this);
            GameObject.Destroy(effectInstance);
            return true;
        }
    }

    public bool Equals(BuffSkill x, BuffSkill y)
    {
        return x.skillName == y.skillName;
    }

    public int GetHashCode(BuffSkill obj)
    {
        return obj.GetHashCode();
    }
}


