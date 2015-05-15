using UnityEngine;
using System.Collections;

public class BodyTurn : MonoBehaviour {
	//directional info
	private PlayerMove pm;

	//orientation
	private float GOOFY = -90f;
	private float NORMAL = 90f;

	public bool normal = true;
	public float STEP = 40f;
	private float step = 0;
	private float dstep = 20f;

	private Vector3 direction;
	private Vector3 adjusted_direction = new Vector3();
	private Vector3 default_direction;

	private float theta;
	private float theta0;
	private Quaternion target;

	private bool slow = false;
	// Use this for initialization
	void Start () {
		pm = this.GetComponentInParent<PlayerMove> ();
		theta0 = normal ? NORMAL : GOOFY;
		default_direction = normal? transform.forward : -transform.forward;
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void LateUpdate(){
		//make sure xzspeed is fast enough to get a useful direction vec
		Vector3 vel = pm.getVelocity();
		vel.Set(vel.x, 0, vel.z);
		slow = vel.magnitude < 0.008;
		//TODO CHANGE THIS IF DX DZ IS EVER RECALCULATED

		if(slow){
			step = 0;
			return;
		}

		//smooth transition between adjusting and not adjusting, don't want board "snap"
		step = Mathf.Min(step + dstep * Time.deltaTime, STEP); 
		direction = pm.getLookDirection ();
		//board angle is 90 deg off movement dir
		if (normal) {
			adjusted_direction.Set(direction.z,0,-direction.x);
		} else {
			adjusted_direction.Set(-direction.z,0,direction.x);
		}
		target.SetLookRotation (adjusted_direction);
		transform.rotation = Quaternion.RotateTowards (transform.rotation, target, step * Time.deltaTime);
	}

	public Vector3 getDirection(){
		return new Vector3(direction.x, direction.y, direction.z);
	}

	public bool isSlow(){
		return slow;
	}
}
