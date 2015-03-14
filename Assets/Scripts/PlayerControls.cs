using UnityEngine;
using System.Collections;

public class PlayerControls : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void StickToWorldspace(Transform root, Transform camera, ref float directionOut, ref float speedOut){
	/*	Vector3 rdir = root.forward;
		Vector3 sdir = new Vector3 (dx, 0, dz);
		speedOut = sdir.sqrMagnitude;
		Vector3 cdir = camera.forward;
		cdir.y = 0.0f;
		Quaternion referentialshift = Quaternion.FromToRotation (Vector3.forward, cdir);
		*/
	}
}
