using UnityEngine;
using System.Collections;
[RequireComponent(typeof(Collider))]
[AddComponentMenu("Game/Weapon/Bolt")]
public class WeaponBolt : MonoBehaviour {
    const float GRAVITY = 1f;
    float flyingTime = .5f;
    float flyingTimer = 0;

    Transform firer;
    BaseCharacterBehavior firerChar;
    public DamageType damageType;
    float damageMount;
    bool isCritical;
    float additionHit;
    

    public Vector3 targetPos;
    public Vector3 fromPos;
    /// <summary>
    /// 拋物線
    /// </summary>
    public bool Parabola;
    float baseSpeed = 1f;
    bool hit;
    Vector3 initialSpeed;
    Transform myTransform;
    void Awake() {
        myTransform = transform;
    }
    public void InitialDamageInfo(Transform firer, float damageMount,  bool isCritical, float additionHit)
    {
        this.firer = firer;
        firerChar = firer.GetComponent<BaseCharacterBehavior>();
        this.damageMount = damageMount;
        
        this.isCritical = isCritical;
        this.additionHit = additionHit;
    }
    public void SetShootTo(Vector3 target,float predictTarSpeed,Vector3 targetForward,float additionSpeed) {        
        
        baseSpeed += additionSpeed;
        fromPos = myTransform.position;        
        //時間 =      值線距離          /                       速度
        flyingTime = Vector3.Distance(target, fromPos) / (baseSpeed + predictTarSpeed * Vector3.Dot(targetForward,fromPos-target));
        //Debug.Log(flyingTime +" = " + Vector3.Distance(target, fromPos) +" / ("+baseSpeed +" + "+ predictTarSpeed+")");
        targetPos = target + targetForward* predictTarSpeed * flyingTime;
    }
    public void Shoot() {
        
        flyingTimer =Time.time;
        if (Parabola)
        {
            // 拋物線函式
            // y = -_gravity / 2 * t * t + a * t + c;
            // transform.po.y = c
            // a = (y-c)/t + _g/2*t
            var a = (targetPos.y - myTransform.position.y) / flyingTime + GRAVITY / 2 * flyingTime;
            //Debug.Log(a);
            //v = _g *t + a
            //v0 = a
            var speedY = a * Vector3.up;
            //經過最高點的秒數 t
            //float shootingTime = (flyingTime / baseSpeed) /2;
            //每秒減速度 , a
            //var gravityPerSec = Vector3.up * _gravity;
            //每秒速度下降 dV = a * dt, 到達最高點之前的速度下降量 = 初速度
            //var speedY = gravityPerSec * shootingTime;

            
            float ang = Vector3.Angle(myTransform.position, targetPos);
            //transform.Rotate(Vector3.up, ang);
            
            //等速前進,平面
            var speedZ = Vector3.ProjectOnPlane(myTransform.forward, Vector3.up) * baseSpeed;
            initialSpeed = speedZ + speedY;
        }
        Vector3 tarDir = targetPos - myTransform.position;
        myTransform.rotation = Quaternion.LookRotation(tarDir);
    }

    // Update is called once per frame
    void Update() {
        if (flyingTimer == 0)
            return;
        if(Time.time - flyingTimer > flyingTime+1)
        {
            //return;
            //飛行時間玩過一秒銷毀
            Destroy(gameObject);
        }
        if (hit)
            return;
        
        if (Parabola)
        {
            myTransform.position += initialSpeed * Time.deltaTime;           
            //速度仰角
            var angle = Vector3.Angle(myTransform.forward, initialSpeed);
            //Debug.Log(angle);
            myTransform.Rotate(Vector3.right, angle);
            initialSpeed += Vector3.down * Time.deltaTime * GRAVITY;
        }
        else
        {
            myTransform.position += myTransform.forward*baseSpeed*Time.deltaTime;
        }
    }
    public void Hit(Transform target) {
       
    }
    void OnTriggerEnter(Collider other)
    {
        //Debug.Log("OnTriggerEnter" + other.gameObject.name);
        if (other.transform != firer)
        {
            Damagable hitTarget = other.transform.GetComponent<Damagable>();
            if (hitTarget != null && !hit && firerChar.CanDamageTarget(hitTarget))
            {        
                hit = true;
                hitTarget.Damage(damageType, firerChar, firerChar);
                transform.parent = other.transform;
            }
            //Destroy(gameObject);
        }
    }
}
