using UnityEngine;
using System.Collections;

public class MainTower : Tower
{
    public bool Occupiable;
    protected override void Awake()
    {
        MaxHealth = 20000;
        CREATE_BREAD_INTERVAL = 10f;
        MAX_BREAD_COUNT = 20;
        INITIAL_BREAD_COUNT = 10;
        CREATE_FOLLOWER_INTERVAL = 1f;
        base.Awake();
    }
    protected override void onUpdate()
    {
        base.onUpdate();
        AILevel = Tower.TowerdDeffenceAILevel.AttackMageFirstThenLowHp;
    }
    protected override void BuildingDestroy()
    {
        if (type == NPCType.Friend)
        {
            //GameOver
        }
        else if (type == NPCType.Enemy) {
            //Win Game
        }
        base.BuildingDestroy();
        
    }
    protected override void UpdateOccupiedValue()
    {
        if(Occupiable)
            base.UpdateOccupiedValue();
    }
    protected override void giveCharacterBuff(BaseCharacterBehavior character)
    {
        if (Tags.IsCompanion(this, character))
        {
            //Buff
            //全第一屬性+5% *駐防數+1
            //生命回復+50%,能量/法力回復+10% * 駐防數+1
            //視野400%(駐防者)/200%(一般)
            //造成傷害+10%(駐防者)/+5%(一般) * 駐防數+1
            //傷害降低 塔的傷害降低的一半(駐防者)/(5%*駐防數+1)
            BuffSkill buff = new BuffSkill(SkillName.WarBlessing, 0, 0, 0, 0, 0, ConsumedAttributeName.Health, 0, 15f, new Buff[] {
                    new Buff(PrimaryAttributeName.Power, Buff.BuffType.Relative,.2f),
                    new Buff(PrimaryAttributeName.Agility, Buff.BuffType.Relative,.2f),
                    new Buff(PrimaryAttributeName.Constitution, Buff.BuffType.Relative,.2f),
                    new Buff(PrimaryAttributeName.Wisdom, Buff.BuffType.Relative,.2f),
                    new Buff(PrimaryAttributeName.Spirit, Buff.BuffType.Relative,.2f),
                    new Buff(SecondaryAttributeName.HealthRecoverRate, Buff.BuffType.Relative,2f),
                    new Buff(SecondaryAttributeName.EnergyRecoverRate, Buff.BuffType.Relative,.4f),
                    new Buff(SecondaryAttributeName.ManaRecoverRate, Buff.BuffType.Relative,.4f),
                    new Buff(SecondaryAttributeName.Vision, Buff.BuffType.Relative,3f),
                    new Buff(StaticAttributeName.DamageMake, Buff.BuffType.Absolute,.4f),
                    new Buff(StaticAttributeName.DamageTakeFix, Buff.BuffType.Absolute,- .2f)
                }, false);
            buff.SetCaster(character);
            buff.Cast(character);
        }
        else
        {
            //Debuff
            //全第一屬性-5% *1+駐防數
            //受到傷害+3% * 駐防數
            BuffSkill buff = new BuffSkill(SkillName.WarRepression, 0, 0, 0, 0, 0, ConsumedAttributeName.Health, 0, 15f, new Buff[] {
                    new Buff(PrimaryAttributeName.Power, Buff.BuffType.Relative,-.2f),
                    new Buff(PrimaryAttributeName.Agility, Buff.BuffType.Relative,-.2f),
                    new Buff(PrimaryAttributeName.Constitution, Buff.BuffType.Relative,-.2f),
                    new Buff(PrimaryAttributeName.Wisdom, Buff.BuffType.Relative,-.2f),
                    new Buff(PrimaryAttributeName.Spirit, Buff.BuffType.Relative,-.2f),
                    new Buff(StaticAttributeName.DamageTakeFix, Buff.BuffType.Absolute, .1f)
                }, false);
            buff.SetCaster(character);
            buff.Cast(character);
        }
    }
}
