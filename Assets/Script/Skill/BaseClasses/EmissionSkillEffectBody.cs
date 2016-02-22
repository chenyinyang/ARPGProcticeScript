using UnityEngine;
using System.Collections;

public class EmissionSkillEffectBody : MonoBehaviour {

    void OnTriggerEnter(Collider other)
    {
        transform.GetComponentInParent<EmissionSkillEffect>().OnTriggerEnter(other);
    }
    //void OnCollisionEnter(Collision other) {
    //    transform.GetComponentInParent<EmissionSkillEffect>().OnCollisionEnter(other);
    //}
}
