using UnityEngine;
using System.Collections.Generic;
using System;

public class Tower : Build ,ICommander{


    //setting
    //施放BUFF的頻率
    protected const float BUFF_CAST_INTERVAL = 10f;
    //計算佔領值的頻率
    protected const float OCCUPIED_CALCULATE_INTERVAL = .1f;
        //產生麵包的頻率
    protected float CREATE_BREAD_INTERVAL = 30f;
    //招換士兵的頻率
    protected float CREATE_FOLLOWER_INTERVAL = 5f;
    //每次佔領值的量
    protected const float OCCUPIED_STEP = .002f;
    //最大麵包數
    internal int MAX_BREAD_COUNT = 10;
    //起使麵包數
    protected int INITIAL_BREAD_COUNT = 5;
    //最大士兵數
    protected const int MAX_FOLLOWERS_COUNT = 5;

    
    internal Transform MainFlag;
    //狀態靈氣    
    List<BaseCharacterBehavior> characterInBuffRange;
    ParticleSystem nova;
    ParticleSystem buffGround;
    Transform buffRange;
    //戰領判斷
    internal float OccupiedValue;
    //麵包
    internal int BreadCount;

    //生兵    
    //public List<NPCController> Followers { get; set; }
    internal List<NPCController> TowerFollowers;
    Vector3 followerSpawnPoint;
    ParticleSystem createFollowerEffect;

    //地圖
    public Tower[] TowerNearBy;
    internal List<Tower> GetNearAliveTowers(List<Tower> path) {
        if (path.Contains(this))
            return new List<Tower>();
        path.Add(this);
        List<Tower> ts = new List<Tower>();
        for (int i = 0; i < TowerNearBy.Length; i++)
        {
            if (!TowerNearBy[i].CompareTag(Tags.DeadBody))
                ts.Add(TowerNearBy[i]);
            else {
                List<Tower> temp = TowerNearBy[i].GetNearAliveTowers(path);
                for (int j = 0; j < temp.Count; j++)
                {
                    ts.Add(temp[j]);
                }
            }
        }
        return ts;
    }
    protected Tower GetNearestTower(bool isCompanion,bool isCombat) {
        Tower t = null;
        List<Tower> path = new List<Tower>();
        List<Tower> ts = GetNearAliveTowers(path);

        for (int i = 0; i < ts.Count; i++)
        {            
            if (Tags.IsCompanion(this, ts[i])==isCompanion 
                && ts[i].IsCombat() == isCombat
                && (t == null || Vector3.Distance(buildPosition, ts[i].buildPosition)
                                < Vector3.Distance(buildPosition, t.buildPosition)))
                t = ts[i];
        }
        return t;
    }
    
    //指揮(防守
    protected enum TowerdDeffenceAILevel {
        DoNothing =0,
        AttackNearest = 1,
        AttackLowHp = 2,
        AttackMageFirstThenLowHp = 3
    }
    float AITimeInterval = 3f;
    protected TowerdDeffenceAILevel AILevel = TowerdDeffenceAILevel.AttackNearest;
    Transform TowerTarget;
    BaseCharacterBehavior AttackTarget;

