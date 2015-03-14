using UnityEngine;
using System.Collections;

public class CameraMotion : MonoBehaviour {

	public GameObject player;
	
	public float distance_away;
	public float distance_up;
	private Transform follow; //this is the Player's transform
	private Vector3 target;
	public float smooth = 10;

	
	// Use this for initialization
	void Start () {
		follow = GameObject.FindGameObjectWithTag ("Player").transform;
	}
	
	// Update is called once per frame
	void Update () {
		
	}
	
	void LateUpdate(){
		if (player != null) {
			//decent but z rotation causes camera to fly forward and backward through object
			//target = player.transform.position + Vector3.up * distance_up - follow.forward * distance_away;

			Vector3 forward = player.GetComponent<PlayerMove>().getLookDirection();
			//rotate (0,0,1) normal forward around x axis by rotation of player's x rotation
			//Vector3 forward = Quaternion.AngleAxis(follow.eulerAngles.x,new Vector3(1,0,0)) * new Vector3(0,0,1);
			//ROTATE 0,0,1 AROUND X FOR CAM POS INDEP OF Y,Z
			target = player.transform.position + Vector3.up * distance_up - forward * distance_away;
			transform.position = Vector3.Lerp(target, transform.position, smooth * Time.deltaTime);
			transform.LookAt(player.transform);

		}
	}

	void CopyTransform(Transform dst , Transform src) {
		dst.localPosition = src.localPosition;
		dst.localRotation = src.localRotation;
		dst.localScale = src.localScale;
	}


}
