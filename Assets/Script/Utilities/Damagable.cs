using UnityEngine;
using System.Collections;

public class Damagable : MonoBehaviour {
    internal Transform MyTransform;
    public delegate void DamageHandler(DamageType type, IDamageMaker damageMaker, BaseCharacterBehavior source);
    public event DamageHandler damageHandler;
    public void Damage(DamageType type, IDamageMaker damageMaker, BaseCharacterBehavior source)
    {
        if (damageHandler != null) {
            damageHandler(type, damageMaker, source);
        }
    }
    bool isPlayer;
    bool isTowerBuild;
    Damagable towerFlag;
    void Start() {
        MyTransform = transform;
        isPlayer = GetComponent<Player>() != null;
        isTowerBuild = GetComponent<Tower>()!=null;
        if (isTowerBuild)
            towerFlag = GetComponent<Tower>().MainFlag.GetComponent<Damagable>();
        gameObject.layer = Layers.Clickable;
        UIEventListener eventListener = UIEventListener.Get(gameObject);
        eventListener.onClick = (obj) => {
            if (UICamera.currentTouchID == -2)
            {
                if(isPlayer)
                    Player.Instance.CommandFollowersStopAttack();
                else if (!Tags.IsCompanion(this,Player.Instance) &&
                    Vector3.ProjectOnPlane(MyTransform.position - Player.Instance.transform.position, Vector3.up).magnitude < 3f)
                {
                    if(isTowerBuild)
                        Player.Instance.CommandFollowersAttack(towerFlag);
                    else
                        Player.Instance.CommandFollowersAttack(this);
                }
            }
            
        };
    }

    void OnParticleCollision(GameObject other)
    {

        //return;
        SkillEffect sk = other.GetComponent<SkillEffect>();
        while (sk == null && other.transform.parent != null)
            sk = other.GetComponentInParent<SkillEffect>();
        if (sk != null && sk.user.CanDamageTarget(this))
        {
            //撞到粒子特效呼叫skill的DamageMaker,回傳粒子特效傷害
            Damage(sk.EffectDamageType,sk,sk.user);
            sk.ParticleCollision(other, this);
            //transform.position -= transform.forward * Time.deltaTime;
        }
    }
}