    //指令(侵略)
    public enum InvadeCmd {
        DoNothing,
        OccupyNearestTower,
        DestroyNearestTower,
        SupportNearestTower
    }
    public InvadeCmd invadeCommand = InvadeCmd.DestroyNearestTower;
    const float InvadeAIInterval = .1f;
    public bool IsCombat()
    {
        for (int i = 0; i < characterInBuffRange.Count; i++)
        {
            if (!Tags.IsCompanion(this, characterInBuffRange[i]))
                return true;
        }
        return false;
    }
    protected virtual void TowerInvadeAI()
    {
        if (type == NPCType.Neutral) {
            invadeCommand = InvadeCmd.DoNothing;
            return;
        }
        Tower nearestCompanionTower = GetNearestTower(true,false);
        Tower nearestEnemyTower = GetNearestTower(false, false);
        Tower nearestCompanionTowerInCombat = GetNearestTower(true, true);
        Tower nearestEnemyTowerInCombat = GetNearestTower(false, false);
        if (IsCombat())
        {
            List<NPCController> npcInRange = new List<NPCController>();
            for (int i = 0; i < characterInBuffRange.Count; i++)
            {
                if (characterInBuffRange[i] is NPCController && Tags.IsCompanion(this, characterInBuffRange[i]))
                {
                    //只針對範圍內的下命令
                    npcInRange.Add(characterInBuffRange[i] as NPCController);
                    
                }
            }
            //是塔的排前面
            npcInRange.Sort((a, b) =>
            {
                if (TowerFollowers.Contains(a) && !TowerFollowers.Contains(b))
                    return -1;
                if (TowerFollowers.Contains(b) && !TowerFollowers.Contains(a))
                    return 1;
                return 0;
            });
            switch (invadeCommand)
            {
                case InvadeCmd.DoNothing:
                    //戰鬥中將npc駐防
                    for (int i = 0; i < stationNPC.Length; i++)
                    {
                        if (npcInRange.Count > i && stationNPC[i] == null && TowerFollowers.Contains(npcInRange[i]))
                        {
                            stationNPC[i] = npcInRange[i];
                            npcInRange[i].Garrison(stationPosition[i]);
                        }
                    }
                    return;
                case InvadeCmd.OccupyNearestTower:
                    invadeCommand = InvadeCmd.DoNothing;
                    break;
                case InvadeCmd.DestroyNearestTower:
                    invadeCommand = InvadeCmd.DoNothing;
                    break;
                case InvadeCmd.SupportNearestTower:
                    //外調的時候被打,開始防守                    
                    invadeCommand = InvadeCmd.DoNothing;
                    break;
            }
            for (int i = 0; i < npcInRange.Count; i++)
            {
                //只針對範圍內的下命令
                CommandFollowerInvade(npcInRange[i]);
            }
        }
        else
        {
            switch (invadeCommand)
            {
                case InvadeCmd.DoNothing:

                    if (TowerFollowers.Count >= 5)
                    {
                        ////滿了就解除駐守全送出去打
                        //for (int i = 0; i < stationNPC.Length; i++)
                        //{
                        //    if (stationNPC[i] != null)
                        //    {
                        //        stationNPC[i].Garrison(null);
                        //        stationNPC[i] = null;                                
                        //    }
                        //}
                        //鄰塔有戰事,支援林塔
                        if (nearestCompanionTowerInCombat != null)
                            invadeCommand = InvadeCmd.SupportNearestTower;
                        //或者衝最進敵塔
                        else if (nearestEnemyTower != null)
                            invadeCommand = InvadeCmd.DestroyNearestTower;
                    }
                    else
                    {
                        //平時敵塔有戰事 增援
                        if (nearestCompanionTowerInCombat != null )
                            invadeCommand = InvadeCmd.DestroyNearestTower;
                    }
                    
                    break;
                case InvadeCmd.OccupyNearestTower:
                    if (nearestEnemyTower == null)
                        invadeCommand = InvadeCmd.DoNothing;
                    break;
                case InvadeCmd.DestroyNearestTower:
                    if (nearestEnemyTower == null)
                        invadeCommand = InvadeCmd.DoNothing;
                    break;
                case InvadeCmd.SupportNearestTower:
                    if (nearestCompanionTowerInCombat == null)
                    {
                        invadeCommand = InvadeCmd.DoNothing;
                    }                    
                    break;
            }
            CommandFollowersInvade();
        }
    }
    //駐守
    internal NPCController[] stationNPC = new NPCController[3];
    Transform[] stationPosition = new Transform[3];

