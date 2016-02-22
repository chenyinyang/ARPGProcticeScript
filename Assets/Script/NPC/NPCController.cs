using UnityEngine;
using System.Collections.Generic;


[RequireComponent(typeof(BaseNPCMovementAI))]
[AddComponentMenu("Game/NPCController")]
public class NPCController : BaseCharacterBehavior {
    public NPCJobs job;
    public NPCType NPCType;
    
    const float deadbodyDisapearTime = 5f;
    float deadbodyDisapearTimer = 0f;
    //gui
    GameObject blood;
    float bloodBarOffsetRatio = 1.1f;
    UISlider bloodSlider;
    UILabel bloodText;
    UISlider manaSlider;
    UILabel manaText;
    UISlider energySlider;
    UILabel energyText;
    UILabel levelLabel;
    Transform buffList;

    public bool addNewBuffFlag = false;    
    protected CastingBar CastingBar;
    //AI Control
    public ICommander commander;
    public Transform attackTarget;
    BaseNPCMovementAI movementAI;
    public delegate void OnAttackTargetDieHandler(NPCController attacker, Transform target);
    public event OnAttackTargetDieHandler OnAttackTargetDie;
    public void AttackTargetDie(Transform target) {
        if (commander != null)
            commander.CommandFollowerInvade(this);
        if (OnAttackTargetDie != null) {
            OnAttackTargetDie(this, target);
        }
    }
    protected NavMeshAgent navAgent;

    protected override void Awake() {
        movementAI = GetComponent<BaseNPCMovementAI>();
        switch (job)
        {
            case NPCJobs.Warrior:
                status = new SwordMasterCharacterState(CharacterName);
                movementAI.IsRanger =false;
                gameObject.AddComponent<AttackAI>();
                break;
            case NPCJobs.Archer:
                status = new ArcherCharacterState(CharacterName);
                movementAI.IsRanger = true;
                gameObject.AddComponent<RangeAttackAI>();
                break;
            case NPCJobs.Mage:
                status = new WizzardCharacterState(CharacterName);
                movementAI.IsRanger = true;
                gameObject.AddComponent<RangeAttackAI>();
                break;
            case NPCJobs.Magician:
                status = new MagicianCharacterState(CharacterName);
                movementAI.IsRanger = true;
                gameObject.AddComponent<RangeAttackAI>();
                break;
            case NPCJobs.Knight:
                status = new KnightCharacterState(CharacterName);
                movementAI.IsRanger = false;
                gameObject.AddComponent<AttackAI>();
                break;
            case NPCJobs.Priest:
                status = new PriestCharacterState(CharacterName);
                movementAI.IsRanger = false;
                gameObject.AddComponent<AttackAI>();
                break;
            case NPCJobs.Monster:
                status = new MonsterCharacterState(CharacterName);
                if(movementAI.IsRanger)
                    gameObject.AddComponent<RangeAttackAI>();
                else
                    gameObject.AddComponent<AttackAI>();
                break;
            default:
                break;
        }
        movementAI.NPCType = NPCType;
        //gameObject.AddComponent<NPCMotion>().model = model;
        movementAI.SetStatus(status);
        base.Awake();
        MakeBloodBar();

        //eargleEyeImgTransform.GetComponent<MeshRenderer>().materials[0].mainTexture = ResourceLoader.NPC.GetFaceicon(job);

        navAgent = gameObject.AddComponent<NavMeshAgent>();
        navAgent.height = BodyHeight;
        navAgent.radius = BodyWidth / 2;
        navAgent.baseOffset = 0.01f;
        navAgent.stoppingDistance = BodyWidth;
        navAgent.autoRepath = true;
        navAgent.autoBraking = true;


    }
    protected override void Start()
    {
        base.Start();
        InfluenceBar.charObjs.Add(this);
    }
    public void SetNPCType(NPCType type) {
        this.NPCType = type;
        movementAI.NPCType = NPCType;
        Color npcColor = NPCType == NPCType.Enemy ?
                new Color(1, .5f, .5f) :
                    NPCType == NPCType.Friend ? new Color(.5f, .5f, 1f) :
                    new Color(1, 1, 1);

        bloodSlider.transform.Find("Foreground").GetComponent<UISprite>().color = NPCType == NPCType.Friend ? Color.green : (NPCType == NPCType.Enemy ? Color.red : Color.white);
        model.GetComponentInChildren<SkinnedMeshRenderer>().materials[0].shader = Shader.Find("Diffuse");
        model.GetComponentInChildren<SkinnedMeshRenderer>().materials[0].color = npcColor;
        eargleEyeImgTransform.GetComponent<MeshRenderer>().materials[0].color = npcColor;
    }

