using UnityEngine;
using System.Collections;

public class TowerInfoPanel : MonoBehaviour,IClosablePanel {
    public static TowerInfoPanel Instance;
    public bool IsOpen { get { return windowPanel.gameObject.activeSelf; } }
    Transform windowPanel;
    UITexture warTexture;
    Transform followersPanel;
    Tower towerBind;
    Transform myTransform;
    const float UPDATE_INTERVAL = .1f;
    float updateTimer = 0;

    void Awake(){
        myTransform = transform;
        Instance = this;
        windowPanel = myTransform.Find("Window");
        warTexture = windowPanel.Find("Warmap").Find("WarTexture").GetComponent<UITexture>();
        followersPanel = windowPanel.Find("Followers");
        Close();
    }	
	
	// Update is called once per frame
	void onUpdate() {
        if (towerBind != null)
        {
            for (int i = 0; i < 5; i++)
            {
                Follower f = followersPanel.GetChild(i).GetComponent<Follower>();
                if (towerBind.Followers.Count > i)
                {
                    if (!f.gameObject.activeSelf)
                        f.gameObject.SetActive(true);
                    f.BindNPC(towerBind.Followers[i], towerBind);
                }
                else
                {
                    f.gameObject.SetActive(false);
                }
            }

        }
	}

    public void Open(Tower tower,Texture warTexture) {
        MyGUI.Instence.CloseAllOpenedPanel();
        if (!windowPanel.gameObject.activeSelf)
            windowPanel.gameObject.SetActive(true);
        towerBind = tower;
        this.warTexture.mainTexture = warTexture;
        InvokeRepeating("onUpdate", 0, UPDATE_INTERVAL);
    }
    public void Close() {
        if (windowPanel.gameObject.activeSelf)
        {
            windowPanel.gameObject.SetActive(false);
            CancelInvoke("onUpdate");
        }
    }

}