    protected Transform eargleEyeImgTransform;
    #region MonoBehaviour Life Cycle
    protected override void Awake()
    {
        IsTower = true;
        base.Awake();
        buffRange = myTransform.Find("BuffRange");
        ColliderChild cc = buffRange.GetComponent<ColliderChild>();
        cc.triggerEnter = BuffRangeEnter;
        cc.triggerExit = BuffRangeExit;
        nova = buffRange.Find("magicCircleSingle").Find("auraWall").GetComponent<ParticleSystem>();
        buffGround = buffRange.Find("magicCircleSingle").GetComponent<ParticleSystem>();
        characterInBuffRange = new List<BaseCharacterBehavior>();

        TowerFollowers = new List<NPCController>();
        Followers = new List<NPCController>();
        followerSpawnPoint = myTransform.Find("SpawnPoint").position;
        createFollowerEffect = myTransform.Find("SpawnPoint").Find("magicGate").GetComponent<ParticleSystem>();
        
        TowerTarget = myTransform.Find("TowerTarget");
        TowerTarget.GetComponent<UIFollowTarget>().gameCamera = Camera.main;
        //TowerTarget.GetComponent<UIFollowTarget>().uiCamera = GameObject.Find("UI Root").GetComponentInChildren<Camera>();
        TowerTarget.gameObject.SetActive(false);
        TowerManager.AddTower(this);
        for (int i = 0; i < stationPosition.Length; i++)
        {
            stationPosition[i] = transform.Find("StationPoints").GetChild(i);
        }
        MainFlag = myTransform.Find("MainFlag");

        //CreatePlaneForEargleEye();
        Mesh mesh = ResourceLoader.Utilities.GetCirclePlane();
        GameObject eargleEyeImg = new GameObject();
        eargleEyeImgTransform = eargleEyeImg.transform;
        eargleEyeImg.name = "EargleEyeImg";
        eargleEyeImg.AddComponent<MeshFilter>().mesh = mesh;
        MeshRenderer eargleEyeImgMeshRenderer = eargleEyeImg.AddComponent<MeshRenderer>();
        eargleEyeImgMeshRenderer.materials = new Material[] {
            new Material(Shader.Find("Sprites/Default"))
        };
      
        eargleEyeImgMeshRenderer.reflectionProbeUsage = UnityEngine.Rendering.ReflectionProbeUsage.Off;
        eargleEyeImgMeshRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        eargleEyeImg.layer = 12;
        eargleEyeImgTransform.parent = myTransform;
        eargleEyeImgTransform.localPosition = Vector3.zero;
        eargleEyeImgTransform.position += Vector3.up * 10f;
        eargleEyeImgTransform.localScale = Vector3.one * 1.5f;
    }
   
    protected override void Start()
    {  
        MainFlag.GetComponent<Damagable>().damageHandler += Tower_damageHandler;
        SetNPCType(type);
      
        BreadCount = INITIAL_BREAD_COUNT;        
        AILevel = TowerdDeffenceAILevel.AttackNearest;
        base.Start();
        InvokeRepeating("CastBuff", 0, BUFF_CAST_INTERVAL);
        InvokeRepeating("UpdateOccupiedValue", 0, OCCUPIED_CALCULATE_INTERVAL);
        InvokeRepeating("MakeBread", 0, CREATE_BREAD_INTERVAL);
        InvokeRepeating("CreateFollower", 0, CREATE_FOLLOWER_INTERVAL);
        InvokeRepeating("CommandFollowerDefence", 0, AITimeInterval);
        InvokeRepeating("TowerInvadeAI", 0, InvadeAIInterval);
    }
    protected override void Build_damageHandler(DamageType type, IDamageMaker damageMaker, BaseCharacterBehavior source)
    {
        NPCController highestHPStationer=null;
        for (int i = 0; i < stationNPC.Length; i++)
        {
            if(stationNPC[i]!=null &&(highestHPStationer==null ||
                stationNPC[i].status.GetConsumedAttrubute(ConsumedAttributeName.Health).CurValue>
                 highestHPStationer.status.GetConsumedAttrubute(ConsumedAttributeName.Health).CurValue))
            {
                highestHPStationer = stationNPC[i];
            }                
        }
        if (highestHPStationer != null)
        {
            highestHPStationer.Damage(type, damageMaker, source);
        }
        else {
            base.Build_damageHandler(type, damageMaker, source);
        }
    }
    private void Tower_damageHandler(DamageType type, IDamageMaker damageMaker, BaseCharacterBehavior source)
    {
        bool isCritical;
        float addHit;
        float dmg = damageMaker.GetCurDamage(type,out isCritical,out addHit);
        hp.CurValue -= dmg;
        ht.SetText((int)dmg * -1, isCritical, false);
        source.CurrentMakeDamage += dmg;
    }