    private void MakeBloodBar() {
        GameObject bloodPoint = new GameObject("Blood Point");
        bloodPoint.transform.parent = myTransform;
        bloodPoint.transform.position = new Vector3(myTransform.position.x, myTransform.position.y + BodyHeight * bloodBarOffsetRatio, myTransform.position.z);
        GameObject goPref = Resources.Load("MonsterBlood") as GameObject;
        blood = Instantiate(goPref, bloodPoint.transform.position, Quaternion.identity) as GameObject;
        blood.name = CharacterName + "_Blood bar";
        blood.transform.parent = GameObject.Find("UI Root").transform;
        blood.transform.localScale = Vector3.one * 1.5f;
        blood.layer = 5;
        UIFollowTarget followScript = blood.GetComponent<UIFollowTarget>();
        followScript.gameCamera = Camera.main;
        followScript.uiCamera = GameObject.Find("UICamera").GetComponent<Camera>();
        followScript.target = bloodPoint.transform;        
        blood.transform.Find("Name").GetComponentInChildren<UILabel>().text = CharacterName;
        bloodSlider = blood.transform.Find("HealthBar").GetComponentInChildren<UISlider>();
        bloodSlider.transform.Find("Foreground").GetComponent<UISprite>().color = NPCType== NPCType.Friend? Color.green: (NPCType == NPCType.Enemy?Color.red:Color.white);
        bloodText = blood.transform.Find("HealthBar").GetComponentInChildren<UILabel>();
        manaSlider = blood.transform.Find("ManaBar").GetComponentInChildren<UISlider>();
        manaText = blood.transform.Find("ManaBar").GetComponentInChildren<UILabel>();
        energySlider = blood.transform.Find("EnergyBar").GetComponentInChildren<UISlider>();
        energyText = blood.transform.Find("EnergyBar").GetComponentInChildren<UILabel>();
        levelLabel = blood.transform.Find("Level").GetComponent<UILabel>();
        CastingBar = blood.transform.Find("CastingBar").GetComponent<CastingBar>();
        CastingBar.gameObject.SetActive(false);

        buffList = blood.transform.Find("BuffList");
    }
    protected override void UpdateStatistic()
    {
        base.UpdateStatistic();
        UpdateBloodUISize();
        if (blood.activeSelf)
        {
            levelLabel.text = "Lv." + status.Level;
            UpdateBloodSlider();
            UpdateManaSlider();
            UpdateEnergySlider();
            Transform buffList = blood.transform.Find("BuffList");
            int buffIconCount = buffList.childCount;
            //buff數量與icon不一致的時候重繪
            if (blood.GetComponent<UIFollowTarget>().isVisible && buffIconCount != buffs.Count || addNewBuffFlag)
            {
                ReDrawBuff();
                addNewBuffFlag = false;
            }
        }
    }
  
    // Update is called once per frame
   

