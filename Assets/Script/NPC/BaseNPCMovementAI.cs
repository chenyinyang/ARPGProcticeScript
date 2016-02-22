using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BaseNPCMovementAI : MonoBehaviour
{
    private NPCType Type;
    public NPCType NPCType
    {
        get
        {
            return Type;
        }
        set
        {
            if (TargetTag == null)
                TargetTag = new Dictionary<string, int>();
            else
                TargetTag.Clear();
            switch (value)
            {
                case NPCType.Enemy:
                    TargetTag.Add(Tags.Friend, 9);
                    TargetTag.Add(Tags.Player, 7);
                    TargetTag.Add(Tags.Monster, 8);
                    tag = Tags.Enemy;
                    break;
                case NPCType.Friend:
                    TargetTag.Add(Tags.Enemy, 9);
                    TargetTag.Add(Tags.Monster, 8);
                    tag = Tags.Friend;
                    break;
                case NPCType.Neutral:
                    TargetTag.Add(Tags.Friend, 9);
                    TargetTag.Add(Tags.Player, 8);
                    TargetTag.Add(Tags.Enemy, 9);
                    tag = Tags.Monster;
                    break;
                default:
                    break;
            }
            TargetTag.Add(Tags.Build, 0);
            Type = value;
        }
    }
    public enum NPCMotion
    {
        Deciding,
        Idle,
        Search,
        Scan,
        Move,
        Fallow,
        Casting,
        Garrison
    }
    protected const float VISION_DEPTH_AGI_RATIO = 100;
    protected const float RANGER_VISION_DEPTH_FIX_RATIO = 1f;
    protected const float ROTATION_SPEED_MOVE_SPEED_RATIO = 2;
    protected const float MOVE_SPEED_RATIO = 0.005f;
    protected const float VISION_WIDTH_AGI_RATIO = 2;
    protected const float CHANCE_TO_SEARCH_WIS_RATIO = 100;
    protected const float LENGTH_TO_SEARCH_WIS_RATIO = 100;
    protected const float CAUTION_TIME_SPIRIT_RATIO = 50;
    protected const float CAUTION_TIME_BASE_VALUE = 1;
    protected const float CAUTION_VISION_DEPTH_FIX_RATIO = 2;
    protected const float RANGER_MIN_ATTACK_DISTANCE_BY_BODY_LANGTH = 5;
    protected const float FALLOW_WHEN_IDLE_MIN_DISTANCE_BY_BODY_LANGTH = 3;
    internal NPCController npc;
    Transform myTransform;
    CharacterController controller;
    public NPCMotion state;// { get; protected set; }
    protected BaseCharacterState status;
    protected Animator animator;
    protected SphereCollider vision;
    private bool isAlive;
    protected float RotationSpeed {
        get { return status.GetSecondaryAttrubute(SecondaryAttributeName.MoveSpeed).AdjustedValue * ROTATION_SPEED_MOVE_SPEED_RATIO; }
    }
    protected float MoveSpeed {
        get { return status.GetSecondaryAttrubute(SecondaryAttributeName.MoveSpeed).AdjustedValue * MOVE_SPEED_RATIO; }
    }
    internal float VisionRadious {
        get { return status.GetSecondaryAttrubute(SecondaryAttributeName.Vision).AdjustedValue / VISION_DEPTH_AGI_RATIO ; }
    }
    protected float VisionWidth {
        get { return status.GetPrimaryAttrubute(PrimaryAttributeName.Agility).AdjustedValue / VISION_WIDTH_AGI_RATIO; }
    }
    protected float CautionTime
    {
        get
        {
            return status.GetPrimaryAttrubute(PrimaryAttributeName.Spirit).AdjustedValue / CAUTION_TIME_SPIRIT_RATIO + CAUTION_TIME_BASE_VALUE;
        }
    }
    private float MinDistance {
        get {
            float _md = 0;
            if (IsRanger)
            {

                _md = controller.radius * 2 * RANGER_MIN_ATTACK_DISTANCE_BY_BODY_LANGTH;
                if (_md > vision.radius / 2)
                    _md = vision.radius / 2;
            }
            else
                _md = controller.radius+.01f;
            return _md;
        }
    }
    protected float SearchTime { get { return 2- status.GetPrimaryAttrubute(PrimaryAttributeName.Wisdom).AdjustedValue/ LENGTH_TO_SEARCH_WIS_RATIO; } }
    protected float _searchTimer;
    //public Vector3? SearchCenterPosition;
    protected float PatrolMaxDistance = 3f;
    protected float PatrolCharMaxDistance = .5f;

    protected float _cautionTimer = 0;
    protected float scanTimer = 0;
    
    protected float scanTimeStep = 1f;
    protected float scanAngle = 0;

    internal Dictionary<string, int> TargetTag;
        
    internal List<Transform> _targetsInVision;
    internal List<Transform> TargetsInVision { get
        {
            List<Transform> temp = new List<Transform>();
            for (int i=0;i<  _targetsInVision.Count;i++)
            {
                Transform tar = _targetsInVision[i];
                //排除已消滅的物件
                if (tar != null)
                {
                    if (tar.tag != Tags.DeadBody)
                        temp.Add(tar);
                    else if (tar == npc.attackTarget)
                    {
                        npc.AttackTargetDie(tar);
                    }

                }
            }
            _targetsInVision.Clear();
            _targetsInVision = temp;
            return _targetsInVision;
        }
    }
    
    public Transform mainTarget;
    public List<Transform> CommandTargets;
    bool isGuardPlayer;
    public Transform guardTarget;
    internal Transform GuardTarget
    {
        get { return guardTarget; }
        set
        {
            if (value!=null && value.GetComponent<Player>() != null) isGuardPlayer = true;
            guardTarget = value;
        }
    }
    
    protected NavMeshAgent navAgent;
    protected float minFallowWhenIdleDistance;
    internal bool IsRanger;
    protected bool IsGarrison = false;
    internal bool InGarrison;
    protected float garrisonRange = 3f;
    public Transform garrisonTarget;
    public Transform GarrisonTarget {
        get { return garrisonTarget; }
        set {
            IsGarrison = (value != null);
            if (!IsGarrison)
                InGarrison = false;
            garrisonTarget = value;
        }
    }
    ICharacterAnimation animationController;
    const float UPDATE_INTERVAL = .05f;
    const float ROTATION_UPDATE = 0.02f;
    protected virtual void Awake()
    {
        myTransform = transform;
        state = NPCMotion.Idle;
        isAlive = true;
        _targetsInVision = new List<Transform>();
        CommandTargets = new List<Transform>();
        //NPCType = Type;
        _searchTimer = 0;
        controller = GetComponent<CharacterController>();
        GameObject visionObject = new GameObject("Vision");
        visionObject.transform.parent = myTransform;
        visionObject.transform.position = myTransform.position;
        VisionCollision visionCollision = visionObject.AddComponent<VisionCollision>();
        visionCollision.onTriggerEnter += onTriggerEnter;
        visionCollision.onTriggerExit += onTriggerExit;
        vision = visionObject.AddComponent<SphereCollider>();
        vision.center = controller.center;
        vision.isTrigger = true;
        
    }
    public void SetStatus(BaseCharacterState state) {
        this.status = state;
    }

    public void SetAnimationController(ICharacterAnimation animationController)   {
        this.animationController = animationController;
    }
    // Use this for initialization
    protected virtual void Start()
    {
        npc = GetComponent<NPCController>();
        status = npc.status;
        npc.OnDead += Dead;
        navAgent = gameObject.GetComponent<NavMeshAgent>();
        //持續往某方向scan的時間
        scanTimeStep = 1f;

        //設定視野
        vision.radius = IsRanger ? VisionRadious * RANGER_VISION_DEPTH_FIX_RATIO : VisionRadious;        
        minFallowWhenIdleDistance = controller.radius * 2 * FALLOW_WHEN_IDLE_MIN_DISTANCE_BY_BODY_LANGTH;
        navAgent.speed = MoveSpeed;
        navAgent.angularSpeed = RotationSpeed;
        navAgent.acceleration = status.GetSecondaryAttrubute(SecondaryAttributeName.MoveSpeed).AdjustedValue / 50;
        InGarrison = false;
        StartCoroutine("FSM");
        InvokeRepeating("Rotate", 0, ROTATION_UPDATE);
    }
    IEnumerator FSM()
    {
        while (isAlive)
        {
            switch (state)
            {
                default:
                case NPCMotion.Deciding:
                    Deciding();
                    break;
                case NPCMotion.Idle:
                    Idle();
                    break;
                case NPCMotion.Search:
                    Search();
                    break;
                case NPCMotion.Scan:
                    Scan();
                    break;
                case NPCMotion.Move:
                    Move();
                    break;
                case NPCMotion.Fallow:
                    Follow();
                    break;
                case NPCMotion.Casting:
                    Casting();
                    break;
                case NPCMotion.Garrison:
                    Garrison();
                    break;
            }
            if (_cautionTimer != 0)
            {
                if (Time.time - _cautionTimer > CautionTime)
                {
                    _cautionTimer = 0;
                    vision.radius /= CAUTION_VISION_DEPTH_FIX_RATIO;
                }
            }
            vision.radius = IsRanger ? VisionRadious * RANGER_VISION_DEPTH_FIX_RATIO : VisionRadious;

            yield return new WaitForSeconds(UPDATE_INTERVAL);
        }
        npc.attackTarget = null;

        mainTarget = null;

    }
    
    float rotationAngle;
    void Rotate() {
        if (rotationAngle != 0)
        {
            float angle = 0;
            if (rotationAngle > RotationSpeed * ROTATION_UPDATE)
            {
                angle = RotationSpeed * ROTATION_UPDATE;
            }
            else if (rotationAngle > RotationSpeed * -ROTATION_UPDATE)
            {
                angle = rotationAngle;
            }
            else
            {
                angle = RotationSpeed * -ROTATION_UPDATE;
            }
            myTransform.Rotate(Vector3.up, angle);
            rotationAngle -= angle;
        }
    }

    //決策
    protected virtual void Deciding() {
        if (npc.IsCasting)
            state = NPCMotion.Casting;        
        else
        {
            CheckTarget();
            npc.attackTarget = null;
            //有目標
            if (mainTarget != null)
            {
                
                if (IsGarrison &&
                    Vector3.ProjectOnPlane(mainTarget.position - GarrisonTarget.position, Vector3.up).magnitude > garrisonRange)
                {
                    state = NPCMotion.Garrison;
                }
                else
                {
                    if (guardTarget != null)
                    {
                        //攻擊目標超出護位目標時往護位目標移動,追擊1.5被距離
                        if (Vector3.ProjectOnPlane(
                            (mainTarget.position - guardTarget.position), Vector3.up).magnitude 
                            > (isGuardPlayer ? PatrolCharMaxDistance : PatrolMaxDistance)*1.5F)
                        {
                            mainTarget = guardTarget;
                            state = NPCMotion.Move;
                            return;
                        }
                    }
                    bool mainTargetOutOfRange = false;
                    if (mainTarget.GetComponentInParent<Build>() == null)
                    {
                        //距離外的人              
                        mainTargetOutOfRange = !_targetsInVision.Contains(mainTarget);
                    }
                    bool mainTargetOutOfVision = Mathf.Abs(Vector3.Angle(    //視野外
                                    Vector3.ProjectOnPlane(
                                        mainTarget.position - myTransform.position, myTransform.up), myTransform.forward))
                                        > VisionWidth && (mainTarget.position - myTransform.position).magnitude >= MinDistance*2;
                    if (mainTargetOutOfRange)
                    {
                        //超出距離,靠過去
                        state = NPCMotion.Fallow;
                    }
                    else
                    {
                        //距離內
                        if (mainTargetOutOfVision)
                        {
                            //視線外,尋找
                            state = NPCMotion.Scan;
                        }
                        else
                        {
                            //視線內,跟上
                            state = NPCMotion.Fallow;
                            npc.attackTarget = mainTarget;
                        }
                    }
                }
            }
            else
            {
                if (IsGarrison)
                {
                    state = NPCMotion.Garrison;
                }
                else
                {
                    //沒有主要目標
                    //有護衛目標時
                    if (guardTarget != null)
                    {
                        //超出護位目標時往護位目標移動
                        if (Vector3.ProjectOnPlane((myTransform.position - guardTarget.position), Vector3.up).magnitude > (isGuardPlayer? PatrolCharMaxDistance: PatrolMaxDistance))
                        {
                            mainTarget = guardTarget;
                            state = NPCMotion.Move;
                        }
                        else
                        {
                            //靠近護位目標時護位待命

                            state = NPCMotion.Idle;
                        }
                    }
                    else
                    {
                        //無護位目標時待命
                        if (npc.commander != null)
                        {
                            //npc.commander.CommandFollowerInvade(npc);
                            if (npc.commander is Tower)
                            {
                                npc.Patrol((npc.commander as Tower).MainFlag);
                            }
                            else if (npc.commander is Player)
                            {
                                npc.Patrol((npc.commander as Player).myTransform);
                            }

                            state = NPCMotion.Deciding;
                        }
                        else
                        {
                            state = NPCMotion.Idle;
                        }
                    }
                }
            }
        }
    }
    //待命
    protected virtual void Idle()
    {
        if (animationController != null)
            animationController.PlayIdle();
        navAgent.destination = myTransform.position;
        
        //else
        //{
            if (!IsGarrison && mainTarget == null && !isGuardPlayer)
            {
                //索敵        
                float seed = Random.Range(0, CHANCE_TO_SEARCH_WIS_RATIO);
                //機率進入索敵狀態
                if (Time.time - _searchTimer > SearchTime + .1f && seed < status.GetPrimaryAttrubute(PrimaryAttributeName.Wisdom).AdjustedValue)
                {
                    _searchTimer = 0;
                    state = NPCMotion.Search;
                }
                else
                {
                    state = NPCMotion.Deciding;
                }
            }
            else
            {
                state = NPCMotion.Deciding;

            }
        //}
        
    }
    //索敵
    private Vector3? searchTo;
    //護衛目標附近徘徊,太遠會回來,無護位目標則遊蕩
    protected virtual void Search()
    {
        if (npc.IsCasting)
        {
            state = NPCMotion.Casting;
            return;
        }
        if (_searchTimer == 0)
        {
            if (guardTarget != null && Vector3.ProjectOnPlane((myTransform.position - guardTarget.position),Vector3.up).magnitude > PatrolMaxDistance)
            {               
                //Debug.Log(SearchCenterPosition);
                float angle = Vector3.Angle(
                    Vector3.ProjectOnPlane(myTransform.forward, Vector3.up), Vector3.ProjectOnPlane(guardTarget.position - myTransform.position, Vector3.up))*
                    (Vector3.Dot(myTransform.right, guardTarget.position - myTransform.position) > 0 ? 1 : -1);
                myTransform.Rotate(Vector3.up,angle);
                //rotationAngle = angle;
            }
            else
            {
                //隨機轉身 
                myTransform.Rotate(Vector3.up, Random.Range(0, 360));
                //rotationAngle = Random.Range(0, 360);
                
            }
            searchTo = myTransform.position + myTransform.forward * MoveSpeed * SearchTime;
            _searchTimer = Time.time;
        }
        else
        {
            if (Time.time - _searchTimer > SearchTime)
            {
                //索敵結束,重新判斷
                state = NPCMotion.Deciding;
                searchTo = null;
            }
            else
            {
                //索敵時間中往該方向前進                
                state = NPCMotion.Search;                
            }
        }
        if (searchTo != null)
        {
            navAgent.destination = (Vector3)searchTo;
            if (animationController != null)
                animationController.PlayWalk();
        }
    }
    //轉身讓視野進行範圍的掃描,原地停留
    protected virtual void Scan()
    {
        if (scanTimer == 0)
        {
            navAgent.destination = myTransform.position;
            scanTimer = Time.time;
            //決定這次scan的方向
            switch (Random.Range(0, 2))
            {
                case 0:
                    scanAngle = RotationSpeed * -1;
                    break;
                case 1:
                    scanAngle = RotationSpeed;
                    break;
                default:
                    break;
            }
        }
        else if (Time.time - scanTimer > scanTimeStep)
        {
            //搜索結束
            scanTimer = 0;
        }
        //轉身搜索
        rotationAngle = scanAngle;
        //myTransform.Rotate(Vector3.up, scanAngle* UPDATE_INTERVAL);
        if (animationController != null)
            animationController.PlayScan();
        state = NPCMotion.Deciding;
    }
    //網主要目標移動
    protected virtual void Move()
    {
        if (npc.IsCasting)
        {
            state = NPCMotion.Casting;
            return;
        }
        //Debug.Log(GetComponent<NPCController>().IsAttacking +" || "+ GetComponent<NPCController>().IsSkilling);
        if (npc.IsAttacking || npc.IsSkilling)
        {
            state = NPCMotion.Deciding;
            return;
        }

        navAgent.destination = mainTarget.position - mainTarget.forward*-1*MinDistance;
        if (animationController != null)
            animationController.PlayRun();
      
        state = NPCMotion.Deciding;
    }
    //施法中,原地停留
    protected virtual void Casting() {
        if (npc.IsCasting)
        {
            navAgent.destination = transform.position;
            state = NPCMotion.Casting;
            if (animationController != null)
                animationController.PlayCast();
        }
        else
            state = NPCMotion.Deciding;
    }
    //轉向主要目標,決定是否靠近
    protected virtual void Follow()
    {
        if (npc.IsCasting)
        {
            state = NPCMotion.Casting;
            return;
        }
        //有跟隨對象
        if (mainTarget)
        {
            //轉往目標方向
            //float angle = RotationSpeed * (Vector3.Dot(myTransform.right, mainTarget.position - myTransform.position) > 0 ? 1 : -1);
            float angle = Vector3.Angle(myTransform.forward, Vector3.ProjectOnPlane( mainTarget.position - myTransform.position,myTransform.up)) *(Vector3.Dot(myTransform.right, mainTarget.position - myTransform.position) > 0 ? 1 : -1);
            rotationAngle = angle;
            //myTransform.Rotate(Vector3.up, angle);
            //目標體寬
            var targetWidth = mainTarget.GetComponentInParent<Build>() != null ? mainTarget.GetComponent<MeshFilter>().mesh.bounds.size.x * mainTarget.transform.localScale.x : mainTarget.GetComponent<CharacterController>().radius;
            //是否已經進入視當的距離
            if (Vector3.Distance(mainTarget.position, myTransform.position) < MinDistance + targetWidth)
            {
                //已經進入視當的距離,停下來
                navAgent.destination = myTransform.position;
                //Debug.Log("Stop");
                state = NPCMotion.Idle;
            }
            else
            {
                //沒進入視當的距離,繼續移動
                state = NPCMotion.Move;
            }
        }
        else
        {
            //無跟隨對象,待機
            state = NPCMotion.Deciding;
        }
    }
    //定點守備
    protected virtual void Garrison() {
        if (garrisonTarget != null)
        {
            if (garrisonTarget.CompareTag(Tags.DeadBody))
            {
                GarrisonTarget = null;
                state = NPCMotion.Deciding;
            }
            else
            {
                InGarrison = Vector3.ProjectOnPlane(myTransform.position - GarrisonTarget.position, Vector3.up).magnitude < garrisonRange;

                if ((myTransform.position - garrisonTarget.position).magnitude < navAgent.stoppingDistance && navAgent.remainingDistance == 0)
                {
                    state = NPCMotion.Idle;
                }
                else
                {
                    mainTarget = garrisonTarget;
                    state = NPCMotion.Move;
                }
            }
        }
        else
        {
            state = NPCMotion.Deciding;
        }
    }
    protected virtual void onTriggerEnter(Collider other)
    {        
        if (other.GetComponent<Damagable>()!=null)
        {
            _targetsInVision.Add(other.transform);

            CheckTarget();
            //_targetInVision = other.transform;
            state = NPCMotion.Deciding;
        }
    }
    protected virtual void onTriggerExit(Collider other)
    {       
        if (_targetsInVision.Contains(other.transform))
        {
            _targetsInVision.Remove(other.transform);
            CheckTarget();
            state = NPCMotion.Deciding;
        }
        //}
    }
    protected virtual void CheckTarget()
    {
        List<Transform> temp = new List<Transform>();
        for (int i=0;i<_targetsInVision.Count;i++)
        {
            Transform tar = _targetsInVision[i];
            //排除已消滅的物件
            if (tar != null && !tar.CompareTag(Tags.DeadBody))
            {
                temp.Add(tar);
            }
            else if (tar == npc.attackTarget)
            {
                npc.AttackTargetDie(tar);
            }
        }
        _targetsInVision.Clear();
        _targetsInVision = temp;
        if (guardTarget != null && guardTarget.CompareTag(Tags.DeadBody))
            guardTarget = null;

        temp = new List<Transform>();
        for (int i = 0; i < CommandTargets.Count; i++)
        {
            Transform tar = CommandTargets[i];        
            //排除已消滅的物件,死亡物件,非敵對物件
            if (tar != null && !tar.CompareTag(Tags.DeadBody) && !Tags.IsCompanion(this,tar))
            {                
                temp.Add(tar);
            }
            else if (npc.attackTarget!=null && tar == npc.attackTarget)
            {
                npc.AttackTargetDie(tar);
            }
        }
        CommandTargets.Clear();
        CommandTargets = temp;

        //目標與正前方夾角 小到大
         _targetsInVision.Sort(
            (x,y) => 
                Vector3.Angle(
                    Vector3.ProjectOnPlane(myTransform.forward, Vector3.up),
                    Vector3.ProjectOnPlane(x.position - myTransform.position, Vector3.up)
                ).CompareTo(
                Vector3.Angle(
                    Vector3.ProjectOnPlane(myTransform.forward, Vector3.up),
                    Vector3.ProjectOnPlane(y.position - myTransform.position, Vector3.up)
                )));
        List<Transform> orderByAngle = _targetsInVision;
        mainTarget = null;
        //命令對象在視野中 
        //MainTarget: 每次回合判斷的攻擊對象
        // 1.嘲諷來源(2),無視太遠的嘲諷
        // 2.範圍內的命令目標
        foreach (Transform commandTarget in CommandTargets)
        {
            if (orderByAngle.Contains(commandTarget)) {
                mainTarget = commandTarget;
                return;
            }
        }
        if (!IsRanger) {
            //進戰找最近
            if (_targetsInVision.Count > 0)
            {
                _targetsInVision.Sort(TargetSorter);
                //進戰找最近的
                foreach (Transform t in _targetsInVision)
                {
                    if (TargetTag.ContainsKey(t.tag))
                    {
                        mainTarget = t;
                        return;
                    }
                }
            }
        }
        else {
            //遠程找視野內血最少
            bool overVision = false;
            for (int i=0;i< orderByAngle.Count;i++)
            {
                Transform t = orderByAngle[i];

                if (TargetTag.ContainsKey(t.tag)  //可攻擊
                    && (mainTarget == null //沒目標
                        || t.GetComponent<BaseCharacterBehavior>().status  //血比目前目標少
                                .GetConsumedAttrubute(ConsumedAttributeName.Health).CurValue
                            < mainTarget.GetComponent<BaseCharacterBehavior>().status
                                .GetConsumedAttrubute(ConsumedAttributeName.Health).CurValue))
                {
                    //是否開始超出視野
                    if (!overVision && overVision != Mathf.Abs(Vector3.Angle(    //視野內
                            Vector3.ProjectOnPlane(
                                t.position - myTransform.position, myTransform.up), myTransform.forward))
                                > VisionWidth)
                    {
                        overVision = !overVision;
                        // 3.2.1.遠程範圍(視野)內的血最少目標
                        if (mainTarget != null)
                            return;
                    }
                    // 3.2.1.遠程範圍(視野)內的血最少目標
                    // 3.2.2遠程範圍(全)內的血最少目標
                    mainTarget = t;
                }                       
            }
        }

        if (CommandTargets.Count > 0)
            mainTarget = CommandTargets[0];;
    }
    public void Dead(BaseCharacterBehavior character)
    {
        isAlive = false;
        controller.enabled = false;
    }
    public void GetDamage()
    {
        if (_cautionTimer == 0)
        {
            _cautionTimer = Time.time;
            vision.radius *= CAUTION_VISION_DEPTH_FIX_RATIO;
        }
        //if (mainTarget != _nearestTargetInVision)
        //{
        //    mainTarget = _nearestTargetInVision;
        //    state = NPCMotion.Fallow;
        //}
    }
    
    protected virtual int TargetSorter(Transform a, Transform b) {
        // -1 A,B , A=B 0 , 1 B,A 小的前面
        //a不合法>合法 , a合法>b不合法, 其餘往下
        if (!TargetTag.ContainsKey(a.tag) && TargetTag.ContainsKey(b.tag))
            return 1;
        else if (TargetTag.ContainsKey(a.tag) && !TargetTag.ContainsKey(b.tag))
            return -1;
        //a, b 合法,排序
        else if (TargetTag.ContainsKey(a.tag) && TargetTag.ContainsKey(b.tag))
        {
            //照tag的value順序先, 大到小
            int tagSort = TargetTag[a.tag].CompareTo(TargetTag[b.tag]) * -1;
            if (tagSort != 0)
                return tagSort;
        }
        //同Tag用距離排序 近->遠, 遠程先打遠
        return Vector3.Distance(a.position, myTransform.position)
                    .CompareTo(Vector3.Distance(b.position, myTransform.position)) * (IsRanger ? -1 : 1);
        
    }
}