    protected override void onUpdate()
    {
        base.onUpdate();
        int aiLevel = 0;
        for (int i = 0; i < stationNPC.Length; i++)
        {
            if (stationNPC[i] != null && stationNPC[i].IsInGarrison)
                aiLevel++;
        }
        AILevel = (TowerdDeffenceAILevel)aiLevel;
    }
    protected override void BuildingDestroy()
    {
        TowerLossControl();
        CancelInvoke("CastBuff");
        CancelInvoke("UpdateOccupiedValue");
        CancelInvoke("MakeBread");
        CancelInvoke("CreateFollower");
        CancelInvoke("CommandFollowerDefence");
        CancelInvoke("TowerInvadeAI");
        Destroy(eargleEyeImgTransform.gameObject);
        base.BuildingDestroy();
        MainFlag.tag = Tags.DeadBody;
        buffGround.gameObject.SetActive(false);
        MainFlag.GetComponent<Damagable>().enabled = false;
        buffRange.gameObject.SetActive(false);
        
    }
    #endregion

    #region Tower Commands
    private void CommandFollowerDefence() {
        AttackTarget = null;
        TowerTarget.gameObject.SetActive(false);
        if (AILevel == TowerdDeffenceAILevel.DoNothing)
            return;

        List<BaseCharacterBehavior> allowedEnemy = new List<BaseCharacterBehavior>();
        for (int i = 0; i < characterInBuffRange.Count; i++)
        {
            if (!Tags.IsCompanion(this, characterInBuffRange[i]))
                allowedEnemy.Add(characterInBuffRange[i]);
        }        
        if (AILevel == TowerdDeffenceAILevel.AttackMageFirstThenLowHp) {
            List<BaseCharacterBehavior> mageEnemies = new List<BaseCharacterBehavior>();
            for (int i = 0; i < allowedEnemy.Count; i++)
            {
                var t = allowedEnemy[i];
                if (t is NPCController && (t as NPCController).job == NPCJobs.Mage)
                    mageEnemies.Add(allowedEnemy[i]);
            }            
            if (mageEnemies.Count > 0) {
                allowedEnemy = mageEnemies;
            }
        }
        if (allowedEnemy.Count == 0)
            return;
        
        BaseCharacterBehavior attackTarget = allowedEnemy[0];
        for (int i = 1; i < allowedEnemy.Count; i++)
        {
            switch (AILevel)
            {
                case TowerdDeffenceAILevel.AttackNearest:
                    if ((attackTarget.transform.position - buildPosition).magnitude >
                        (allowedEnemy[i].transform.position - buildPosition).magnitude)
                        attackTarget = allowedEnemy[i];
                    break;
                case TowerdDeffenceAILevel.AttackLowHp:
                case TowerdDeffenceAILevel.AttackMageFirstThenLowHp:
                    if (attackTarget.status.GetConsumedAttrubute(ConsumedAttributeName.Health).CurValue >
                        allowedEnemy[i].status.GetConsumedAttrubute(ConsumedAttributeName.Health).CurValue)
                        attackTarget = allowedEnemy[i];
                    break;
                default:
                    break;
            }
        }
        
        List<BaseCharacterBehavior> friendInRnge = new List<BaseCharacterBehavior>();
        for (int i = 0; i < characterInBuffRange.Count; i++)
        {
            if (Tags.IsCompanion(this, characterInBuffRange[i]))
                friendInRnge.Add(characterInBuffRange[i]);
        }
        
        Transform attackTargetTransform = attackTarget.transform;        
        for (int i=0;i<friendInRnge.Count;i++)
        {
            if (friendInRnge[i] is NPCController)
                (friendInRnge[i] as NPCController).AttackTo(attackTargetTransform);
        }
        AttackTarget = attackTarget;
        
        TowerTarget.gameObject.SetActive(true);
        TowerTarget.GetComponent<UIFollowTarget>().target = attackTargetTransform;
        
        
    }
    public void CommandFollowersInvade() {
       
        for (int i = 0; i < Followers.Count; i++)
        {            
            CommandFollowerInvade(Followers[i]);
        }        
    }
    public void CommandFollowerInvade(NPCController follower) {
        if (!characterInBuffRange.Contains(follower))
        {
            return;
        }
        if (invadeCommand == InvadeCmd.DoNothing)
        {
            follower.Patrol(MainFlag);
            return;
        }
        follower.Patrol(null);
        follower.Garrison(null);
        
        Tower nearestCompanionTower = GetNearestTower(true, false)?? GetNearestTower(true, true);
        Tower nearestEnemyTower = GetNearestTower(false, false)?? GetNearestTower(false, true);
        switch (invadeCommand)
        {
            case InvadeCmd.OccupyNearestTower:
                if (nearestEnemyTower == null)
                    return;
                follower.Patrol(nearestEnemyTower.MainFlag);
                break;
            case InvadeCmd.DestroyNearestTower:
                if (nearestEnemyTower == null)
                    return;
                follower.AttackTo(nearestEnemyTower.MainFlag);
                break;
            case InvadeCmd.SupportNearestTower:
                if (nearestCompanionTower == null)
                    return;
                
                follower.Patrol(nearestCompanionTower.MainFlag);
                if (nearestCompanionTower != this)
                    ChangeFollower(nearestCompanionTower, follower);
                
                //if(!nearestCompanionTower.Followers.Contains(follower))
                //    nearestCompanionTower.otherFollower.Add(follower);
                break;
            case InvadeCmd.DoNothing:
            default:
                break;
        }
    }
    #endregion

