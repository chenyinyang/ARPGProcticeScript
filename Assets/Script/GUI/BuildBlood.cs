using UnityEngine;
using System.Collections;
[RequireComponent(typeof(UIFollowTarget))]
public class BuildBlood : BuildBloodOnGUI
{
    UIFollowTarget uiFollowTarget;
    protected override void Awake()
    {
        base.Awake();
        uiFollowTarget = GetComponent<UIFollowTarget>();
    }

    public void InitialWithTarget(Transform target,Build build)
    {
        base.InitialWithTarget(build);
        uiFollowTarget.gameCamera = Camera.main;
        uiFollowTarget.uiCamera = MyGUI.Instence.UICamera;
        uiFollowTarget.target = target;
        uiFollowTarget.disableIfInvisible = false;
        myTranform.parent = MyGUI.Instence.UIRoot;
        myTranform.localScale = Vector3.one*3;        
    }

    protected override void onUpdate() {
        if (build.IsDestroied)
            this.enabled = false;
        float dis =Vector3.ProjectOnPlane((uiFollowTarget.target.position - MyGUI.Instence.MainCamera.position),Vector3.up).magnitude;
        bool showUI= false;
        //距離很近
        if (dis < 3)
        {
            MyGUI.Instence.ShowBuildBlood(build);
            showUI = false;
        }
        //距離中等
        else if (dis < 6) {
            MyGUI.Instence.HideBuildBlood(build);
            //看的見 show
            if (uiFollowTarget.isVisible)
            {
                myTranform.localScale = Vector3.one * (3 - dis/3);
                showUI = true;
            }
            else {
                showUI = false;
            }
        }
        else
        {
            MyGUI.Instence.HideBuildBlood(build);
            showUI = false;
            //距離遠,看不見不SHOW           
        }
        if (hp.gameObject.activeSelf != showUI)
        {
            hp.gameObject.SetActive(showUI);
            towerName.gameObject.SetActive(showUI);
            if (build.IsTower)
                towerSet.gameObject.SetActive(showUI);
        }
        base.onUpdate();       
    }
    
}

