using UnityEngine;
using System.Collections;
using System.Timers;

public class ConsumedStatusBuff : Buff
{
    private float ofTimeValue;
    private float onceValue;
    private BaseCharacterState status;
    float timePerTick;
    float timer=0;
    public float TotalMount(float buffTimeLong) {
        return onceValue + (buffTimeLong / timePerTick) * ofTimeValue;
    }
    
    public ConsumedStatusBuff(ConsumedAttributeName attr, BuffType type, float value,float onceValue, float ofTimeValue,float timePerTick) : base(attr, type, value)
    {
        this.ofTimeValue = ofTimeValue;
        this.onceValue = onceValue;
        this.timePerTick = timePerTick;
    }

    public override void OnBuff( BaseCharacterBehavior character,BaseCharacterBehavior caster, bool stackable = false, float updateValue = 0)
    {
        if (stackable)
        {
            stack++;
            valueOverLayer.Add(updateValue);
        }
        else
        {
            stack = 1;
            if (updateValue != 0)
                valueOverLayer[0] = updateValue;
        }
        this.caster = caster;
        ConsumedAttribute conAttr = character.status.GetConsumedAttrubute(this.attr3);
        int addMount = (int)(type == BuffType.Absolute ? valueOverLayer[stack - 1] : conAttr.baseValue * valueOverLayer[stack - 1]);
        conAttr.buffedValue += addMount;
        if(addMount!=0)
            character.buffHt.SetText(conAttr.name.ToString() + (addMount > 0 ? " +  " : " ") + addMount);

        switch (attr3)
        {
            case ConsumedAttributeName.Health:
                character.Heal(addMount,caster);
                character.Heal(onceValue, caster);
                if (addMount + onceValue < 0)
                    caster.CurrentMakeDamage += -(addMount + onceValue);
                break;
            case ConsumedAttributeName.Mana:
                character.ManaRecover(addMount);
                character.ManaRecover(onceValue);
                break;
            case ConsumedAttributeName.Energy:
                character.EnergyRecover(addMount);
                character.EnergyRecover(onceValue);
                break;
            default:
                break;
        }

        
        if (ofTimeValue != 0) {
            this.status = character.status;
            timer = Time.time;
        }
    }
    
    public void OnUpdate(BaseCharacterBehavior character) {
        if (timePerTick!=0 && Time.time - timer >= timePerTick)
        {
            switch (attr3)
            {
                case ConsumedAttributeName.Health:
                    character.Heal(ofTimeValue*stack,caster);
                    if (ofTimeValue < 0)
                        caster.CurrentMakeDamage -= ofTimeValue;
                    break;
                case ConsumedAttributeName.Mana:
                    character.ManaRecover(ofTimeValue*stack);
                    break;
                case ConsumedAttributeName.Energy:
                    character.EnergyRecover(ofTimeValue*stack);
                    break;
                default:
                    break;
            }
            
            timer = Time.time;
        }
    }
    public override void OnDeBuff(BaseCharacterBehavior character)
    {
        ConsumedAttribute conAttr = character.status.GetConsumedAttrubute(this.attr3);
        foreach (float value in valueOverLayer)
        {
            int addMount = (int)(type == BuffType.Absolute ? value: conAttr.baseValue * value) * -1;
            conAttr.buffedValue += addMount * stack;
            if (addMount != 0)
                character.buffHt.SetText(conAttr.name.ToString() + (addMount > 0 ? " + " : " ") + addMount);
        }
    }
}