    #region Auto tower behaviours
    int priestCount=0;
    int knightCount =0;
    int archerCount =0;

    public List<NPCController> Followers
    {
        get;
        set;
    }

    private bool CreateFollower() {
        //沒資源
        if (BreadCount <= 0)
            return false;
        //數量超出
        if (TowerFollowers.Count >= MAX_FOLLOWERS_COUNT)
            return false;
        NPCJobs followerJob ;
        //int mageCount = Followers.Sum(npc => npc.job == NPCJobs.Mage ? 1 : 0);
        //int warCount = Followers.Sum(npc => npc.job == NPCJobs.Warrior ? 1 : 0);
        //int archerCount = Followers.Sum(npc => npc.job == NPCJobs.Archer ? 1 : 0);
        if (type == NPCType.Neutral) {
            followerJob = NPCJobs.Monster;
        }else
        if (knightCount == 0)
        {
            //沒騎士時
            followerJob = NPCJobs.Knight;
        }
        else if (priestCount == 0)
        {
            //沒mage時
            followerJob = NPCJobs.Priest;
        }
        else 
        {
            if(priestCount ==2)
                followerJob = (NPCJobs)UnityEngine.Random.Range(0, (int)NPCJobs.Count-2); //不出法師
            else
                followerJob = (NPCJobs)UnityEngine.Random.Range(0, (int)NPCJobs.Count - 1);
        }


        GameObject npcPrefab = ResourceLoader.NPC.GetNPCPrefab(followerJob);
        if (npcPrefab == null)
            return false;
        createFollowerEffect.Play();
        GameObject newFollowerObj =(GameObject) Instantiate(npcPrefab, followerSpawnPoint, Quaternion.AngleAxis(UnityEngine.Random.Range(0, 360), Vector3.up));
        NPCController newFollower = newFollowerObj.GetComponent<NPCController>();        
        newFollower.CharacterName = ResourceLoader.NPC.GetName(followerJob);
        newFollower.name = BuildName + "_" + newFollower.CharacterName;
        newFollower.SetNPCType(type);
        TowerFollowers.Add(newFollower);
        AddFollower(newFollower);
        newFollower.OnDead += NewFollower_OnDead;
        switch (type)
        {
            case NPCType.Enemy:
                FactionManager.EnemyFactionManager.AddNPC(newFollower);
                break;
            case NPCType.Friend:
                FactionManager.PlayerFactionManager.AddNPC(newFollower);
                break;
            case NPCType.Neutral:
                FactionManager.MonsterFactionManager.AddNPC(newFollower);
                break;
            default:
                break;
        }
        //newFollower.Patrol(transform);
        CommandFollowerInvade(newFollower);
        switch (followerJob)
        {
            case NPCJobs.Knight:
                knightCount++;
                break;
            case NPCJobs.Archer:
                archerCount++;
                break;
            case NPCJobs.Priest:
                priestCount++;
                break;
        }
        BreadCount--;
        
        return true;
    }    

