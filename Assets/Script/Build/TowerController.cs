using UnityEngine;

public class TowerController : MonoBehaviour {
    RenderTexture eargleEyeTexture;
    static Camera eargleEyeCamera;
    Tower tower;
    Transform myTransform;

    protected float updateInterval = 1f;
    protected float updateTimer = 0;

    void Awake() {
        myTransform = transform;
        if (eargleEyeCamera == null)
        {
            eargleEyeCamera = GameObject.Find("EargleEyeCamera").GetComponent<Camera>();
            eargleEyeCamera.enabled = false;
        }

        eargleEyeTexture = ResourceLoader.Utilities.GetEargleEye();       
        Transform plane = myTransform.Find("Plane");
        Material m = new Material(Shader.Find("Unlit/Texture"));
        m.shader = Shader.Find("Unlit/Texture");
        m.mainTexture = eargleEyeTexture;
        plane.GetComponent<MeshRenderer>().material = m;        

        tower = GetComponentInParent<Tower>();

        UIEventListener.Get(gameObject).onClick = (g) => {
            if (UICamera.currentTouchID==-2)
            {
                TowerInfoPanel.Instance.Open(tower, eargleEyeTexture);
            }
        };
        gameObject.layer = Layers.Clickable;
    }
    
    protected void Update()
    {
        if (Time.time - updateTimer > updateInterval && Vector3.Distance(Player.Instance.transform.position, myTransform.position) < 2)
        {
            onUpdate();
            updateTimer = Time.time;
        }
    }
    void onUpdate() {
        if (Vector3.Distance(Player.Instance.transform.position, myTransform.position) < 1 ) {
            if(!eargleEyeCamera.enabled)
                eargleEyeCamera.enabled = true;
        }
        else
        {
            if (eargleEyeCamera.enabled)
            {
                eargleEyeCamera.enabled = false;
                if (TowerInfoPanel.Instance.IsOpen)
                    TowerInfoPanel.Instance.Close();
            }
        }
    }
   
}
