using UnityEngine;
using System.Collections;
[RequireComponent(typeof(Collider))]
public class PlayerWeaponCollider : WeaponCollider
{
    Vector3 lastPos;
    Vector3 speed;
    private bool isPlayerAttacking;
    // Use this for initialization
    void Start()
    {
        isPlayerAttacking = false;
        lastPos = transform.position;
        Messenger.AddListener<bool>(MessengerTopic.PLAYER_WEAPON_WAVE, PlayerAttackCallback);
    }

    // Update is called once per frame
    void Update()
    {
        speed = transform.position - lastPos;
        lastPos = transform.position;
    }
    void OnTriggerEnter(Collider other)
    {

        //if (other.CompareTag(Tags.Monster)) {
        //    Debug.Log("Weapon Hit");
        //    other.gameObject.GetComponent<NPC>().Damage(10);
        //}
    }
    void OnCollisionEnter(Collision other)
    {
        if (other.collider.CompareTag(Tags.Monster))
        {

            if (isPlayerAttacking)
            {
                // && speed.magnitude / Time.deltaTime > 1

                Debug.Log("Weapon Hit");
                //other.gameObject.GetComponent<NPC>().Damage(10);
            }
        }
    }

    void PlayerAttackCallback(bool attack)
    {
        isPlayerAttacking = attack;
    }
}
