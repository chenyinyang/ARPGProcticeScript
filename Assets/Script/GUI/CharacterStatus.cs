using UnityEngine;
using System.Collections;
public class CharacterStatus : MonoBehaviour {

    public BaseCharacterBehavior character;
    public GameObject labelPref;
    Transform myTransform;
    const float UPDATE_INTERVAL = .1f;
    float updateTimer = 0;


    Vector3[] anglePoint = new Vector3[] {
            new Vector3(0,0,0),
            new Vector3(0,1,0),
            new Vector3(1,.5f,0),
            new Vector3(1,-.5f,0),
            new Vector3(0,-1,0),
            new Vector3(-1,-.5f,0),
            new Vector3(-1,.5f,0)
        };
    Vector3[] statusPoint = new Vector3[] {
        new Vector3(0,0,0),
        new Vector3(0,1,0),
        new Vector3(1,.5f,0),
        new Vector3(1,-.5f,0),
        new Vector3(0,-1,0),
        new Vector3(-1,-.5f,0),
        new Vector3(-1,.5f,0)
    };

    private BaseCharacterState status;

    private MeshFilter baseMesh;
    private MeshFilter adjustedMesh;
    private GameObject secAttrPanel;
    private GameObject conAttrPanel;
    Vector3 zIndex = Vector3.back;
    float scale = .3f;
    void Awake() {
        myTransform = transform;
        float uiRootScale = 1 / MyGUI.Instence.UIRoot.transform.localScale.x;
        //Fix point position
        scale *= uiRootScale;
        for (int i = 0; i < anglePoint.Length; i++)
        {
            anglePoint[i] *= scale;
        }
        //Create backgroun
        AddSubMesh("BackGround", new Color(.5f, .39f, .196f, 1f), 0, true, "Unlit/Color");
        AddSubMesh("Adjusted", new Color(1f, 1f, 0f, .5f), 1, false,"Unlit/Color");
        adjustedMesh = myTransform.Find("Adjusted").GetComponent<MeshFilter>();
        AddSubMesh("Base", new Color(1f, 0f, 0f, .5f), 2, false, "Unlit/Color");
        baseMesh = myTransform.Find("Base").GetComponent<MeshFilter>();
        CreateLine();

        secAttrPanel = myTransform.Find("SecAttrPanel").gameObject;        
        for (int i = 0; i < (int)SecondaryAttributeName.Count; i++)
        {
            UILabel secAttr = NGUITools.AddWidget<UILabel>(secAttrPanel);
            secAttr.transform.localPosition = new Vector3((i % 2==0 ? -125 : 100), -30 *(i/2), 0);
            secAttr.bitmapFont = labelPref.GetComponent<UILabel>().bitmapFont;
            secAttr.width = 200;
            secAttr.height = 30;
            secAttr.fontSize = 20;
            secAttr.alignment = NGUIText.Alignment.Left;
            secAttr.name = ((SecondaryAttributeName)i).ToString();
            secAttr.text = ((SecondaryAttributeName)i).ToString() + " : " + 0;
        }
        conAttrPanel = myTransform.Find("ConAttrPanel").gameObject;
      
    }
    // Use this for initialization
    void Start () {
        status = character.status;
       
    }
	
	// Update is called once per frame
	void Update () {
        if (Time.time - updateTimer > UPDATE_INTERVAL)
        {
            updateTimer = Time.time;
            UpdateBaseStatusPoint();
            UpdateAdjustedStatusPoint();
            UpdateSecAttr();
            UpdateConAttr();
        }
    }
    #region PriAttr
    private void AddSubMesh(string name,Color color,int order,bool initialText,string shader) {
        //Create Backgrond
        GameObject meshObject = new GameObject();
        meshObject.name = name;
        meshObject.transform.parent = myTransform;
        meshObject.transform.position = myTransform.position;
        meshObject.transform.localPosition = Vector3.forward * -5;
        meshObject.layer = gameObject.layer;
        MeshFilter backMesh = meshObject.AddComponent<MeshFilter>();
        backMesh.mesh = CreateMesh();
        meshObject.AddComponent<MeshRenderer>().material.shader = Shader.Find(shader);
        meshObject.GetComponent<MeshRenderer>().material.color = color;
        meshObject.GetComponent<MeshRenderer>().sortingOrder = order;
        meshObject.transform.localScale = Vector3.one;
        for (int i = 1; i < anglePoint.Length; i++)
        {            
            UILabel uiLabel = NGUITools.AddWidget<UILabel>(meshObject);
            uiLabel.name = ((PrimaryAttributeName)(i - 1)).ToString();
            if (initialText)
                uiLabel.text = ((PrimaryAttributeName)(i - 1)).ToString();
            uiLabel.bitmapFont = labelPref.GetComponent<UILabel>().bitmapFont;            
            uiLabel.transform.localPosition =  anglePoint[i] * 1.2f;
            uiLabel.fontSize = 20;
        }
    }

