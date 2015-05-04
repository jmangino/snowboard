﻿using UnityEngine;
using System.Collections;

public class CameraMotion : MonoBehaviour {

	public GameObject player;
	
	public float distance_away;
	public float distance_up;
	private Vector3 target;
	public float smooth = 10;
	private Vector3 forward;
	private Vector3 up1 = new Vector3(0,1,0);

	//FIXME if player is going straight down, camera will go into player.If dir < threshold, pos relative to mesh?
	//re above: pos always relative to mesh?
	//FIXME behaves poorly with slow speed
	
	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {
		
	}
	
	void LateUpdate(){
		if (player != null) {
			forward = player.GetComponent<PlayerMove>().getLookDirection();
			//ROTATE 0,0,1 AROUND X FOR CAM POS INDEP OF Y,Z
			target = player.transform.position + Vector3.up * distance_up - forward * distance_away;
			transform.position = Vector3.Lerp(target, transform.position, smooth * Time.deltaTime);
			//look @ player center not board
			transform.LookAt(player.transform.position + up1);
		}
	}
	


}
