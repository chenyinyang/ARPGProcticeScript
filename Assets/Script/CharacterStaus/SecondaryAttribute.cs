using UnityEngine;
using System.Collections;

public class SecondaryAttribute : ModifiedAttribute
{
   
    public SecondaryAttributeName name { get; private set; }

    public SecondaryAttribute(SecondaryAttributeName name, AttributeModifier attrModifier):base(name.ToString(),attrModifier)
    {
        this.name = name;
    }
    public SecondaryAttribute(SecondaryAttributeName name, int value,int buffedValue, int exp, int expToLevelUp,AttributeModifier attrModifier) : 
        base(name.ToString(),value, buffedValue, exp, expToLevelUp,attrModifier)
    {
        this.name = name;
    }
}
public enum SecondaryAttributeName {
    //Base Damages
    /// <summary>
    /// Power *2
    /// </summary>
    PhysicalDamage,
    /// <summary>
    /// Agility *2
    /// </summary>
    SkillDamage,
    /// <summary>
    /// Wisdom *2
    /// </summary>
    MagicalDamage,

    //Base Defences
    /// <summary>
    /// Power + Constitution
    /// </summary>
    PhysicalDeffence,
    /// <summary>
    /// Agility + Constitution
    /// </summary>
    SkillDeffence,
    /// <summary>
    /// Wisdom + Constitution 
    /// </summary>
    MagicalDeffence,

    //Critical rate
    /// <summary>
    /// Power *.66 + Agility *.66 + Wisdom *.66
    /// </summary>
    Critical,

    //Addition Damage at Critical
    /// <summary>
    /// Agility + Power
    /// </summary>
    CriticalDamage,

    //Miss
    /// <summary>
    /// Agility + Wisdom
    /// </summary>
    Dodge,

    //Against miss
    /// <summary>
    /// Power + Wisdom
    /// </summary>
    Hit,

    //other
    /// <summary>
    /// Luck*2
    /// </summary>
    DoubleExpRate,

    //Resource recover
    /// <summary>
    /// Constitute + Spirit
    /// </summary>
    HealthRecoverRate,
    /// <summary>
    /// Wisdom + Spirit
    /// </summary>
    ManaRecoverRate,
    /// <summary>
    /// Agility + Spirit
    /// </summary>
    EnergyRecoverRate,
    /// <summary>
    /// .25Con+.25Agi+100
    /// </summary>
    MoveSpeed,
    /// <summary>
    /// Agi+100 ->basevalue default
    /// </summary>
    Vision,
    Count
}


