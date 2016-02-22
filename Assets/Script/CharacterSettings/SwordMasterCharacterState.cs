using UnityEngine;
using System.Collections;

public class SwordMasterCharacterState : BaseCharacterState {

    public SwordMasterCharacterState(string name) : base(name) { }
    
    protected override void SetPrimaryAttribute()
    {
        GetPrimaryAttrubute(PrimaryAttributeName.Power).baseValue = Random.Range(30, 50);
        GetPrimaryAttrubute(PrimaryAttributeName.Agility).baseValue = Random.Range(20, 40);
        GetPrimaryAttrubute(PrimaryAttributeName.Wisdom).baseValue = Random.Range(0, 20);
        GetPrimaryAttrubute(PrimaryAttributeName.Constitution).baseValue = Random.Range(30, 50);
        GetPrimaryAttrubute(PrimaryAttributeName.Spirit).baseValue = Random.Range(20, 40);
        GetPrimaryAttrubute(PrimaryAttributeName.Luck).baseValue = Random.Range(0, 100);
    }
}
