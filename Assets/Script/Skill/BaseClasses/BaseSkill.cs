using UnityEngine;
using System.Collections;

public class BaseSkill
{
    const float GCD=1.5f;
    public SkillName skillName;
    protected float baseDamage;
    public float AdjustedDamage
    {
        get
        {
            return baseDamage + buffedDamage;
        }
    }
    protected float effectDamage;
    public float AdjustedEffectDamage
    {
        get
        {
            return effectDamage + buffedDamage;
        }
    }
    private float buffedDamage;
    private int baseCostValue;
    private int buffedCostValue;

    internal float SkillCoolDown;
    internal float CastTime;
    internal ConsumedAttributeName costType;
    protected GameObject effectInstance;
    public int AdjustCostValue { get { return baseCostValue + buffedCostValue; } }
    internal GameObject effect;
    internal SkillEffect skillEffect;
    protected BaseCharacterBehavior caster;
    public void SetCaster(BaseCharacterBehavior caster) {
        this.caster = caster;
        effect.GetComponent<SkillEffect>().SetCaster(caster);
    }
    public Texture2D icon
    {
        get
        {            
            return ResourceLoader.Skill.GetIcon(skillName);
        }
    }
    protected float skillTimer;
    //private GameObject _effect;

    public BaseSkill(SkillName skillName) :this(skillName,0,0,0,0,0, ConsumedAttributeName.Energy,2f){ }
    public BaseSkill(SkillName skillName, float baseDamage,float effectDamage, int buffedDamage,
        int baseCostValue, int buffedCostValue, ConsumedAttributeName costType, float skillCoolDown, float castTime = 0)
    {
        this.skillName = skillName;
        this.baseDamage = baseDamage;
        this.effectDamage = effectDamage;
        this.buffedDamage = buffedDamage;
        this.baseCostValue = baseCostValue;
        this.buffedCostValue = buffedCostValue;
        this.costType = costType;
        this.SkillCoolDown = skillCoolDown;
        this.CastTime = castTime;
        this.effect = ResourceLoader.Skill.GetSkillPrefab(skillName);
        CooldownLeft = 0;
        skillEffect = effect.GetComponent<SkillEffect>();

    }
    public float CooldownLeft {
        get {
            if(Time.time - skillTimer < SkillCoolDown)
                return SkillCoolDown - (Time.time - skillTimer);
            return 0;
        }
        set {
            skillTimer = Time.time - (SkillCoolDown - value);
        }
    }
    public void SetGCD() {
        if (CooldownLeft < GCD)
            CooldownLeft = GCD;
    }
    private BaseCharacterBehavior castTo;
    private GameObject castingEffect;
    public virtual bool Cast(BaseCharacterBehavior target, BaseCharacterBehavior castTo = null)
    {

        //cd中,失敗
        if (!CheckCD())
            return false;
        //有資源可以放
        if (!CheckReourceEnough())
            return false;
        this.castTo = castTo;
        //中斷目前施法
        caster.CastingCancel();
        CreateEffect(target);
        if (CastTime != 0)
        {   
            effectInstance.GetComponent<SkillEffect>().Prepare(caster.model,CastTime);
            castingEffect = GameObject.Instantiate(ResourceLoader.Skill.GetSkillCastingPrefab(), caster.transform.position, Quaternion.identity) as GameObject;
            castingEffect.transform.parent = caster.transform;
            caster.Casting(skillName, CastTime,CastCallBack);
        }
        else
        {
            //Play技能
            skillTimer = Time.time;
            effectInstance.GetComponent<SkillEffect>().Play(target.model,this, castTo);
        }
        return true;
        
    }
    protected void CreateEffect(BaseCharacterBehavior target) {
        //取得技能對象的作用點
        Transform skillOnObj = GetSkillOnObj(target);

        //在技能作用點產生技能特效物件
        effectInstance = GameObject.Instantiate(effect, skillOnObj.position, skillOnObj.rotation) as GameObject;
        effectInstance.transform.parent = skillOnObj;
        SkillEffect sk = effectInstance.GetComponent<SkillEffect>();
        //設定特效施放者
        sk.SetCaster(caster);        
    }
    protected void CastCallBack(bool result)
    {
        GameObject.Destroy(castingEffect);
        if (result)
        {
            effectInstance.GetComponent<SkillEffect>().Play(caster.model,this, castTo);
            skillTimer = Time.time;
        }
        else
            GameObject.Destroy(effectInstance);
    }
    protected bool CheckCD() {
        return !(Time.time - skillTimer < SkillCoolDown);
    }
    protected bool CheckReourceEnough() {
        
        if (caster.status.GetConsumedAttrubute(costType).CurValue >= AdjustCostValue)
        {
            //消耗
            caster.status.GetConsumedAttrubute(costType).CurValue -= AdjustCostValue;
            return true;
        }
        return false;
    }
    protected Transform GetSkillOnObj(BaseCharacterBehavior target) {
        switch (effect.GetComponent<SkillEffect>().effectPosition)
        {            
            case SkillEffect.EffectPosition.BodyCenter:
                //沒有身體中心就創造出來
                Transform skillOnObj = target.model.Find("BodyCenter");
                if (skillOnObj == null)
                {
                    GameObject bodyCenter = new GameObject("BodyCenter");
                    bodyCenter.transform.position = target.model.transform.position + target.GetComponent<CharacterController>().center;
                    bodyCenter.transform.rotation = target.model.transform.rotation;
                    bodyCenter.transform.parent = target.model.transform;
                    skillOnObj = bodyCenter.transform;
                }
                return skillOnObj;
            case SkillEffect.EffectPosition.Weapon:
                if (target.model.GetComponentInChildren<WeaponCollider>() != null)
                    return target.model.GetComponentInChildren<WeaponCollider>().transform;
                if (target.model.GetComponentInChildren<WeaponEmission>() != null)
                    return target.model.GetComponentInChildren<WeaponEmission>().transform;
                break;
            case SkillEffect.EffectPosition.BodyGround:                
            default:
                break;
        }
        return target.transform;
    }
}
public enum DamageType {
    Physical,
    Skill,
    Magic,
    Nature
}