using UnityEngine;
using System.Collections;

public class PriestCharacterState : BaseCharacterState {

    public PriestCharacterState(string name) : base(name) { }
    
    protected override void SetPrimaryAttribute()
    {
        GetPrimaryAttrubute(PrimaryAttributeName.Power).baseValue = Random.Range(10, 30);
        GetPrimaryAttrubute(PrimaryAttributeName.Agility).baseValue = Random.Range(10, 30);
        GetPrimaryAttrubute(PrimaryAttributeName.Wisdom).baseValue = Random.Range(30, 50);
        GetPrimaryAttrubute(PrimaryAttributeName.Constitution).baseValue = Random.Range(20, 40);
        GetPrimaryAttrubute(PrimaryAttributeName.Spirit).baseValue = Random.Range(20, 40);
        GetPrimaryAttrubute(PrimaryAttributeName.Luck).baseValue = Random.Range(0, 100);
    }
    protected override void SetSecondaryAttribute()
    {
        base.SetSecondaryAttribute();
        secAttributes[(int)SecondaryAttributeName.PhysicalDeffence].baseValue *= 1.2f;        
        secAttributes[(int)SecondaryAttributeName.MagicalDeffence].baseValue *= 1.5f;
        secAttributes[(int)SecondaryAttributeName.PhysicalDamage].baseValue *= .8f;
        secAttributes[(int)SecondaryAttributeName.MagicalDamage].baseValue *= 1.5f;
        secAttributes[(int)SecondaryAttributeName.Dodge].baseValue *= .5f;
        secAttributes[(int)SecondaryAttributeName.Critical].baseValue *= .75f;
        secAttributes[(int)SecondaryAttributeName.CriticalDamage].baseValue *= .75f;
    }
    protected override void SetConsumedAttribute()
    {
        base.SetConsumedAttribute();
        conAttributes[(int)ConsumedAttributeName.Health].baseValue *=1.2f;
        conAttributes[(int)ConsumedAttributeName.Health].CurValue = conAttributes[(int)ConsumedAttributeName.Health].AdjustedValue;
    }
}
