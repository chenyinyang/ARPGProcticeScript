using UnityEngine;
using System.Collections.Generic;
//using System;
[RequireComponent(typeof(CharacterController))]
public abstract class BaseCharacterBehavior:MonoBehaviour, IDamageMaker
{
    //Statistic
    public float _totolMakeDamage = 0;
    public float _totolTakeDamage = 0;
    public float _totolMakeHealing = 0;
    public float CurrentMakeDamage;
    public float CurrentTakeDamage;
    public float CurrentMakeHealing;
    private float DamageCounterInDelta = 0;
    private bool damageColloctEnable = false;
    private float HealingCounterInDelta = 0;
    private bool healingColloctEnable = false;
    const float UPDATE_STATISTIC_INTERVAL = .5f;
    public delegate void OnGetExpHandler(float mount);
    public event OnGetExpHandler onGetExp;
    protected virtual void UpdateStatistic() {
        float expMount = CurrentMakeDamage + CurrentMakeDamage+ CurrentMakeHealing;
        if (Random.Range(0, 101) < status.GetPrimaryAttrubute(PrimaryAttributeName.Luck).AdjustedValue)
            expMount *= 1.2f;
        if (onGetExp != null)
            onGetExp(expMount);
        if (status.GetExp(expMount))
            LevelUp();
        _totolMakeDamage += CurrentMakeDamage;
        _totolTakeDamage += CurrentMakeDamage;
        _totolMakeHealing += CurrentMakeHealing;
        CurrentMakeDamage = 0;
        CurrentMakeDamage = 0;
        CurrentMakeHealing = 0;

        if (damageColloctEnable)
        {
            if (DamageCounterInDelta > 0)
            {
                ht.SetText((int)DamageCounterInDelta * -1);
                DamageCounterInDelta = 0;
            }
            else
            {
                damageColloctEnable = false;
            }
        }
        if (healingColloctEnable)
        {
            if (HealingCounterInDelta > 0)
            {
                ht.SetText((int)HealingCounterInDelta);
                HealingCounterInDelta = 0;
            }
            else
            {
                healingColloctEnable = false;
            }
        }

    }
    float updateStatisticTimer = 0;

    //Events
    public delegate void CastOver(bool result);
    public delegate void AttackEnvHandler();
    public delegate void onDeadHandler(BaseCharacterBehavior npc);
    public delegate void onDamageHandler(float originalDamage, DamageType type, float damageCause, BaseCharacterBehavior attackTo, BaseCharacterBehavior attackFrom);
    public event AttackEnvHandler onAttackStart;
    public event AttackEnvHandler onAttackStop;    
    public event onDeadHandler OnDead;    
    public event onDamageHandler OnDamaged;

    //Character status
    public string CharacterName;
    internal BaseCharacterState status;
    protected abstract void LevelUp();
    public SkillName[] skills;
    internal bool IsDead;
    public bool isAttacking;
    internal bool IsAttacking
    {
        get { return isAttacking; }
        set
        {
            if (IsDead)
                return;
            if (isAttacking != value)
            {
                isAttacking = value;
                if (value && onAttackStart != null)
                {
                    CastingCancel();
                    onAttackStart();
                }
                if (!value && onAttackStop != null)
                    onAttackStop();
            }
            else
            {
                isAttacking = value;
            }
        }
    }
    public bool isSkilling;
    internal bool IsSkilling {
        get { return isSkilling; }
        set {
            if (!value) currentSkillingSkill = 0;
            isSkilling = value;
        } 
    }
    internal int currentSkillingSkill=0;
    internal bool IsCasting { get { return castCallback != null; } }
    internal WeaponCollider meleeWeapon;
    internal List<BuffSkill> buffs;
    const float BUFF_CHECK_INTERVAL = .1f;
    private float buffCheckTimer = 0f;
    protected virtual void CheckBuff() {
        List<BuffSkill> temp = new List<BuffSkill>();
        foreach (BuffSkill buff in buffs)
        {
            if (buff.Check(this))
                temp.Add(buff);
        }
        buffs = temp;
    }
    const float CONSUMED_ATTRIBUTE_RECOVER_INTERVAL = .1f;
    private float consumedAttrRecoverTimer = 0f;
    void RecoverConsumedAttribute() {
        //ResourceRecover        
        status.GetConsumedAttrubute(ConsumedAttributeName.Health).CurValue
            += status.GetSecondaryAttrubute(SecondaryAttributeName.HealthRecoverRate)
                    .AdjustedValue / 10* CONSUMED_ATTRIBUTE_RECOVER_INTERVAL;

        status.GetConsumedAttrubute(ConsumedAttributeName.Mana).CurValue
            += status.GetSecondaryAttrubute(SecondaryAttributeName.ManaRecoverRate)
                    .AdjustedValue / 10 * CONSUMED_ATTRIBUTE_RECOVER_INTERVAL;

        status.GetConsumedAttrubute(ConsumedAttributeName.Energy).CurValue
            += status.GetSecondaryAttrubute(SecondaryAttributeName.EnergyRecoverRate)
                    .AdjustedValue / 10 * CONSUMED_ATTRIBUTE_RECOVER_INTERVAL;        
    }
    protected virtual void Dead()
    {
        tag = Tags.DeadBody;
        CastingCancel();
        IsAttacking = false;
        CancelInvoke("UpdateStatistic");
        CancelInvoke("CheckBuff" );
        CancelInvoke("RecoverConsumedAttribute");
        CancelInvoke("EargleEyeImgTransformRotation");
        IsDead = true;
        if (OnDead != null)
        {
            OnDead(this);
        }

    }
    public virtual void AddBuff(BuffSkill buff)
    {
        this.buffs.Add(buff);
    }
    public virtual void RemoveBuff(SkillName buff)
    {
        BuffSkill bsk = buffs.Find(b => b.skillName == buff);
        if (bsk != null)
            bsk.RemoveBuff(this);
    }

