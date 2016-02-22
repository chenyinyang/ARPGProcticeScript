using UnityEngine;
using System.Collections.Generic;
public class BaseCharacterState  {
    public string Name;
    const int PRIMARY_ATTRIBUTE_INITIAL_VALUE = 50;
    const int SECONDARY_ATTRIBUTE_INITIAL_VALUE = 100;
    const int CONSUMED_ATTRIBUTE_INITIAL_VALUE = 2000;
    public List<BaseSkill> Skills;
    protected PrimaryAttribute[] priAttributes;
    protected SecondaryAttribute[] secAttributes;
    protected ConsumedAttribute[] conAttributes;
    protected StaticAttribute[] staticAttributes;
    const float EXP_TO_LEVEL1 = 3000;
    const float EXP_FIX_TO_NEXT_LEVEL = 1.1f;
    public int Level;
    public float Exp;
    public float ExpToLevel;
    // Use this for initialization
    public BaseCharacterState(string name) {
        this.Name = name;
        this.Skills = new List<BaseSkill>();
        CreatePrimaryAttribute();
        SetPrimaryAttribute();
        CreateSecondaryAttribute();
        SetSecondaryAttribute();
        CreateConsumedAttribute();
        SetConsumedAttribute();
        CreateStaticAttribute();        
        Exp = 0;
        ExpToLevel = EXP_TO_LEVEL1;
        Level = 1;
    }
    
    public bool GetExp(float mount) {
        Exp += mount;
        if (Exp > ExpToLevel) {
            Exp -= ExpToLevel;
            Level++;
            ExpToLevel *= EXP_FIX_TO_NEXT_LEVEL;
            return true;
        }
        return false;
    }


    public PrimaryAttribute GetPrimaryAttrubute(PrimaryAttributeName name) {
        return priAttributes[(int)name];
    }
    public SecondaryAttribute GetSecondaryAttrubute(SecondaryAttributeName name)
    {
        return secAttributes[(int)name];
    }
    public ConsumedAttribute GetConsumedAttrubute(ConsumedAttributeName name)
    {
        return conAttributes[(int)name];
    }
    public StaticAttribute GetStaticAttribute(StaticAttributeName name)
    {
        return staticAttributes[(int)name];
    }

    void CreatePrimaryAttribute() {
        priAttributes = new PrimaryAttribute[(int)PrimaryAttributeName.Count];
        for (int i = 0; i < priAttributes.Length; i++)
        {
            priAttributes[i] = new PrimaryAttribute((PrimaryAttributeName)i);
        }
    }
    protected virtual void SetPrimaryAttribute() {
        for (int i=0;i< priAttributes.Length;i++)
        {
            priAttributes[i].baseValue = PRIMARY_ATTRIBUTE_INITIAL_VALUE;
        }
    }

