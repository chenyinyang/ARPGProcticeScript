using UnityEngine;
using System.Collections;

public class ModifiedAttribute : Ability
{
    private AttributeModifier attrModifier;  

    public ModifiedAttribute(string name, AttributeModifier attrModifier):base(name.ToString())
    {
        this.attrModifier = attrModifier;
    }
    public ModifiedAttribute(string name, int value, int buffedValue, int exp, int expToLevelUp, AttributeModifier attrModifier) : 
        base(name.ToString(),value, buffedValue, exp, expToLevelUp)
    {
        this.attrModifier = attrModifier;
    }
    public override float AdjustedValue
    {
        get
        {
            return base.AdjustedValue + attrModifier.Value;
        }
    }
}