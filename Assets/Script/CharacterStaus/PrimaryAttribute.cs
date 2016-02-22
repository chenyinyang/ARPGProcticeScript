using UnityEngine;
using System.Collections;

public class PrimaryAttribute : Ability
{
    
    public PrimaryAttributeName name { get; private set; }
    public PrimaryAttribute(PrimaryAttributeName name):base(name.ToString())
    {
        this.name = name;
    }
    public PrimaryAttribute(PrimaryAttributeName name,int value,int buffedValue, int exp, int expToLevelUp) :
        base(name.ToString(),value,buffedValue,exp,expToLevelUp)
    {
        this.name = name;
    }
}
public enum PrimaryAttributeName
{
    /// <summary>
    /// +2 Physical Dmg, 
    /// +1 Physical Def,
    /// +.66 Critical Rate,
    /// +1 Critical Dmg,
    /// +1 Hit,
    /// </summary>
    Power,
    /// <summary>
    /// +2 Skill Dmg,
    /// +1 Skill Def,
    /// +.66 Critical Rate,
    /// +1 Critical Dmg,
    /// +1 Dodge,
    /// +1 Energy Recover Rate,
    /// </summary>
    Agility,
    /// <summary>
    /// +2 Magical Dmg,
    /// +1 Magical Def,
    /// +.66 Critical Rate,
    /// +1 Dodge,
    /// +1 Hit,
    /// +1 Mana Recover Rate,
    /// </summary>
    Wisdom,
    /// <summary>
    /// +1 Physical Def,
    /// +1 Skill Def,
    /// +1 Magical Def,
    /// +1 Health Recover Rate,
    /// </summary>
    Constitution,
    /// <summary>
    /// +1 HealthRecoverRate,
    /// +1 ManaRecoverRate,
    /// +1 Energy Recover Rate,
    /// </summary>
    Spirit,
    /// <summary>
    /// +2 Double Exp Rate
    /// </summary>
    Luck,
    Count
}