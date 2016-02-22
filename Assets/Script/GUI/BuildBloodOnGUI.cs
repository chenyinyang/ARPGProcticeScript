using UnityEngine;
using System.Collections;

public class BuildBloodOnGUI : MonoBehaviour
{
    public Build build;
    protected UILabel towerName;
    protected UISlider hp;
    protected UILabel hpText;
    protected UISlider occupied;
    protected UILabel breadLabel;
    protected NPCController[] stays;
    protected UISlider[] staysHp;
    protected UIPanel stayPanel;
    protected Transform towerSet;
    protected Transform myTranform;
    HUDText damageText;
    float damageCollectTimer = 0f;    
    float preHealth;

    const float UPDATE_INTERVAL = 0.1f;
    float updateTimer = 0;
    protected virtual void onUpdate() {
        UpdateHp(build.hp.CurValue, build.hp.AdjustedValue);
        if (build is Tower)
        {
            Tower t = build as Tower;
            UpdateOccupied(t.OccupiedValue);
            UpdateBread(t.BreadCount);
            UpdateStayNPC(t);
        }
    }
    protected virtual void Awake() {
        myTranform = transform;
        towerName = myTranform.Find("Name").GetComponent<UILabel>();
        hp = myTranform.Find("HealthBar").GetComponent<UISlider>();
        hpText = hp.GetComponentInChildren<UILabel>();
        towerSet = myTranform.Find("TowerSets");
        occupied = towerSet.Find("OccupiedBar").GetComponent<UISlider>();
        breadLabel = towerSet.Find("BreadCount").GetComponent<UILabel>();
        stays = new NPCController[3];
        staysHp = new UISlider[3];
        stayPanel = towerSet.Find("StayPanel").GetComponent<UIPanel>();
        damageText = GetComponentInChildren<HUDText>();
    }

    public virtual void InitialWithTarget(Build build)
    {
        this.build = build;
        towerName.text = this.build.BuildName;        
        towerSet.gameObject.SetActive(this.build.IsTower);
        damageCollectTimer = Time.time;
        preHealth = this.build.hp.CurValue;
    }

    protected virtual void Update()
    {
        if (Time.time - updateTimer > UPDATE_INTERVAL) {
            onUpdate();
            updateTimer = Time.time;
        }        
        if (damageText!=null && Time.time - damageCollectTimer > .5f) {
            float hpLoss = build.hp.CurValue - preHealth;
            if (hpLoss != 0)
            {
                damageText.Add(hpLoss.ToString("0"), hpLoss > 0 ? Color.green : Color.red, 0);
                preHealth = build.hp.CurValue;
                damageCollectTimer = Time.time;
            }
        }
    }

    protected void UpdateHp(float current, float max)
    {
        hp.value = current / max;
        hpText.text = current.ToString("0");
       
    }
    protected void UpdateOccupied(float value)
    {        
        occupied.value = value;
    }
    protected void UpdateBread(int breadCount)
    {
        breadLabel.text = "x" + breadCount;
    }
    protected void UpdateStayNPC(Tower t) {
       
        for (int i = 0; i < t.stationNPC.Length; i++)
        {
            if (t.stationNPC[i] != null)
            {
                if (t.stationNPC[i] != stays[i])
                {
                    stays[i] = t.stationNPC[i];
                    if (staysHp[i] != null)
                    {
                        GameObject.Destroy(staysHp[i].transform.parent.gameObject);
                        //Debug.Log(staysHp[i].transform.parent.gameObject);
                    }
                    GameObject stayPrefab = ResourceLoader.UIPrefabs.GetStayPrefabd(stays[i].job);
                    Transform stayNpc = NGUITools.AddChild(stayPanel.gameObject, stayPrefab).transform;
                    stayNpc.localPosition = new Vector3(12 * i, 0, 0);
                    staysHp[i] = stayNpc.Find("HealthBar").GetComponent<UISlider>();
                }
                staysHp[i].value = stays[i].status.GetConsumedAttrubute(ConsumedAttributeName.Health).CurValue / stays[i].status.GetConsumedAttrubute(ConsumedAttributeName.Health).AdjustedValue;
            }
            else {
                if (staysHp[i] != null)
                {
                    GameObject.Destroy(staysHp[i].transform.parent.gameObject);
                }
            }

        }
    }

    
}