    private void CreateLine() {
        //Create row line
        GameObject lines = new GameObject();
        lines.name = "lines";
        lines.transform.parent = myTransform;
        lines.transform.position = myTransform.position;
        lines.layer = gameObject.layer;
        LineRenderer line = lines.AddComponent<LineRenderer>();
        line.SetWidth(0.005f, 0.005f);
        line.SetVertexCount((anglePoint.Length - 1) * 2);
        int linePosIndex = 0;
        for (int i = 1; i < anglePoint.Length; i++)
        {
            line.SetPosition(linePosIndex++, anglePoint[0]);
            line.SetPosition(linePosIndex++, anglePoint[i]);
        }
        line.material.shader = Shader.Find("UI/Default");
        line.material.color = new Color(0.313f, 0.235f, 0, 0.784f);
        line.useWorldSpace = false;
        line.sortingOrder = 3;
        lines.transform.localScale = Vector3.one;
        line.transform.localPosition = Vector3.forward * -5;
    }
    private Mesh CreateMesh() {
        Mesh mesh = new Mesh();
        mesh.vertices = anglePoint;
        mesh.triangles = new int[] {
            0, 1, 2,
            0, 2, 3,
            0, 3, 4,
            0, 4, 5,
            0, 5, 6,
            0, 6, 1
        };

        mesh.uv = new Vector2[] {
            new Vector2(0,0),
            new Vector2(0,0),
            new Vector2(1,0),
            new Vector2(1,0),
            new Vector2(0,0),
            new Vector2(-1,0),
            new Vector2(-1,0)
        };
        return mesh;
    }
    void UpdateBaseStatusPoint()
    {
        Vector3[] baseValuePoint = new Vector3[anglePoint.Length];
        baseValuePoint[0] = anglePoint[0];
        for (int i = 1; i < baseValuePoint.Length; i++)
        {
            baseValuePoint[i] = anglePoint[i] *(status.GetPrimaryAttrubute((PrimaryAttributeName)i - 1).baseValue > 110 ? 110 : status.GetPrimaryAttrubute((PrimaryAttributeName)i - 1).baseValue) / 100;
        };
       
        for (int i = 0; i < baseValuePoint.Length-1; i++)
        {
            UILabel label = baseMesh.transform.Find(((PrimaryAttributeName)i).ToString()).GetComponent<UILabel>();
            label.transform.localPosition = baseValuePoint[i + 1]*0.8f;
            label.text = status.GetPrimaryAttrubute((PrimaryAttributeName)i).baseValue.ToString();
            label.color = Color.white;
        }
        
        baseMesh.mesh.Clear();
        baseMesh.mesh.vertices = baseValuePoint;
        baseMesh.mesh.triangles = new int[] {
            0, 1, 2,
            0, 2, 3,
            0, 3, 4,
            0, 4, 5,
            0, 5, 6,
            0, 6, 1
        };
    }
    void UpdateAdjustedStatusPoint()
    {
        Vector3[] AdjustedValuePoint = new Vector3[anglePoint.Length];
        AdjustedValuePoint[0] = anglePoint[0];
        for (int i=1;i<AdjustedValuePoint.Length;i++)
        {            
           AdjustedValuePoint[i] = anglePoint[i] * (status.GetPrimaryAttrubute((PrimaryAttributeName)i - 1).AdjustedValue > 110 ? 110 : status.GetPrimaryAttrubute((PrimaryAttributeName)i - 1).AdjustedValue) / 100;           
        };

        for (int i = 0; i < AdjustedValuePoint.Length - 1; i++)
        {
            UILabel label = adjustedMesh.transform.Find(((PrimaryAttributeName)i).ToString()).GetComponent<UILabel>();
            label.transform.localPosition = AdjustedValuePoint[i + 1] * 0.8f;
            label.text = status.GetPrimaryAttrubute((PrimaryAttributeName)i).baseValue == status.GetPrimaryAttrubute((PrimaryAttributeName)i).AdjustedValue?"": status.GetPrimaryAttrubute((PrimaryAttributeName)i).AdjustedValue.ToString();
            label.color = Color.green;
            label.fontSize = 24;
        }

        adjustedMesh.mesh.Clear();
        adjustedMesh.mesh.vertices = AdjustedValuePoint;
        adjustedMesh.mesh.triangles = new int[] {
            0, 1, 2,
            0, 2, 3,
            0, 3, 4,
            0, 4, 5,
            0, 5, 6,
            0, 6, 1
        };
        
    }
    #endregion

    void UpdateSecAttr() {
        
        for (int i = 0; i < (int)SecondaryAttributeName.Count; i++)
        {   
            UILabel secAttr =secAttrPanel.transform.Find(((SecondaryAttributeName)i).ToString()).GetComponent<UILabel>();            
            secAttr.text = ((SecondaryAttributeName)i).ToString() + " : " + status.GetSecondaryAttrubute((SecondaryAttributeName)i).AdjustedValue;
        }
    }
    void UpdateConAttr()
    {       
        UISlider conAttr;
        for (int i = 0; i < (int)ConsumedAttributeName.Count; i++)
        {
            conAttr = conAttrPanel.transform.Find(((ConsumedAttributeName)i).ToString()+"Bar").GetComponent<UISlider>();
            conAttr.value = status.GetConsumedAttrubute((ConsumedAttributeName)i).CurValue / status.GetConsumedAttrubute((ConsumedAttributeName)i).AdjustedValue;
            conAttr.GetComponentInChildren<UILabel>().text = status.GetConsumedAttrubute((ConsumedAttributeName)i).CurValue.ToString("0")+" / "+ status.GetConsumedAttrubute((ConsumedAttributeName)i).AdjustedValue.ToString("0");
        }
        conAttr = conAttrPanel.transform.Find("ExpBar").GetComponent<UISlider>();
        conAttr.value = status.Exp / status.ExpToLevel;
        conAttr.GetComponentInChildren<UILabel>().text = status.Exp.ToString("0") + " / " + status.ExpToLevel.ToString("0");
        conAttrPanel.transform.Find("Level").GetComponent<UILabel>().text = "Lv. " + status.Level;
    }
}
