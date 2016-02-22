using UnityEngine;
using System.Collections;
[RequireComponent(typeof(UILabel))]
public class PlayNameLabel : MonoBehaviour {

	// Use this for initialization
	void Start () {
        GetComponent<UILabel>().text = Player.PlayerName;
	}
	
}
