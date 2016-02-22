using UnityEngine;
using System.Collections.Generic;

public class InfluenceBar : MonoBehaviour {
    public static List<Build> buildObjs;
    public static List<NPCController> charObjs;
    UISlider friendBar;
    UISlider enemyBar;
    UILabel friendInf;
    UILabel monsterInf;
    UILabel enemyInf;
    void Awake() {
        buildObjs = new List<Build>();
        charObjs = new List<NPCController>();
        
        friendBar = GetComponents<UISlider>()[0];
        enemyBar = GetComponents<UISlider>()[1];

        friendInf = transform.Find("FriendInf").GetComponent<UILabel>();
        monsterInf = transform.Find("MonsterInf").GetComponent<UILabel>();
        enemyInf = transform.Find("EnemyInf").GetComponent<UILabel>();
    }
	// Use this for initialization
	void Start () {
        InvokeRepeating("UpdateInfluenceBar", 0, .1f);
	}
    void UpdateInfluenceBar() {
        
        float friendHP=0;
        float monsterHP = 0;
        float enemyHP = 0;
        lock (buildObjs)
        {
            for (int i = 0; i < buildObjs.Count; i++)
            {
                switch (buildObjs[i].type)
                {
                    case NPCType.Enemy:
                        enemyHP += buildObjs[i].hp.CurValue;
                        break;
                    case NPCType.Friend:
                        friendHP += buildObjs[i].hp.CurValue;
                        break;
                    case NPCType.Neutral:
                        monsterHP += buildObjs[i].hp.CurValue;
                        break;
                }
            }
        }
        lock (charObjs)
        {
            for (int i = 0; i < charObjs.Count; i++)
            {
                switch (charObjs[i].NPCType)
                {
                    case NPCType.Enemy:
                        enemyHP += charObjs[i].status.GetConsumedAttrubute(ConsumedAttributeName.Health).CurValue;
                        break;
                    case NPCType.Friend:
                        friendHP += charObjs[i].status.GetConsumedAttrubute(ConsumedAttributeName.Health).CurValue;
                        break;
                    case NPCType.Neutral:
                        monsterHP += charObjs[i].status.GetConsumedAttrubute(ConsumedAttributeName.Health).CurValue;
                        break;
                }
            }
        }
        float totalHP = enemyHP + monsterHP + friendHP;
        friendBar.value = friendHP / totalHP;
        enemyBar.value = enemyHP / totalHP;
        friendInf.text = friendHP.ToString("0");
        if (monsterHP == 0)
            monsterInf.gameObject.SetActive(false);
        else
            monsterInf.text = monsterHP.ToString("0");
        enemyInf.text = enemyHP.ToString("0");        
        monsterInf.transform.localPosition = new Vector3(-150+(300* (monsterHP / 2 + friendHP)/totalHP), -20, 0);
    }
	
}
