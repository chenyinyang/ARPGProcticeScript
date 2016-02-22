using UnityEngine;
using System.Collections;

public class MagicianCharacterState : BaseCharacterState {

    public MagicianCharacterState(string name) : base(name) { }
    
    protected override void SetPrimaryAttribute()
    {
        GetPrimaryAttrubute(PrimaryAttributeName.Power).baseValue = Random.Range(5, 25);
        GetPrimaryAttrubute(PrimaryAttributeName.Agility).baseValue = Random.Range(10, 30);
        GetPrimaryAttrubute(PrimaryAttributeName.Wisdom).baseValue = Random.Range(40, 60);
        GetPrimaryAttrubute(PrimaryAttributeName.Constitution).baseValue = Random.Range(5, 25);
        GetPrimaryAttrubute(PrimaryAttributeName.Spirit).baseValue = Random.Range(30, 50);
        GetPrimaryAttrubute(PrimaryAttributeName.Luck).baseValue = Random.Range(0, 100);
    }
    protected override void SetSecondaryAttribute()
    {
        base.SetSecondaryAttribute();
        secAttributes[(int)SecondaryAttributeName.PhysicalDeffence].baseValue *= .5f;
        secAttributes[(int)SecondaryAttributeName.SkillDeffence].baseValue *= .5f;
        secAttributes[(int)SecondaryAttributeName.MagicalDeffence].baseValue *= 1.4f;
        secAttributes[(int)SecondaryAttributeName.PhysicalDamage].baseValue *= .5f;
        secAttributes[(int)SecondaryAttributeName.SkillDamage].baseValue *= .5f;
        secAttributes[(int)SecondaryAttributeName.MagicalDamage].baseValue *= 1.5f;
        secAttributes[(int)SecondaryAttributeName.Dodge].baseValue *= .5f;
    }
    protected override void SetConsumedAttribute()
    {
        base.SetConsumedAttribute();
        conAttributes[(int)ConsumedAttributeName.Health].baseValue *= .8f;
        conAttributes[(int)ConsumedAttributeName.Health].CurValue = conAttributes[(int)ConsumedAttributeName.Health].AdjustedValue;
        conAttributes[(int)ConsumedAttributeName.Mana].baseValue *=1.2f;
        conAttributes[(int)ConsumedAttributeName.Mana].CurValue = conAttributes[(int)ConsumedAttributeName.Mana].AdjustedValue;
    }
}
