using UnityEngine;
using System.Collections.Generic;
using System;

public class Buff
{
    public enum BuffType
    {
        Absolute,
        Relative
    }
    protected BuffType type;
    protected PrimaryAttributeName attr;
    protected SecondaryAttributeName attr2;
    protected ConsumedAttributeName attr3;
    protected StaticAttributeName attr4;
    protected int attributeType;
    public float BaseValue { get { return valueOverLayer[0]; } }
    protected List<float> valueOverLayer;
    protected int stack;
    protected BaseCharacterBehavior caster;
    public delegate void BuffEventHandler(BaseCharacterBehavior target);
    public BuffEventHandler onBuff;
    public BuffEventHandler endBuff;
    public Buff(PrimaryAttributeName attr, BuffType type, float value)
    {
        this.attr = attr;
        this.type = type;
        attributeType = 1;
        valueOverLayer = new List<float>() { value};
    }
    public Buff(SecondaryAttributeName attr, BuffType type, float value)
    {
        this.attr2 = attr;
        this.type = type;
        attributeType = 2;
        valueOverLayer = new List<float>() { value };
    }

    public Buff(ConsumedAttributeName attr, BuffType type, float value)
    {
        this.attr3 = attr;
        this.type = type;
        attributeType = 3;
        valueOverLayer = new List<float>() { value };
    }

    public Buff(StaticAttributeName attr, BuffType type, float value)
    {
        this.attr4 = attr;
        this.type = type;
        attributeType = 4;
        valueOverLayer = new List<float>() { value };
    }
    public virtual void OnBuff(BaseCharacterBehavior character, BaseCharacterBehavior caster,bool stackable = false, float updateValue = 0)
    {
        if (stackable)
        {
            stack++;
            valueOverLayer.Add(updateValue);
        }
        else
        {
            stack = 1;
            if (updateValue != 0)
                valueOverLayer[0] = updateValue;
        }
        this.caster = caster;
        Ability attr = null;
        int mount;
        switch (attributeType)
        {
            case 1:
                attr = character.status.GetPrimaryAttrubute(this.attr);
                break;
            case 2:
                attr = character.status.GetSecondaryAttrubute(this.attr2);
                break;
            case 3:
                attr = character.status.GetConsumedAttrubute(this.attr3);
                break;
            case 4:
                attr = character.status.GetStaticAttribute(this.attr4);                
                break;
            default:
                break;
        }
        mount = (int)(type == BuffType.Absolute ? valueOverLayer[stack - 1] : attr.baseValue * valueOverLayer[stack - 1]);
        attr.buffedValue += mount;
        if (onBuff != null) {
            onBuff(character);
        }
        
    }
    public virtual void OnDeBuff(BaseCharacterBehavior character)
    {
        int mount;
        Ability attr = null;
        switch (attributeType)
        {
            case 1:
                attr = character.status.GetPrimaryAttrubute(this.attr);
               
                break;
            case 2:
                attr = character.status.GetSecondaryAttrubute(this.attr2);
              
                break;
            case 3:
                attr = character.status.GetConsumedAttrubute(this.attr3);
               
                break;
            case 4:
                attr = character.status.GetStaticAttribute(this.attr4);                
                break;
            default:
                break;
        }
        
        foreach (float value in valueOverLayer)
        {
            mount = (int)(type == BuffType.Absolute ? value : attr.baseValue * value) * -1;
            attr.buffedValue += mount;
        }        
        if (endBuff != null)
        {
            endBuff(character);
        }
    }

    public virtual void OnReBuff(BaseCharacterBehavior target, BaseCharacterBehavior caster, bool stackable,float updateValue=0)
    {
        if (!stackable)
            OnDeBuff(target);            
        OnBuff(target, caster, stackable, updateValue);
    }    
}
