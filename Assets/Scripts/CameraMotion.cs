using UnityEngine;
using System.Collections;

public class CameraMotion : MonoBehaviour {

	public GameObject player;
	
	public float distance_away;
	public float distance_up;

	private Vector3 target;
	private Vector3 forward;
	private Vector3 up1 = new Vector3(0,1,0);

	public float smooth = 10;
	private float ground_buffer = 0.9f;

	private BodyTurn Body;
	private GameObject SlowPoint;

	//raycast
	int ground_mask = 1<<8;
	RaycastHit to_ground;

	//TODO speed up camera to catch fast player

	//TODO maybe do an orbital camera
	
	// Use this for initialization
	void Start () {
		Body = player.GetComponentInChildren<BodyTurn>();
		SlowPoint = GameObject.FindGameObjectWithTag("CameraPoint");
	}
	
	// Update is called once per frame
	void Update () {
		
	}
	//FIXME cam is snapping too hard
	void LateUpdate(){
		if (player != null) {
			forward = Body.getDirection();
			//if speed is too low to use for cam pos, go to default location
			if(Body.isSlow()){
				forward = player.transform.position - SlowPoint.transform.position;
				forward.Normalize();
			}
			//move up and away from player
			target = player.transform.position + Vector3.up * distance_up - forward * distance_away;
			//check if target in ground then move up
			Vector3 target_dir = target - transform.position;
			//cast towards target, distance between, see if passes through ground
			bool hit = Physics.Raycast (transform.position, target_dir, out to_ground, target_dir.magnitude, ground_mask);
			//going through ground so move up
			if(hit){
				Physics.Raycast (target, Vector3.up, out to_ground, 5f, ground_mask);
				//move target up, distance through ground and then up by ground buffer
				target += Vector3.up * (to_ground.distance + ground_buffer);
				//Debug.Log("camera correct: "+ (to_ground.distance + ground_buffer));
			}
			transform.position = Vector3.Lerp( transform.position, target, smooth * Time.deltaTime);
			//look @ player center not board , possibly have an obj for this
			transform.LookAt(player.transform.position + up1);
		}
	}
	


}
