using UnityEngine;
using System.Collections.Generic;
using System.Linq;
public class SkillType {
    public static SkillName[] HealingSkills = new SkillName[] {
        SkillName.Recover,
        SkillName.Heal,
        SkillName.HealingShield,
    };
    public static SkillName[] BuffSkills = new SkillName[] {
        SkillName.WillOfFight,
        SkillName.Recover,
        SkillName.Heal,
        SkillName.HealingShield,
    };
    public static SkillName[] DeBuffSkills = new SkillName[] {
        SkillName.WillOfFight,    
        SkillName.Thander,
        SkillName.ArmorBreak,
        SkillName.Taunt,
        SkillName.DeadlyShot,
        SkillName.PoisonShot,    
        SkillName.Silence,
        SkillName.Disperse
    };
    public static SkillName[] MeleeSkills = new SkillName[] {
        SkillName.CrossAttack,
        SkillName.WindSlash,
        SkillName.ArmorBreak
    };
    public static SkillName[] RangeSkills = new SkillName[] {
        SkillName.IceBlade,
        SkillName.Heal,
        SkillName.Thander,
        SkillName.FireTornadal,
        SkillName.Taunt,
        SkillName.ArrowRain,
        SkillName.ImpactArrow,
        SkillName.DeadlyShot,
        SkillName.PoisonShot,
        SkillName.HealingShield,
        SkillName.FireWave,
        SkillName.Silence,
        SkillName.Disperse
    };
}

public enum TypeOfSkill
{
    Healing,
    Buff,
    Melee,
    Range
}
public enum SkillName
{
    //Player | test
    CrossAttack =0,
    Recover = 1,
    FireTornadal = 2,
    
    //Warior
    ArmorBreak = 11,
    WindSlash = 12,
    IceBlade = 13,
    //Knight
    Taunt = 21,
    WillOfFight = 22,
    //Archer
    ArrowRain = 31,
    ImpactArrow = 32,
    DeadlyShot = 33,
    PoisonShot = 34,
    //Priest
    Heal = 41,
    HealingShield = 42,
    //Mage
    Silence = 51,
    Disperse = 52,
    //Magician
    FireWave = 45,
    Thander = 46,
    //Tower
    WarBlessing = 91,
    WarRepression = 92

}