    //Combat
    public void Heal(float mount, BaseCharacterBehavior source)
    {
        if (IsDead)
            return;
        if (mount == 0)
            return;
        ConsumedAttribute health = status.GetConsumedAttrubute(ConsumedAttributeName.Health);
        if (health.LossValue == 0)
            return;
        mount *= status.GetStaticAttribute(StaticAttributeName.HealTakeFix).AdjustedValue;
        source.CurrentMakeHealing += mount;
        if (health.LossValue < mount)
            mount = health.LossValue;
        health.CurValue += mount;
        if (healingColloctEnable)
            HealingCounterInDelta += mount;
        else
        {
            healingColloctEnable = true;
            ht.SetText((int)mount);
        }
        if (health.CurValue <= 0)
        {
            Dead();
        }
        else
        {
            //保留event("GetHeal");
        }
    }
    public void ManaRecover(float mount)
    {
        if (IsDead)
            return;
        if (mount == 0)
            return;
        status.GetConsumedAttrubute(ConsumedAttributeName.Mana).CurValue += mount;
        ht.SetText((int)mount, Color.blue);
    }
    public void EnergyRecover(float mount)
    {
        if (IsDead)
            return;
        if (mount == 0)
            return;
        status.GetConsumedAttrubute(ConsumedAttributeName.Energy).CurValue += mount;
        ht.SetText((int)mount, Color.yellow);
    }
    public void Damage(DamageType type, IDamageMaker damageMaker, BaseCharacterBehavior source)
    {
        Damage(type,damageMaker, source , true);
    }
    public void Damage(DamageType type, IDamageMaker damageMaker, BaseCharacterBehavior source, bool triggerEvent)
    {
        if (IsDead)
            return;
        bool isCritical;
        float additionHit;
        float mount = damageMaker.GetCurDamage(type, out isCritical, out additionHit);
        if (mount == 0)
            return;
        float damage = 0;
        if (type == DamageType.Nature)
        {
            damage = mount;
        }
        else
        {
            //Calculate hit
            float dodge = status.GetSecondaryAttrubute(SecondaryAttributeName.Dodge).AdjustedValue;
            int dodgeRnd = Random.Range(0, 1000);
            if (type != DamageType.Magic && dodgeRnd < dodge)
            {
                //Miss
                if (dodge > additionHit)
                {
                    ht.SetText("Miss!!");
                    return;
                }
            }
            float deffence = 0;
            switch (type)
            {
                case DamageType.Physical:
                    deffence = status.GetSecondaryAttrubute(SecondaryAttributeName.PhysicalDeffence)
                        .AdjustedValue;
                    break;
                case DamageType.Skill:
                    deffence = status.GetSecondaryAttrubute(SecondaryAttributeName.SkillDeffence)
                        .AdjustedValue;
                    break;
                case DamageType.Magic:
                    deffence = status.GetSecondaryAttrubute(SecondaryAttributeName.MagicalDeffence)
                        .AdjustedValue;
                    break;
            }
            //def 300 => 1-300/600 => .5f, def 1000=>1- 1000/1300=> .23f
            damage = mount * (1 - deffence / (deffence + 300));
            damage *= status.GetStaticAttribute(StaticAttributeName.DamageTakeFix).AdjustedValue;
        }
        CurrentTakeDamage += damage;
        source.CurrentMakeDamage += damage;
        ConsumedAttribute health = status.GetConsumedAttrubute(ConsumedAttributeName.Health);
        health.CurValue -= damage >= 1 ? damage : 1;
        if (isCritical)
        {
            ht.SetText((int)damage * -1, true);
        }
        else
        {
            if (damageColloctEnable)
                DamageCounterInDelta += damage;
            else
            {
                damageColloctEnable = true;
                ht.SetText((int)damage * -1);
            }
        }

        if (health.CurValue <= 0)
        {
            Dead();
        }
        else
        {
            if (triggerEvent && OnDamaged != null)
            {
                OnDamaged(mount, type, damage, this, source);
            }
        }
    }
    public float GetCurDamage(DamageType type, out bool isCritical, out float additionHit)
    {       
        float damage = 0;
        switch (type)
        {
            case DamageType.Physical:
                damage = status.GetSecondaryAttrubute(SecondaryAttributeName.PhysicalDamage).AdjustedValue;
                break;
            case DamageType.Skill:
                damage = status.GetSecondaryAttrubute(SecondaryAttributeName.SkillDamage).AdjustedValue * (IsSkilling ? curSkillBaseDmg : 1);
                break;
            case DamageType.Magic:
                damage = status.GetSecondaryAttrubute(SecondaryAttributeName.MagicalDamage).AdjustedValue;
                break;
            case DamageType.Nature:
                damage = (status.GetSecondaryAttrubute(SecondaryAttributeName.MagicalDamage).AdjustedValue
                    + status.GetSecondaryAttrubute(SecondaryAttributeName.PhysicalDamage).AdjustedValue
                    + status.GetSecondaryAttrubute(SecondaryAttributeName.SkillDamage).AdjustedValue) / 3;
                break;
        }
        damage *= status.GetStaticAttribute(StaticAttributeName.DamageMake).AdjustedValue;
        if (type == DamageType.Nature)
        {
            isCritical = false;
        }
        else
        {
            //Get Critical
            float critical = status.GetSecondaryAttrubute(SecondaryAttributeName.Critical).AdjustedValue;
            // ex: pow 100, agi 100, wis 100 => cri 200 == 20%
            if (Random.Range(0, 1000) < critical)
            {
                isCritical = true;
                //criDmg 300 => + base*0.3
                damage *= (int)(2 +
                            (float)status.GetSecondaryAttrubute(SecondaryAttributeName.CriticalDamage)
                                .AdjustedValue / 1000);
            }
            else
                isCritical = false;
        }
        additionHit = status.GetSecondaryAttrubute(SecondaryAttributeName.Hit).AdjustedValue;
        return damage;
    }
    public virtual bool CanDamageTarget(Damagable target)
    {
        return true;
    }
    public void Skill(int number)
    {
        if (IsDead)
            return;
        if (number > 0 && number <= status.Skills.Count)
        {
            bool success = false;
            if (buffs.Contains(status.Skills[number - 1] as BuffSkill))
            {
                success = (status.Skills[number - 1] as BuffSkill).Rebuff(this);
            }
            else
            {
                success = status.Skills[number - 1].Cast(this);
                if (success)
                {
                    curSkillBaseDmg = status.Skills[number - 1].AdjustedDamage;
                }
            }
            if (success)
            {
                currentSkillingSkill = number;
                buffHt.SetText(status.Skills[number - 1].skillName.ToString());
                status.Skills.ForEach(s => s.SetGCD());
            }
        }
    }
    public virtual void Casting(SkillName name, float time, CastOver callBack)
    {        
        CastingCancel();
        castCallback = callBack;
        Invoke("CastWait", time);
        
        //castingTime = time;
        //castingTimer = Time.time;
    }
    public virtual void CastingCancel()
    {        
        if (castCallback != null)
        {            
            castCallback(false);
            castCallback = null;
            CancelInvoke("CastWait");
        }
    }

