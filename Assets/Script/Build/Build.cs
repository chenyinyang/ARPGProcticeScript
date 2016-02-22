using UnityEngine;
[RequireComponent(typeof(Damagable))]
public class Build : MonoBehaviour
{
    public NPCType type;
    public delegate void OnBuildDestroidHandler(Build build);
    public event OnBuildDestroidHandler OnBuildDestroied;
    public ConsumedAttribute hp;
    internal bool IsDestroied;
    BuildBlood buildBlood;
    public string BuildName;
    protected int MaxHealth = 10000;
    protected float DamageReduce = .9f;
    internal HurtText ht;
    public bool IsTower = false;
    protected Transform myTransform;
    protected Vector3 buildPosition;
    private GameObject destroyFire;

    protected float updateInterval = .1f;
    protected virtual void Awake()
    {
        myTransform = transform;
        buildPosition = transform.position;
        hp = new ConsumedAttribute(ConsumedAttributeName.Health, MaxHealth, 0, 0, 50, new AttributeModifier(), MaxHealth);
        GetComponent<Damagable>().damageHandler += Build_damageHandler;
        this.tag = Tags.Build;
        GameObject go;
        var hudTextPoint = myTransform.Find("BuffHUDTextPoint");
        if (hudTextPoint != null)
        {
            go = hudTextPoint.gameObject;
        }
        else
        {
            go = new GameObject("BuffHUDTextPoint");
            go.transform.parent = myTransform;
            go.transform.position = myTransform.position
                + new Vector3(0, .1f, 0);
            hudTextPoint = go.transform;
        }
        ht = (GameObject.Instantiate(
                            ResourceLoader.UIPrefabs.GetHurtText(),
                            go.transform.position, Quaternion.identity) as GameObject)
                        .GetComponent<HurtText>();

        ht.InitailHUDText(hudTextPoint);

        var bloodPoint = myTransform.Find("BloodPoint");
        if (bloodPoint != null)
        {
            go = bloodPoint.gameObject;
        }
        else
        {

            go = new GameObject("BloodPoint");
            go.transform.parent = myTransform;
            go.transform.position = myTransform.position
                + new Vector3(0, .5f, 0);
        }
        GameObject bb = GameObject.Instantiate(ResourceLoader.UIPrefabs.GetBuildBlood(), go.transform.position, Quaternion.identity) as GameObject;
        buildBlood = bb.GetComponent<BuildBlood>();
        buildBlood.InitialWithTarget(go.transform, this);
        if (myTransform.Find("DestroyFire") != null)
            destroyFire = myTransform.Find("DestroyFire").gameObject;
        SetNPCType(type);
    }
    protected virtual void Start()
    {
        InfluenceBar.buildObjs.Add(this);
        InvokeRepeating("onUpdate", 0, updateInterval);
    }
    protected virtual void onUpdate()
    {
        if (IsDestroied)
            CancelInvoke("onUpdate");

        if (hp.CurValue <= 0)
        {
            IsDestroied = true;
            this.enabled = false;
            if (OnBuildDestroied != null)
                OnBuildDestroied(this);
            BuildingDestroy();
        }
        hp.CurValue++;
    }


    protected virtual void Build_damageHandler(DamageType type, IDamageMaker damageMaker, BaseCharacterBehavior source)
    {
        bool isCritical;
        float addHit;
        float dmg = damageMaker.GetCurDamage(type, out isCritical, out addHit) * (1 - DamageReduce);
        hp.CurValue -= dmg;
        ht.SetText((int)dmg * -1, isCritical, false);
        source.CurrentMakeDamage += dmg;
    }
    protected virtual void BuildingDestroy()
    {
        tag = Tags.DeadBody;
        if(destroyFire!=null)
            destroyFire.SetActive(true);
        myTransform.GetComponent<Damagable>().enabled = false;
        InfluenceBar.buildObjs.Remove(this);
    }
    private void SetNPCType(NPCType type)
    {
        switch (type)
        {
            case NPCType.Enemy:
                this.tag = Tags.Enemy;
                break;
            case NPCType.Friend:
                this.tag = Tags.Friend;
                break;
            case NPCType.Neutral:
            default:
                this.tag = Tags.Build;
                break;
        }
    }
}
