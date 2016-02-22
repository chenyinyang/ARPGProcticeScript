using UnityEngine;
using System.Collections;

public class ColliderChild : MonoBehaviour {

    public delegate void onTriggerEnter(Collider other);
    public onTriggerEnter triggerEnter;
    public delegate void onTriggerExit(Collider other);
    public onTriggerExit triggerExit;
    void OnTriggerEnter(Collider other) {
        if (triggerEnter != null) {
            triggerEnter(other);
        }
    }
    void OnTriggerExit(Collider other)
    {
        if (triggerExit != null)
        {
            triggerExit(other);
        }
    }
}
