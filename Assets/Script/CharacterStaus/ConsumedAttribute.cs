using UnityEngine;
using System.Collections;

public class ConsumedAttribute : ModifiedAttribute
{
    public delegate void OnValueUpdate(float curValue, float maxValue);
    public event OnValueUpdate onValueUpdate;
    private float _curValue;
    public ConsumedAttributeName name { get; private set; }
    public float LossValue { get { return AdjustedValue - CurValue; } }
    public float CurValue
    {
        get
        {
            return _curValue;
        }
        set
        {

            if (value >= AdjustedValue)
                _curValue = AdjustedValue;
            else if (value < 0)
                _curValue = 0;
            else
                _curValue = value;
            
            if (onValueUpdate != null)
                onValueUpdate(_curValue, AdjustedValue);
        }
    }
    public ConsumedAttribute(ConsumedAttributeName name, AttributeModifier attrModifier):base(name.ToString(),attrModifier)
    {
        this.name = name;
        this.CurValue = AdjustedValue;
        
    }
    public ConsumedAttribute(ConsumedAttributeName name, int value, int buffedValue, int exp, int expToLevelUp, AttributeModifier attrModifier,int curValue) : 
        base(name.ToString(),value, buffedValue, exp, expToLevelUp,attrModifier)
    {
        this.name = name;
        this.CurValue = curValue;
    }
}
public enum ConsumedAttributeName {
    Health,
    Mana,
    Energy,
    Count
}