    void CreateSecondaryAttribute()
    {
        secAttributes = new SecondaryAttribute[(int)SecondaryAttributeName.Count];
        for (int i = 0; i < secAttributes.Length; i++)
        {
            secAttributes[i] = new SecondaryAttribute((SecondaryAttributeName)i, GetSecondaryAttributeModifier((SecondaryAttributeName)i));
        }
    }
    protected virtual void SetSecondaryAttribute()
    {
        for (int i = 0; i < secAttributes.Length; i++)
        {
            secAttributes[i].baseValue = SECONDARY_ATTRIBUTE_INITIAL_VALUE;
        }      
    }
    protected virtual AttributeModifier GetSecondaryAttributeModifier(SecondaryAttributeName attrName)
    {
        AttributeModifier modifier = new AttributeModifier();
        switch (attrName)
        {
            case SecondaryAttributeName.PhysicalDamage:
                modifier.AddRatio(GetPrimaryAttrubute(PrimaryAttributeName.Power), 2f);
                break;
            case SecondaryAttributeName.SkillDamage:
                modifier.AddRatio(GetPrimaryAttrubute(PrimaryAttributeName.Agility), 2f);
                break;
            case SecondaryAttributeName.MagicalDamage:
                modifier.AddRatio(GetPrimaryAttrubute(PrimaryAttributeName.Wisdom), 2f);
                break;
            case SecondaryAttributeName.PhysicalDeffence:
                modifier.AddRatio(GetPrimaryAttrubute(PrimaryAttributeName.Power), 1f);
                modifier.AddRatio(GetPrimaryAttrubute(PrimaryAttributeName.Constitution), 1f);
                break;
            case SecondaryAttributeName.SkillDeffence:
                modifier.AddRatio(GetPrimaryAttrubute(PrimaryAttributeName.Agility), 1f);
                modifier.AddRatio(GetPrimaryAttrubute(PrimaryAttributeName.Constitution), 1f);
                break;
            case SecondaryAttributeName.MagicalDeffence:
                modifier.AddRatio(GetPrimaryAttrubute(PrimaryAttributeName.Wisdom), 1f);
                modifier.AddRatio(GetPrimaryAttrubute(PrimaryAttributeName.Constitution), 1f);
                break;

            case SecondaryAttributeName.Critical:
                modifier.AddRatio(GetPrimaryAttrubute(PrimaryAttributeName.Agility), .66f);
                modifier.AddRatio(GetPrimaryAttrubute(PrimaryAttributeName.Power), .66f);
                modifier.AddRatio(GetPrimaryAttrubute(PrimaryAttributeName.Wisdom), .66f);
                break;
            case SecondaryAttributeName.CriticalDamage:
                modifier.AddRatio(GetPrimaryAttrubute(PrimaryAttributeName.Agility), 1f);
                modifier.AddRatio(GetPrimaryAttrubute(PrimaryAttributeName.Power), 1f);
                break;
            case SecondaryAttributeName.Dodge:
                modifier.AddRatio(GetPrimaryAttrubute(PrimaryAttributeName.Agility), 1f);
                modifier.AddRatio(GetPrimaryAttrubute(PrimaryAttributeName.Wisdom), 1f);
                break;
            case SecondaryAttributeName.Hit:
                modifier.AddRatio(GetPrimaryAttrubute(PrimaryAttributeName.Power), 1f);
                modifier.AddRatio(GetPrimaryAttrubute(PrimaryAttributeName.Wisdom), 1f);
                break;
            case SecondaryAttributeName.DoubleExpRate:
                modifier.AddRatio(GetPrimaryAttrubute(PrimaryAttributeName.Luck), 2f);
                break;
            case SecondaryAttributeName.HealthRecoverRate:
                modifier.AddRatio(GetPrimaryAttrubute(PrimaryAttributeName.Constitution), 1f);
                modifier.AddRatio(GetPrimaryAttrubute(PrimaryAttributeName.Spirit), 1f);
                break;
            case SecondaryAttributeName.ManaRecoverRate:
                modifier.AddRatio(GetPrimaryAttrubute(PrimaryAttributeName.Wisdom), 1f);
                modifier.AddRatio(GetPrimaryAttrubute(PrimaryAttributeName.Spirit), 1f);
                break;
            case SecondaryAttributeName.EnergyRecoverRate:
                modifier.AddRatio(GetPrimaryAttrubute(PrimaryAttributeName.Agility), 1f);
                modifier.AddRatio(GetPrimaryAttrubute(PrimaryAttributeName.Spirit), 1f);
                break;
            case SecondaryAttributeName.MoveSpeed:
                modifier.AddRatio(GetPrimaryAttrubute(PrimaryAttributeName.Agility), .25f);
                modifier.AddRatio(GetPrimaryAttrubute(PrimaryAttributeName.Constitution), .25f);
                break;
            case SecondaryAttributeName.Vision:
                modifier.AddRatio(GetPrimaryAttrubute(PrimaryAttributeName.Agility), 1f);
                break;

            default:
                break;
        }
        return modifier;
    }

    void CreateConsumedAttribute()
    {
        conAttributes = new ConsumedAttribute[(int)ConsumedAttributeName.Count];
        for (int i = 0; i < conAttributes.Length; i++)
        {
            conAttributes[i] = new ConsumedAttribute((ConsumedAttributeName)i, GetConsumedAttributeModifier((ConsumedAttributeName)i));
        }
    }
    protected virtual void SetConsumedAttribute()
    {
        for (int i = 0; i < conAttributes.Length; i++)
        {
            conAttributes[i].baseValue = CONSUMED_ATTRIBUTE_INITIAL_VALUE;
            conAttributes[i].CurValue = conAttributes[i].AdjustedValue;
        }
    }
    protected virtual AttributeModifier GetConsumedAttributeModifier(ConsumedAttributeName attrName)
    {
        AttributeModifier modifier = new AttributeModifier();
        switch (attrName)
        {
            case ConsumedAttributeName.Health:
                modifier.AddRatio(GetPrimaryAttrubute(PrimaryAttributeName.Constitution), 20f);
                break;
            case ConsumedAttributeName.Mana:
                modifier.AddRatio(GetPrimaryAttrubute(PrimaryAttributeName.Wisdom), 20f);
                break;
            case ConsumedAttributeName.Energy:
                modifier.AddRatio(GetPrimaryAttrubute(PrimaryAttributeName.Agility), 20f);
                break;
            default:
                break;
        }
        return modifier;
    }

    private void CreateStaticAttribute()
    {
        staticAttributes = new StaticAttribute[(int)StaticAttributeName.Count];
        for (int i = 0; i < staticAttributes.Length; i++)
        {
            staticAttributes[i] = new StaticAttribute((StaticAttributeName)i);
        }
    }
}