    private void UpdateBloodUISize() {
        float dis = Vector3.ProjectOnPlane((myTransform.position - MyGUI.Instence.MainCamera.position), Vector3.up).magnitude;
       
        //距離很近
        if (dis < 3)
        {
            //ShowFullUI
            if (!blood.activeSelf)
            {
                blood.SetActive(true);
                
            }
            SetSimpleUI(false);

        }
        //距離中等
        else if (dis < 6)
        {
            if (!blood.activeSelf)
                blood.SetActive(true);
            SetSimpleUI(true);
            blood.transform.localScale = Vector3.one * 1.5f * (1 / (dis-2));
            //ShowSinpleUI,Size<<
        }
        else
        {
            //HideUI
            if (blood.activeSelf)
            {
                blood.SetActive(false);
                SetSimpleUI(true);
            }
        }
    }
    bool isSimpleUI;
    private void SetSimpleUI(bool switchOn) {
        if (isSimpleUI != switchOn)
        {            
            manaSlider.gameObject.SetActive(!switchOn);
            energySlider.gameObject.SetActive(!switchOn);
            CastingBar.gameObject.SetActive(!switchOn);
            blood.transform.Find("BuffList").gameObject.SetActive(!switchOn);
            isSimpleUI = switchOn;
        }
    }
    public void ReDrawBuff() {
        //移除所有
        buffList.DestroyChildren();
        if (buffList.gameObject.activeSelf == false)
            return;
        int iconDrawIndex = 0;
        foreach (BuffSkill b in buffs)
        {
            //計算位置        
            Vector3 newBuffIconPos = new Vector3(
                    (iconDrawIndex % 5) * 20 - 40,
                    (iconDrawIndex / 5) * -20);
            GameObject buffIconPrefab = ResourceLoader.UIPrefabs.BuffIcon();

            GameObject buffIcon = NGUITools.AddChild(buffList.gameObject, buffIconPrefab);

            //GameObject buffIcon = GameObject.Instantiate(buffIconPrefab, newBuffIconPos, buffList.transform.rotation) as GameObject;
            buffIcon.name = b.skillName.ToString();
            buffIcon.transform.localPosition = newBuffIconPos;
            //buffIcon.transform.parent = buffList.transform;
            buffIcon.transform.localScale = new Vector3(.6f, .6f, .6f);

            buffIcon.GetComponentInChildren<UITexture>().mainTexture = b.BuffIcon;
            if (b.Stackable)
            {                
                buffIcon.transform.Find("LayerCount").gameObject.SetActive(true);
                buffIcon.transform.Find("LayerCount").GetComponent<UILabel>().text = b.stackLayer.ToString();
            }
            else
            {
                buffIcon.transform.Find("LayerCount").gameObject.SetActive(false);
            }
            iconDrawIndex++;
        }
    }
    void UpdateBloodSlider() {
        float curValue = status.GetConsumedAttrubute(ConsumedAttributeName.Health).CurValue;
        float maxValue = status.GetConsumedAttrubute(ConsumedAttributeName.Health).AdjustedValue ;        
        bloodText.text = curValue.ToString("0") + "/" + maxValue;
        bloodSlider.value = curValue / maxValue;
    }
    void UpdateManaSlider()
    {
        float curValue = status.GetConsumedAttrubute(ConsumedAttributeName.Mana).CurValue;
        float maxValue = status.GetConsumedAttrubute(ConsumedAttributeName.Mana).AdjustedValue;

        manaText.text = (curValue * 100 / maxValue).ToString("0") + "%";
        manaSlider.value = curValue / maxValue;
    }
    void UpdateEnergySlider()
    {
        float curValue = status.GetConsumedAttrubute(ConsumedAttributeName.Energy).CurValue;
        float maxValue = status.GetConsumedAttrubute(ConsumedAttributeName.Energy).AdjustedValue;

        energyText.text = (curValue*100 / maxValue).ToString("0")+"%";
        energySlider.value = curValue / maxValue;
    }
    
    public void AttackTo(Transform target)
    {
        if (target.GetComponent<Build>()!=null) {
            List<Transform> temp = new List<Transform>();
            for (int i = 0; i < movementAI.CommandTargets.Count; i++)
            {
                if (movementAI.CommandTargets[i].GetComponent<Build>() == null)
                    temp.Add(movementAI.CommandTargets[i]);
            }
            movementAI.CommandTargets = temp;
        }
        if(!movementAI.CommandTargets.Contains(target))
            movementAI.CommandTargets.Insert(0,target);
    }
    public void RemoveAttackTo(Transform target)
    {
        if (movementAI.CommandTargets.Contains(target))
            movementAI.CommandTargets.Remove(target);
    }
    public void ClearAllCommand() {
        movementAI.CommandTargets.Clear();
    }
    public void Patrol(Transform target) {
        movementAI.GuardTarget = target;
    }
    public bool IsInGarrison { get { return movementAI.InGarrison; } }
    public void Garrison(Transform target)
    {
        movementAI.GarrisonTarget = target;
    }
    
