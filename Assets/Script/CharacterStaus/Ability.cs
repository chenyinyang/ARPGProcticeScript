using UnityEngine;
using System.Collections;

public class Ability
{
    string name;
    public float baseValue;
    public float buffedValue;
    public float exp;
    public float expToLevelUp;
    public Ability(string name):this(name,0,0,0,100) {
      
    }
    public Ability(string name, float value, float buffedValue, float exp, float expToLevelUp)
    {
        this.name = name;
        this.baseValue = value;
        this.buffedValue = buffedValue;
        this.exp = exp;
        this.expToLevelUp = expToLevelUp;
    }
    public virtual float AdjustedValue { get { return baseValue + buffedValue; } }
}

