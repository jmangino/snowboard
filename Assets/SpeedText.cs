using UnityEngine;
using System.Collections;

public class SpeedText : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		UnityEngine.UI.Text text = GetComponent<UnityEngine.UI.Text> ();
		GameObject player = GameObject.FindGameObjectWithTag ("Player");
		PlayerMove pm = player.GetComponent<PlayerMove> ();
		Vector3 speed = pm.getDirection ();
		speed.y = 0;
		text.text = "Speed: "+(int)(speed.magnitude*100)+" km/h";
	}
}
