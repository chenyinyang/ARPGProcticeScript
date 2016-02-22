using UnityEngine;
using System.Collections;
using System;

public class CharacterPanel : MonoBehaviour , IClosablePanel
{
    public static CharacterPanel Instance;
    public bool IsOpen { get { return characterWindow.activeSelf; } }
    GameObject characterWindow;
    public void CloseWindow()
    {
        characterWindow.SetActive(false);
    }
    void Awake(){
        Instance = this;
        //UIEventListener.Get(gameObject).onClick = (g) => { };
    }
    void Start() {
        characterWindow = transform.Find("Window").gameObject;
    }
	// Update is called once per frame
	void Update () {
       
	}
    public void Trigger() {        
        characterWindow.SetActive(!characterWindow.activeSelf);
    }

    public void Close()
    {
        characterWindow.SetActive(false);     
    }
}
