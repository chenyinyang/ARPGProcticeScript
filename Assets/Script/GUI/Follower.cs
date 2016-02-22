using UnityEngine;
using System.Linq;

public class Follower : MonoBehaviour {
    const string Garrison = "Garrison";
    const string Leave = "Leave";
    
    
    Transform myTransform;
    NPCController npcBind;
    Tower towerBind;
    UITexture face;
    UILabel level;
    UILabel npcName;
    UISlider healthBar;
    UISlider expBar;
    UILabel power;
    UILabel agi;
    UILabel wis;
    UILabel btnLabel;
    const float UPDATE_INTERVAL = .1f;
    float updateTimer = 0;

    void Awake() {
        myTransform = transform;
        face = myTransform.Find("Face").GetComponent<UITexture>();
        level = myTransform.Find("Level").GetComponent<UILabel>();
        npcName = myTransform.Find("Name").GetComponent<UILabel>();
        healthBar = myTransform.Find("HealthBar").GetComponent<UISlider>();
        expBar = myTransform.Find("ExpBar").GetComponent<UISlider>();
        power = myTransform.Find("Power").Find("value").GetComponent<UILabel>();
        agi = myTransform.Find("Agi").Find("value").GetComponent<UILabel>();
        wis = myTransform.Find("Wis").Find("value").GetComponent<UILabel>();
        if(myTransform.Find("Station")!=null)
            btnLabel = myTransform.Find("Station").Find("Name").GetComponent<UILabel>();
    }
	
	
	// Update is called once per frame
	void Update () {
        if (Time.time - updateTimer > UPDATE_INTERVAL)
        {
            updateTimer = Time.time;
            if (npcBind != null)
            {
                healthBar.value = npcBind.status.GetConsumedAttrubute(ConsumedAttributeName.Health).CurValue / npcBind.status.GetConsumedAttrubute(ConsumedAttributeName.Health).AdjustedValue;
                healthBar.GetComponentInChildren<UILabel>().text =
                npcBind.status.GetConsumedAttrubute(ConsumedAttributeName.Health).CurValue.ToString("0") + "/" + npcBind.status.GetConsumedAttrubute(ConsumedAttributeName.Health).AdjustedValue.ToString("0");
                float expRatio = npcBind.status.Exp / npcBind.status.ExpToLevel;
                expBar.value = expRatio;
                expBar.GetComponentInChildren<UILabel>().text = (expRatio * 100).ToString("0") + "%";
                level.text = "Lv." + npcBind.status.Level.ToString();
                power.text = npcBind.status.GetPrimaryAttrubute(PrimaryAttributeName.Power).AdjustedValue.ToString("0");
                agi.text = npcBind.status.GetPrimaryAttrubute(PrimaryAttributeName.Agility).AdjustedValue.ToString("0");
                wis.text = npcBind.status.GetPrimaryAttrubute(PrimaryAttributeName.Wisdom).AdjustedValue.ToString("0");
            }
        }
	}
    public void BindNPC(NPCController npc, Tower tower) {
        npcBind = npc;
        towerBind = tower;
        face.mainTexture = ResourceLoader.NPC.GetFaceicon(npcBind.job);
        npcName.text = npcBind.CharacterName;
        if (tower != null)
        {
            if (tower.stationNPC.Contains(npc))
                btnLabel.text = Leave;
            else
                btnLabel.text = Garrison;
        }
    }
    public void OnStationButtonClick() {        
        if (btnLabel.text == Garrison && towerBind.AssignNPCStay(npcBind))
        {
            btnLabel.text = Leave;
        }
        else {
            if(towerBind.CancelNPCStay(npcBind))
                btnLabel.text = Garrison;
        }
    }
    public void OnGuardButtonClick()
    {
        if(Player.Instance.Followers.Count<3)
            towerBind.ChangeFollower(Player.Instance, npcBind);

    }
}