    private void NewFollower_OnDead(BaseCharacterBehavior npc)
    {
        for (int i = 0; i < stationNPC.Length; i++)
        {
            if (stationNPC[i] == npc)
                stationNPC[i] = null;
        }
        TowerFollowers.Remove(npc as NPCController);        
        switch ((npc as NPCController).job)
        {
            case NPCJobs.Knight:
                knightCount--;
                break;
            case NPCJobs.Archer:
                archerCount--;  
                break;
            case NPCJobs.Priest:
                priestCount--;
                break;
        }
    }
    private void MakeBread()
    {
        if (BreadCount < MAX_BREAD_COUNT)
            BreadCount++;
        else {
            for (int i = 0; i < TowerNearBy.Length; i++)
            {
                if(Tags.IsCompanion(this,TowerNearBy[i]) && TowerNearBy[i].BreadCount < TowerNearBy[i].MAX_BREAD_COUNT)
                {
                    TowerNearBy[i].MakeBread();
                    break;
                }
            }
        }
    }
    protected virtual void UpdateOccupiedValue() {
        float totalFriendlyHealth = 0;
        //Hostility , 敵對的,敵意
        float totalEnemyHealth = 0;
        float totalMonsterHealth = 0;
        List<BaseCharacterBehavior> updateChrs = new List<BaseCharacterBehavior>();
        for (int i=0;i< characterInBuffRange.Count;i++)
        {
            BaseCharacterBehavior character = characterInBuffRange[i];
            if (character != null)
            {
                updateChrs.Add(character);
                float hp = character.status.GetConsumedAttrubute(ConsumedAttributeName.Health).CurValue;
                //中立怪不算
                if (character.CompareTag(Tags.Monster)) {
                    totalMonsterHealth += hp;
                    continue;
                }
                //玩家方
                if (Tags.IsCompanion(Player.Instance, character))
                {
                    totalFriendlyHealth += hp;
                }
                else
                {
                    totalEnemyHealth += hp;
                }
            }
        }

        if ((type == NPCType.Neutral && totalMonsterHealth == 0) || type != NPCType.Neutral)
        {
            if (totalFriendlyHealth > totalEnemyHealth * 2f)
            {
                //友方式有善方->紅的減少
                if (OccupiedValue > 0)
                    OccupiedValue -= OCCUPIED_STEP;
            }
            else if (totalFriendlyHealth * 2f < totalEnemyHealth)
            {
                //敵方血量高

                //友方式有善方->紅的增加.
                if (OccupiedValue < 1)
                    OccupiedValue += OCCUPIED_STEP;

            }
        }
        if (OccupiedValue < 0 && type!= NPCType.Friend) {
            SetNPCType(NPCType.Friend);
            TowerLossControl();
        }
        if (OccupiedValue > 1 && type != NPCType.Enemy)
        {
            SetNPCType(NPCType.Enemy);
            TowerLossControl();
        }
        characterInBuffRange = updateChrs;

    }
    private void TowerLossControl() {
        for (int i = 0; i < stationNPC.Length; i++)
        {
            if (stationNPC[i] != null)
            {
                stationNPC[i].Garrison(null);
                stationNPC[i] = null;
            }
        }
        invadeCommand = InvadeCmd.SupportNearestTower;
        CommandFollowersInvade();
        TowerFollowers.Clear();
        Followers.Clear();
        invadeCommand = InvadeCmd.DoNothing;
    }
    private void CastBuff() {
        //show ground effect
        nova.Emit(1);
        List<BaseCharacterBehavior> updateChrs = new List<BaseCharacterBehavior>();
        for (int i=0;i<characterInBuffRange.Count;i++)
        {
            if (characterInBuffRange[i] != null)
            {
                updateChrs.Add(characterInBuffRange[i]);
                giveCharacterBuff(characterInBuffRange[i]);
            }
        }
        characterInBuffRange = updateChrs;

    }
    #endregion

