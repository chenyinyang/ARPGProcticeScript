using UnityEngine;
using System.Collections;

public class BuffPanelGUI : MonoBehaviour {
    Transform myTransform;
    struct buffPanelChild {
     public  Transform transform;
     public  GameObject gameObject;
     public  UILabel LayerCountText;
     public  GameObject LayerCountTextGameObject;
     public  UITexture texture;
    }
    buffPanelChild[] childs;
    void Awake() {
        myTransform = transform;
        childs = new buffPanelChild[myTransform.childCount];
        for (int i = 0; i < childs.Length; i++)
        {
            childs[i].transform = myTransform.GetChild(i);
            childs[i].gameObject = childs[i].transform.gameObject;
            childs[i].LayerCountText = childs[i].transform.Find("LayerCount").GetComponent<UILabel>();
            childs[i].LayerCountTextGameObject = childs[i].transform.Find("LayerCount").gameObject;
            childs[i].texture = childs[i].transform.GetComponentInChildren<UITexture>();
        }       
    }

    public void SetBuffIcon(BuffSkill[] buffs)
    {
        for (int i = 0; i < myTransform.childCount; i++)
        {
            if (i < buffs.Length) {
                childs[i].gameObject.SetActive(true);
                childs[i].texture.mainTexture = buffs[i].BuffIcon;
                if (buffs[i].Stackable)
                {
                    childs[i].LayerCountTextGameObject.SetActive(true);
                    childs[i].LayerCountText.text = buffs[i].stackLayer.ToString();
                }
                else {
                    childs[i].LayerCountTextGameObject.SetActive(false);
                }
            } else {
                if (childs[i].texture != null)
                {
                    childs[i].texture.mainTexture = null;
                    childs[i].gameObject.SetActive(false);
                }
            }
        }        
    }
}
