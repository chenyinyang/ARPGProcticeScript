using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

public class Player : BaseCharacterBehavior,ICommander
{
    internal static string PlayerName;
    internal static Player Instance;
    public PlayerJob job;
    
    private string[] TargetTag = new string[] { Tags.Monster, Tags.Enemy,Tags.Build };

    public List<NPCController> Followers { get; set; }

    protected override void Awake() {
        gameObject.AddComponent<PlayerMotion>().model = model;
        base.Awake();
        switch (job)
        {
            case PlayerJob.Swordmaster:
                status = new SwordMasterCharacterState(CharacterName);
                break;
            case PlayerJob.Archer:
                status = new ArcherCharacterState(CharacterName);
                break;
            case PlayerJob.Wizzard:
                status = new WizzardCharacterState(CharacterName);
                break;
            default:
                status = new SwordMasterCharacterState(CharacterName);
                break;
        }
        SetConsumedAttrUpdate();
        PlayerName = CharacterName;
        Instance = this;
        NavMeshObstacle navObstacle = gameObject.AddComponent<NavMeshObstacle>();
        navObstacle.shape = NavMeshObstacleShape.Capsule;
        navObstacle.height = GetComponent<CharacterController>().height;
        navObstacle.radius = GetComponent<CharacterController>().radius;
        navObstacle.center = GetComponent<CharacterController>().center;
        Followers = new List<NPCController>();

    }

    void SetConsumedAttrUpdate() {
        
        status.GetConsumedAttrubute(ConsumedAttributeName.Health).onValueUpdate += Player_onHealthUpdate;
        
        status.GetConsumedAttrubute(ConsumedAttributeName.Mana).onValueUpdate += Player_onManaUpdate;
        
        status.GetConsumedAttrubute(ConsumedAttributeName.Energy).onValueUpdate += Player_onEnergyUpdate;
        
    }

    private void Player_onEnergyUpdate(float curValue, float maxValue)
    {
        Messenger.Broadcast<float, float>(MessengerTopic.PLAYER_ENERGY_UPDATE, curValue, maxValue);
    }

    private void Player_onManaUpdate(float curValue, float maxValue)
    {
        Messenger.Broadcast<float, float>(MessengerTopic.PLAYER_MANA_UPDATE, curValue, maxValue);
    }

    private void Player_onHealthUpdate(float curValue, float maxValue)
    {
        Messenger.Broadcast<float, float>(MessengerTopic.PLAYER_HEALTH_UPDATE, curValue, maxValue);
    }

    // Use this for initialization
    protected override void Start () {
        base.Start();
        //        status.skills.Add(SkillGenerator.Instance().GetSkill(SkillGenerator.SkillName.WindSlash));
        
        status.GetConsumedAttrubute(ConsumedAttributeName.Health).CurValue = status.GetConsumedAttrubute(ConsumedAttributeName.Health).CurValue;
        status.GetConsumedAttrubute(ConsumedAttributeName.Mana).CurValue = status.GetConsumedAttrubute(ConsumedAttributeName.Mana).CurValue;
        status.GetConsumedAttrubute(ConsumedAttributeName.Energy).CurValue = status.GetConsumedAttrubute(ConsumedAttributeName.Energy).CurValue;
        for(int i=0;i<status.Skills.Count;i++)
        {
            MyGUI.Instence.SetSkillBtn(i, status.Skills[i].icon, status.Skills[i].CooldownLeft, status.Skills[i].SkillCoolDown);
        }
        
    }
    
    protected override void CheckBuff()
    {
        base.CheckBuff();
        base.UpdateStatistic();
        for (int i = 0; i < status.Skills.Count; i++)
        {
            MyGUI.Instence.SetSkillBtn(i, status.Skills[i].CooldownLeft, status.Skills[i].SkillCoolDown);

        }
        MyGUI.Instence.SetBuffIcon(buffs.ToArray());
    }
    public override bool CanDamageTarget(Damagable target)
    {
        for (int i = 0; i < TargetTag.Length; i++)
        {
            if (TargetTag[i] == target.tag)
                return true;
        }
        return false;
    }

    public override void Casting(SkillName name, float time, CastOver callBack)
    {
        base.Casting(name, time, callBack);
        MyGUI.Instence.ShowCastBar(name, time);
    }
    public override void CastingCancel()
    {
        base.CastingCancel();
        MyGUI.Instence.CancelCastBar();
    }

    protected override void LevelUp()
    {
        //Customize up attr
        for (int i = 0; i < 6; i++)
        {
            status.GetPrimaryAttrubute((PrimaryAttributeName)UnityEngine.Random.Range(0, 5)).baseValue += 1;
        }
        for (int i = 0; i <(int)SecondaryAttributeName.Count; i++)
        {
            status.GetSecondaryAttrubute((SecondaryAttributeName)i).baseValue += 5;
        }

        for (int i = 0; i < (int)ConsumedAttributeName.Count; i++)
        {
            status.GetConsumedAttrubute((ConsumedAttributeName)i).baseValue += 50;
            status.GetConsumedAttrubute((ConsumedAttributeName)i).CurValue += status.GetConsumedAttrubute((ConsumedAttributeName)i).AdjustedValue * .1f;
        }
    }

    public void CommandFollowersInvade()
    {
        for (int i = 0; i < Followers.Count; i++)
        {
            CommandFollowerInvade(Followers[i]);
        }
    }
    public void CommandFollowersAttack(Damagable target) {
        for (int i = 0; i < Followers.Count; i++)
        {
            Followers[i].AttackTo(target.MyTransform);
        }
    }
    public void CommandFollowersStopAttack()
    {
        for (int i = 0; i < Followers.Count; i++)
        {
            Followers[i].ClearAllCommand();
            Followers[i].Patrol(myTransform);
        }
    }
    public void CommandFollowerInvade(NPCController follower)
    {
        follower.Patrol(myTransform);
    }

    public void ChangeFollower(ICommander commander,NPCController follower)
    {        
        follower.ClearAllCommand();            
        commander.AddFollower(follower);            
        Followers.Remove(follower);
    }

    public void AddFollower(NPCController follower)
    {
        Followers.Add(follower);
        follower.commander = this;
        CommandFollowerInvade(follower);
    }
}

