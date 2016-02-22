using UnityEngine;
using System.Collections;

public class MonsterCharacterState : BaseCharacterState {

    public MonsterCharacterState(string name):base(name) { }
    protected override void SetPrimaryAttribute()
    {
        for (int i=0;i< priAttributes.Length;i++)
        {
            priAttributes[i].baseValue = Random.Range(10,50);
        }
    }
}
