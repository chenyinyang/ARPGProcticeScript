using UnityEngine;
using System.Collections;

public class VisionCollision : MonoBehaviour {

    public delegate void OnVisionEventHandler(Collider other);
    public event OnVisionEventHandler onTriggerEnter;
    public event OnVisionEventHandler onTriggerExit;
    void OnTriggerEnter(Collider other)
    {
        if (onTriggerEnter != null)
            onTriggerEnter(other);
    }
    void OnTriggerExit(Collider other)
    {
        if (onTriggerExit != null)
            onTriggerExit(other);
    }
}