    void CastWait()
    {
        if (castCallback != null)
        {
            castCallback(true);
            castCallback = null;
        }
    }

    //gameobject    
    public float BodyWidth;
    public float BodyHeight;
    public Transform model;
    protected CharacterController characterController;
    internal Transform myTransform;
    protected Transform eargleEyeImgTransform;
    protected float curSkillBaseDmg;
    protected int curAttackBaseDmg;
    void EargleEyeImgTransformRotation()
    {
        eargleEyeImgTransform.rotation = Quaternion.AngleAxis(180, Vector3.up);
    }


    protected float castingTimer;
    protected float castingTime;
    protected CastOver castCallback; 

    
    
    public HurtText ht {  get {
            return hts[Random.Range(0, hts.Count)];
        }
    }
    private List<HurtText> hts;
    public HurtText buffHt;
    public bool ManualCharacterController;
    //Monobehaviour life cycle
    protected virtual void Awake() {
        myTransform = transform;
        //設定角色控制器大小        
        characterController = GetComponent<CharacterController>();
        SkinnedMeshRenderer skin = model.GetComponentInChildren<SkinnedMeshRenderer>();
        if (!ManualCharacterController)
        {
            characterController.stepOffset = 0.04f;
            characterController.slopeLimit = 45;
            characterController.height = skin.bounds.size.y;
            characterController.radius = (skin.bounds.size.x + skin.bounds.size.z) / 4;
            characterController.center = Vector3.up * skin.bounds.size.y / 2;
            characterController.skinWidth = 0.0001f;
        }
        BodyWidth = characterController.bounds.size.x;
        BodyHeight = characterController.bounds.size.y;
       
        //設定浮動傷害顯視點
        buffs = new List<BuffSkill>();
        hts = new List<HurtText>();        
        for (int i = 0; i < 2; i++)
        {
            Transform hudTextPoint = new GameObject("HUDTextPoint").transform;
            hudTextPoint.parent = myTransform;
            hudTextPoint.position = myTransform.position
                + new Vector3(Random.Range(BodyWidth * -1, BodyWidth),
                              Random.Range(BodyHeight / 2,
                              BodyHeight), 0);
            HurtText hurtText = (GameObject.Instantiate(
                                        ResourceLoader.UIPrefabs.GetHurtText(),
                                        hudTextPoint.position, Quaternion.identity) as GameObject)
                                    .GetComponent<HurtText>();
            hurtText.gameObject.name = CharacterName + "_hurtText";
            hurtText.InitailHUDText(hudTextPoint);
            hts.Add(hurtText);
        }

        //設定浮動文字顯視點
        Transform buffHUDTextPoint = new GameObject("BuffHUDTextPoint").transform;
        buffHUDTextPoint.parent = myTransform;
        buffHUDTextPoint.position = myTransform.position 
            + new Vector3(0, BodyHeight, 0);
        buffHt = (GameObject.Instantiate(
                            ResourceLoader.UIPrefabs.GetHurtText(), 
                            buffHUDTextPoint.position, Quaternion.identity) as GameObject)
                        .GetComponent<HurtText>();
        buffHt.InitailHUDText(buffHUDTextPoint);
        gameObject.AddComponent<Damagable>().damageHandler += Damage;

        //CreatePlaneForEargleEye();
        Mesh mesh = ResourceLoader.Utilities.GetCirclePlane();
        GameObject eargleEyeImg = new GameObject();
        eargleEyeImgTransform = eargleEyeImg.transform;
        eargleEyeImg.name = "EargleEyeImg";
        eargleEyeImg.AddComponent<MeshFilter>().mesh = mesh;
        MeshRenderer eargleEyeImgMeshRenderer = eargleEyeImg.AddComponent<MeshRenderer>();
        eargleEyeImgMeshRenderer.materials = new Material[] {
            new Material(Shader.Find("Sprites/Default"))
        };
        eargleEyeImgMeshRenderer.materials[0].color =
            tag == Tags.Player ? Color.cyan :
            tag == Tags.Friend ? Color.green :
            tag == Tags.Enemy ? Color.yellow :
            Color.black;
        eargleEyeImgMeshRenderer.reflectionProbeUsage = UnityEngine.Rendering.ReflectionProbeUsage.Off;
        eargleEyeImgMeshRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        eargleEyeImg.layer = 12;
        eargleEyeImgTransform.parent = myTransform;
        eargleEyeImgTransform.localPosition = Vector3.up * .5f;
        eargleEyeImgTransform.localScale = Vector3.one * .4f;
    }
    protected virtual void Start()
    {
        for (int i = 0; i < skills.Length; i++)
        {
            BaseSkill sk = SkillGenerator.Instance().GetSkill(skills[i]);
            sk.SetCaster(this);
            status.Skills.Add(sk);
        }
        InvokeRepeating("UpdateStatistic", 0, UPDATE_STATISTIC_INTERVAL);
        InvokeRepeating("CheckBuff", 0, BUFF_CHECK_INTERVAL);
        InvokeRepeating("RecoverConsumedAttribute", 0, CONSUMED_ATTRIBUTE_RECOVER_INTERVAL);
        InvokeRepeating("EargleEyeImgTransformRotation", 0, 0.05f);
    }



}