    #region Character Enter/ exit/ dead controll
    private void BuffRangeEnter(Collider other)
    {
        //針對玩家
        BaseCharacterBehavior character = other.GetComponent<BaseCharacterBehavior>();
        if (character != null)
        {
            characterInBuffRange.Add(character);            
            giveCharacterBuff(character);
            if (AttackTarget == null)
                CommandFollowerDefence();
            character.OnDead += CharacterInRange_OnDead;
        }
    }
    private void BuffRangeExit(Collider other)
    {
        //針對玩家
        BaseCharacterBehavior character = other.GetComponent<BaseCharacterBehavior>();
        if (character != null)
        {
            if (characterInBuffRange.Contains(character))
            {
                characterInBuffRange.Remove(character);
                character.RemoveBuff(SkillName.WarBlessing);
                if (AttackTarget == character) {
                    List<BaseCharacterBehavior> friendInRnge = new List<BaseCharacterBehavior>();
                    for (int i = 0; i < characterInBuffRange.Count; i++)
                    {
                        if (Tags.IsCompanion(this, characterInBuffRange[i]))
                            friendInRnge.Add(characterInBuffRange[i]);
                    }
                    Transform attackTargetTransform = AttackTarget.transform;
                    for (int i = 0; i < friendInRnge.Count; i++)
                    {
                        if (friendInRnge[i] is NPCController)
                            (friendInRnge[i] as NPCController).RemoveAttackTo(attackTargetTransform);
                    }
                    CommandFollowerDefence();
                }
                character.OnDead -= CharacterInRange_OnDead;
            }
        }
    }
    protected virtual void giveCharacterBuff(BaseCharacterBehavior character) {
        int stationCount = 1;
        bool isGarrisoner = false;
        for (int i = 0; i < stationNPC.Length; i++)
        {
            if (stationNPC[i] != null && stationNPC[i].IsInGarrison)
            {
                stationCount++;
                if (stationNPC[i] == character)
                    isGarrisoner = true;
            }
        }        

        if (Tags.IsCompanion(this, character))
        {
            //Buff
            //全第一屬性+5% *駐防數+1
            //生命回復+50%,能量/法力回復+10% * 駐防數+1
            //視野400%(駐防者)/200%(一般)
            //造成傷害+10%(駐防者)/+5%(一般) * 駐防數+1
            //傷害降低 塔的傷害降低的一半(駐防者)/(5%*駐防數+1)
            BuffSkill buff = new BuffSkill(SkillName.WarBlessing, 0, 0, 0, 0, 0, ConsumedAttributeName.Health, 0, 15f, new Buff[] {
                    new Buff(PrimaryAttributeName.Power, Buff.BuffType.Relative,.05f*stationCount),
                    new Buff(PrimaryAttributeName.Agility, Buff.BuffType.Relative,.05f*stationCount),
                    new Buff(PrimaryAttributeName.Constitution, Buff.BuffType.Relative,.05f*stationCount),
                    new Buff(PrimaryAttributeName.Wisdom, Buff.BuffType.Relative,.05f*stationCount),
                    new Buff(PrimaryAttributeName.Spirit, Buff.BuffType.Relative,.05f*stationCount),
                    new Buff(SecondaryAttributeName.HealthRecoverRate, Buff.BuffType.Relative,.5f*stationCount),
                    new Buff(SecondaryAttributeName.EnergyRecoverRate, Buff.BuffType.Relative,.1f*stationCount),
                    new Buff(SecondaryAttributeName.ManaRecoverRate, Buff.BuffType.Relative,.1f*stationCount),
                    new Buff(SecondaryAttributeName.Vision, Buff.BuffType.Relative,isGarrisoner?3f: 1f),
                    new Buff(StaticAttributeName.DamageMake, Buff.BuffType.Absolute,(isGarrisoner?.1f: .05f)*stationCount),
                    new Buff(StaticAttributeName.DamageTakeFix, Buff.BuffType.Absolute,-(isGarrisoner?DamageReduce : .05f*stationCount))
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
                    new Buff(PrimaryAttributeName.Power, Buff.BuffType.Relative,-.05f*stationCount),
                    new Buff(PrimaryAttributeName.Agility, Buff.BuffType.Relative,-.05f*stationCount),
                    new Buff(PrimaryAttributeName.Constitution, Buff.BuffType.Relative,-.05f*stationCount),
                    new Buff(PrimaryAttributeName.Wisdom, Buff.BuffType.Relative,-.05f*stationCount),
                    new Buff(PrimaryAttributeName.Spirit, Buff.BuffType.Relative,-.05f*stationCount),
                    new Buff(StaticAttributeName.DamageTakeFix, Buff.BuffType.Absolute, .03f*(stationCount-1))
                }, false);
            buff.SetCaster(character);
            buff.Cast(character);
        }
    }
    private void CharacterInRange_OnDead(BaseCharacterBehavior npc)
    {
        characterInBuffRange.Remove(npc);
        if(AttackTarget == npc)
            CommandFollowerDefence();
    }
    #endregion

