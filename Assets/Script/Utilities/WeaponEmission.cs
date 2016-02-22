using UnityEngine;
using System.Collections;
using System;
[AddComponentMenu("Game/Weapon/RangeEmission")]
public class WeaponEmission : MonoBehaviour {
    public Transform _user;
    private BaseCharacterBehavior userChar;
    public GameObject boltPrefab;
    private Vector3 lastTargetPos;
    private float lastFireTime;
    Transform myTransform;
	// Use this for initialization
	void Start () {
        myTransform = transform;
        userChar = _user.GetComponent<BaseCharacterBehavior>();
        userChar.onAttackStart += WeaponCollider_onAttackStart;
        userChar.onAttackStop += WeaponCollider_onAttackStop;
    }

    private void WeaponCollider_onAttackStop()
    {       
    }

    private void WeaponCollider_onAttackStart()
    {   
        Fire();
    }
    
    public void Fire() {
        
        bool isCri;
        float addHit;

        GameObject go = GameObject.Instantiate(boltPrefab, myTransform.position, _user.rotation) as GameObject;
        WeaponBolt bolt = go.GetComponent<WeaponBolt>();

        float dmg = userChar.GetCurDamage(bolt.damageType, out isCri, out addHit);
       
        bolt.InitialDamageInfo(_user, dmg, isCri, addHit);
        float predictTarSpeed = 0f;
        

        if (userChar is NPCController)
        {
            Transform attackTarget = (userChar as NPCController).attackTarget;
            CharacterController targetChar = attackTarget.GetComponent<CharacterController>();
            Vector3 targetPos = attackTarget.position
                + (targetChar==null ?
                Vector3.zero: targetChar.center);
            if (lastFireTime != 0)
            {   
                predictTarSpeed = Vector3.Distance(targetPos , lastTargetPos) / (Time.time - lastFireTime);                
                //Debug.Log(predictTarSpeed +" "+ attackTarget.forward);
            }
            bolt.SetShootTo(targetPos, predictTarSpeed, attackTarget.forward, (float)addHit/1000);
            lastTargetPos = targetPos;
            lastFireTime = Time.time;
        }
        else
        {
            bolt.Parabola = false;
            //20力多飛一倍
            bolt.SetShootTo(userChar.model.position+userChar.model.forward* userChar.status.GetPrimaryAttrubute( PrimaryAttributeName.Power
                ).AdjustedValue/20+new Vector3(0,userChar.BodyHeight/2,0), predictTarSpeed,Vector3.zero, (float)addHit / 1000);
        }
       

        bolt.Shoot();
    }
}
