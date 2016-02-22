using UnityEngine;
using System.Collections;

public class HurtText : MonoBehaviour {
    private const float TEXT_STAY_TIME = 5f;
    private HUDText hudText;
    private Color playerDamage = Color.Lerp(Color.yellow, Color.red, .2f);
    private Color playerHeal = Color.green;
    private Color npcDamage = Color.red;
    UIFollowTarget uiFollowTarget;
    Transform myTransform;
    private bool isPlayer;
    private bool ShowText {
        get {            
            return (uiFollowTarget.isVisible && Vector3.Distance(MyGUI.Instence.MainCamera.position, uiFollowTarget.target.position)<5);
        }
    }
    public void InitailHUDText(Transform target) {
        uiFollowTarget.target = target;
        uiFollowTarget.gameCamera = Camera.main;
        uiFollowTarget.uiCamera = MyGUI.Instence.UICamera;
        myTransform.parent = MyGUI.Instence.UIRoot;
        myTransform.localScale = Vector3.one;
        isPlayer = target.GetComponentInParent<Player>() != null;
    }
    private void AddText(string str,Color color,float stay,bool large = false,bool force =false) {
        
        if (force)
        {
            hudText.fontSize = (int)(((isPlayer ? 30 : 20) * (large ? 2 : 1)) );
            hudText.Add(str, color, stay);
        }
        else
        {
            if (!ShowText)
                return;
            hudText.fontSize = (int)(((isPlayer ? 30 : 20) * (large ? 2 : 1)) * (6 - Vector3.Distance(Camera.main.transform.position, GetComponent<UIFollowTarget>().target.position)) / 5);
            hudText.Add(str, color, stay);
        }
    }
    public void SetText(int value, bool large = false,bool force = false) {        
        AddText(value.ToString(), value > 0 ? playerHeal : isPlayer ? playerDamage : npcDamage, 0f, large, force);                
    }

    public void SetText(int value, Color color)
    {
        AddText(value.ToString(), color, 0f);
    }
    public void SetText(string text)
    {        
        AddText(text, Color.blue, 0f);
    }
    public void SetText(string text, Color color)
    {
        AddText(text,color, 0f);
    }
    public void Say(string text)
    {
        AddText(text, Color.white, TEXT_STAY_TIME);
    }
    // Use this for initialization

    void Awake() {
        myTransform = transform;
        hudText = GetComponentInChildren<HUDText>();
        uiFollowTarget = GetComponent<UIFollowTarget>();
    }	
}