    public bool AssignNPCStay(NPCController npc) {
        for (int i = 0; i < stationNPC.Length; i++)
        {
            if (stationNPC[i] == null)
            {
                stationNPC[i] = npc;
                npc.Garrison(stationPosition[i]);
                return true;
            }
        }
        return false;
    }
    public bool CancelNPCStay(NPCController npc)
    {
        for (int i = 0; i < stationNPC.Length; i++)
        {
            if (stationNPC[i] == npc)
            {
                stationNPC[i] = null;
                npc.Garrison(null);
                return true; 
            }
        }
        return false;
    }
    public void SetNPCType(NPCType type) {
        this.type = type;

        switch (type)
        {
            case NPCType.Enemy:                
                this.tag = Tags.Enemy;
                MainFlag.tag = Tags.Enemy;
                buffGround.startColor = Color.red;
                OccupiedValue = 1f;
                break;
            case NPCType.Friend:
                this.tag = Tags.Friend;
                MainFlag.tag = Tags.Friend;
                buffGround.startColor = Color.blue;
                OccupiedValue = 0f;
                break;
            case NPCType.Neutral:
            default:
                this.tag = Tags.Monster;
                MainFlag.tag = Tags.Monster;
                buffGround.startColor = Color.white;
                OccupiedValue = .5f;
                break;
        }
        buffGround.Play();
        Color npcColor = type == NPCType.Enemy ?
               new Color(1, .5f, .5f) :
                   type == NPCType.Friend ? new Color(.5f, .5f, 1f) :
                   new Color(1, 1, 1);
        eargleEyeImgTransform.GetComponent<MeshRenderer>().materials[0].color = npcColor;

        Mesh castleCylinder = ResourceLoader.NPCTypeObject.GetCastleCylinder(type);
        Mesh castlePlane = ResourceLoader.NPCTypeObject.GetCastlePlane(type);
        Mesh main = ResourceLoader.NPCTypeObject.GetCastleMainFlag(type);
        MainFlag.GetComponent<MeshFilter>().mesh = main;
        for (int i = 0; i < MainFlag.Find("BannerCollection").childCount; i++)
        {
            Transform child = MainFlag.Find("BannerCollection").GetChild(i);
            if (child.name == "Cylinder") {
                child.GetComponent<MeshFilter>().mesh = castleCylinder;
            }
            if (child.name == "Plane")
            {
                child.GetComponent<MeshFilter>().mesh = castlePlane;
            }
        }
        
    }

   
    public void ChangeFollower(ICommander commander, NPCController follower)
    {
        for (int j = 0; j < stationNPC.Length; j++)
        {
            if (stationNPC[j] == follower)
            {
                return;
            }
        }            
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
