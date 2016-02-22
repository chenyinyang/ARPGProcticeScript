using UnityEngine;
using System.Collections;

public class StaticAttribute : Ability {
    public StaticAttributeName name { get; private set; }
    public StaticAttribute(StaticAttributeName name) : base(name.ToString(), 1, 0, 0, 50) {
        this.name = name;
    }
}

public enum StaticAttributeName {
    //傷害減免
    DamageTakeFix,
    //傷害增加
    DamageMake,
    //治療減免
    HealTakeFix,
    //治療增加
    HealMake,
    Count

}
