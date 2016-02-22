using UnityEngine;
using System.Collections.Generic;
[RequireComponent(typeof(Collider))]
[AddComponentMenu("Game/Weapon/MeleeCollider")]
public class WeaponCollider : MonoBehaviour {
    public delegate void HitOnCharacter(Damagable character);
    public event HitOnCharacter hitOnCharacter;
    public GameObject _user;
    BaseCharacterBehavior userChar;
    Transform myTransform;
    Vector3 lastPos;
    Vector3 speed;
    private List<Damagable> hitOnOneAttack;

    // Use this for initialization
    void Awake() {
        Rigidbody rBody = gameObject.AddComponent<Rigidbody>();
        rBody.constraints = RigidbodyConstraints.FreezeAll;
        myTransform = transform;
        GetComponent<Collider>().isTrigger = true;
        if (_user.GetComponent<AttackAI>() != null) {
            _user.GetComponent<AttackAI>().weapon = this;
        }
    }
    void Start () {
        hitOnOneAttack = new List<Damagable>();
        lastPos = myTransform.position;
        userChar = _user.GetComponent<BaseCharacterBehavior>();
        userChar.onAttackStart += WeaponCollider_onAttackStart;
        userChar.onAttackStop += WeaponCollider_onAttackStop;
        userChar.meleeWeapon = this;
        //Messenger.AddListener<bool>(MessengerTopic.PLAYER_WEAPON_WAVE, PlayerAttackCallback);
    }

    private void WeaponCollider_onAttackStop()
    {
        hitOnOneAttack.Clear();
    }

    private void WeaponCollider_onAttackStart()
    {
    }

    public void Equip(GameObject user) {
        _user = user;
        if (_user.GetComponent<AttackAI>() != null)
        {
            _user.GetComponent<AttackAI>().weapon = this;
        }
    }
    public void Dequip() {
        _user = null;
        if (_user.GetComponent<AttackAI>() != null)
        {
            _user.GetComponent<AttackAI>().weapon = null;
        }
    }
	// Update is called once per frame
	void Update () {
        speed = myTransform.position - lastPos;
        lastPos = myTransform.position;
	}
    
    void OnTriggerEnter(Collider other) {
        Damagable targetDamage = other.GetComponent<Damagable>();
        if (_user && targetDamage != null && other.gameObject!=_user && !other.CompareTag(Tags.DeadBody))
        {
            if (userChar.CanDamageTarget(targetDamage))
            {                
                //處發擊中事件
                if (hitOnCharacter != null)
                {                    
                    hitOnCharacter(targetDamage);
                }
                //攻擊或發動技能時可造成傷害
                if ((userChar.IsAttacking || userChar.IsSkilling))
                {
                    //攻擊(不是放技能)
                    if (!userChar.IsSkilling)
                    {
                        //不在這次打擊過的清單
                        if (!hitOnOneAttack.Contains(targetDamage))
                        {
                            //加入
                            hitOnOneAttack.Add(targetDamage);
                            //攻擊成功 一次傷害判定,若為技能不影響取得傷害的部分
                            targetDamage.Damage(DamageType.Physical, userChar, userChar);
                        }
                        //已打擊過
                        else
                        {
                            //啥都不做
                            return;
                        }
                    }
                    else {
                        //技能觸發OnHitCharacter造成傷害
                    }
                }
            }
           
        }
    }
   
}
