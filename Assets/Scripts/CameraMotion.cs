using UnityEngine;
using System.Collections;

public class CameraMotion : MonoBehaviour {

	public GameObject player;
	
	public float distance_away;
	public float distance_up;
	private Vector3 target;
	public float smooth = 10;
	private Vector3 forward;
	private Vector3 up1 = new Vector3(0,1,0);
	private BodyTurn Body;
	private GameObject SlowPoint;

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
			transform.position = Vector3.Lerp( transform.position, target, smooth * Time.deltaTime);
			//look @ player center not board , possibly have an obj for this
			transform.LookAt(player.transform.position + up1);
		}
	}
	


}