    public override bool CanDamageTarget(Damagable target)
    {
        if (IsDead)
            return false;
        return movementAI.TargetTag.ContainsKey(target.tag);
    }

    public override void Casting(SkillName name, float time, CastOver callBack)
    {
        base.Casting(name, time, callBack);
        if (!isSimpleUI)
        {
            CastingBar.gameObject.SetActive(true);
            CastingBar.Casting(name, time, true);
        }
    }
    public override void CastingCancel()
    {
        base.CastingCancel();
        CastingBar.gameObject.SetActive(false);
    }
    protected override void Dead()
    {
        base.Dead();
        bloodSlider.alpha = 0;
        manaSlider.alpha = 0;
        energySlider.alpha = 0;
        if (commander != null)
            commander.Followers.Remove(this);
        
        model.GetComponentInChildren<SkinnedMeshRenderer>().materials[0].shader = Shader.Find("Transparent/Diffuse");
        navAgent.enabled = false;
        GetComponent<CharacterController>().enabled = false;
        InvokeRepeating("DeadbodyDiapear", 0, 0.05f);
        Invoke("DestroySelf", deadbodyDisapearTime);
        InfluenceBar.charObjs.Remove(this);
    }
    void DeadbodyDiapear()
    {
        Color c = model.GetComponentInChildren<SkinnedMeshRenderer>().materials[0].color;
        c.a -= 0.05f / deadbodyDisapearTime;
        model.GetComponentInChildren<SkinnedMeshRenderer>().materials[0].color = c;
    }
    void DestroySelf()
    {
        CancelInvoke("DeadbodyDiapear");
        Destroy(gameObject);
    }

    public override void AddBuff(BuffSkill buff)
    {
        base.AddBuff(buff);
        addNewBuffFlag = true;
    }

    protected override void LevelUp()
    {
        switch (job)
        {
            case NPCJobs.Warrior:
                status.GetPrimaryAttrubute(PrimaryAttributeName.Power).baseValue += 1;
                status.GetPrimaryAttrubute(PrimaryAttributeName.Constitution).baseValue += 1;
                for (int i = 0; i < 4; i++)
                {
                    status.GetPrimaryAttrubute((PrimaryAttributeName)UnityEngine.Random.Range(0,6)).baseValue += 1;
                }
                break;
            case NPCJobs.Archer:
                status.GetPrimaryAttrubute(PrimaryAttributeName.Agility).baseValue += 2;
                for (int i = 0; i < 4; i++)
                {
                    status.GetPrimaryAttrubute((PrimaryAttributeName)UnityEngine.Random.Range(0, 6)).baseValue += 1;
                }
                break;
            case NPCJobs.Mage:
                status.GetPrimaryAttrubute(PrimaryAttributeName.Wisdom).baseValue += 1;
                status.GetPrimaryAttrubute(PrimaryAttributeName.Spirit).baseValue += 1;
                for (int i = 0; i < 4; i++)
                {
                    status.GetPrimaryAttrubute((PrimaryAttributeName)UnityEngine.Random.Range(0, 6)).baseValue += 1;
                }
                break;
            case NPCJobs.Monster:                
                for (int i = 0; i < 6; i++)
                {
                    status.GetPrimaryAttrubute((PrimaryAttributeName)UnityEngine.Random.Range(0, 6)).baseValue += 1;
                }
                break;
            default:
                break;
        }
        for (int i = 0; i < (int)SecondaryAttributeName.Count; i++)
        {
            status.GetSecondaryAttrubute((SecondaryAttributeName)i).baseValue += 5;
        }

        for (int i = 0; i < (int)ConsumedAttributeName.Count; i++)
        {
            status.GetConsumedAttrubute((ConsumedAttributeName)i).baseValue += 50;
            status.GetConsumedAttrubute((ConsumedAttributeName)i).CurValue += status.GetConsumedAttrubute((ConsumedAttributeName)i).AdjustedValue * .1f;
        }
    }
}
