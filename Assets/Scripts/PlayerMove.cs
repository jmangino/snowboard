using UnityEngine;
using System.Collections;


public class PlayerMove : MonoBehaviour {
	//movement
	private float ddmax = 0.35f;
	private float dx =0, dy=0, dz=0;
	private float ddx = 0f, ddy = -0.1f, ddz = 0f;
	private float dmax = 100;
	//angular
	private float dtheta = 0.81f;

	//correction
	private float ycorrect = 0.0f;
	private float yclipup = 0.3f;
	//FIXME causing jumps when -dy is too great, immediately slowing in air (keep track of dy?)
	private float yclipdown = 0.2f;

	//acceleration
	public float SPEEDUP = 1f; //set elsewhere
	public float DRAG = 0.001f; //add other kinds of drag?

	int ground_mask = 1<<8;

	//controls
	private bool turnleft = false;
	private bool turnright = false;
	private bool brake = false;

	//flags
	private bool onground = false;

	//grad
	private static float delta = 0.1f;
	//ADJUST xandz shift to account for turned player
	private static Vector3 zshift = new Vector3 (0, 0, delta);
	private static Vector3 xshift = new Vector3 (delta, 0, 0);
	private static Vector3 down = new Vector3(0f,-1f,0f);

	Vector3 dleft = down + zshift;
	Vector3 dright = down - zshift;
	Vector3 dforward = down + xshift;
	Vector3 dback = down - xshift;

	RaycastHit r1, r2;
	RaycastHit to_ground;
	


	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void FixedUpdate(){

	}



	//Raycast after fixed update
	void LateUpdate(){
		useInputs ();
		//ISSUE: move sideways then downs so always grounded.
		Physics.Raycast (transform.position, -Vector3.up, out to_ground,100f, ground_mask);
		//in air, NO HIT -> falling
		if (to_ground.distance > 0.2f) {
			onground = false;
			//dy = dy + ddy * dt
			dy += ddy * Time.deltaTime;
			//will hit ground
			if ( to_ground.distance - dy < 0) {
				dy = to_ground.distance + 0.01f;
			}
		} else { //on ground
			onground = true;
			dy = 0;
			findLocalGrad();
			dx += ddx * Time.deltaTime;
			dz += ddz * Time.deltaTime;

			//set local x,z rotation to reflect grad

			//factor in max speed and drag?
		}
		transform.Translate (dx, dy, dz);
		//correct y
		ycorrect = 0;
		if(onground){
			Physics.Raycast (transform.position, -Vector3.up, out to_ground,10f, ground_mask);
			ycorrect = to_ground.distance;
			//gravity will naturally clip back to ground
			if(ycorrect < yclipdown && ycorrect != 0){

				ycorrect *= -1;
			}else {
				//clip up, fix this so I don't have to cast twice
				Physics.Raycast (transform.position, Vector3.up, out to_ground,10f, ground_mask);
				ycorrect = Mathf.Clamp(ycorrect, 0, yclipup);
			}
		}
		transform.Translate (0, ycorrect, 0);

	}

	void findLocalGrad(){
		Vector3 position = transform.position; position.y += 1;
		//TODO recalculate this by shooting straight down from sides, then use rise over run, currently approximating
		Physics.Raycast (position, dleft, out r1, 10f, ground_mask);
		Physics.Raycast (position, dright, out r2, 10f, ground_mask);
		ddz = (r1.distance - r2.distance)/(delta*2);
		//limit acceleration to no faster than gravity
		ddz = Mathf.Clamp (ddz, -1 * ddmax, ddmax);
		ddz *= SPEEDUP;

		Physics.Raycast (position, dforward, out r1, 10f, ground_mask);
		Physics.Raycast (position, dback, out r2, 10f, ground_mask);
		ddx = (r1.distance - r2.distance)/(delta*2);
		//limit acceleration to no faster than gravity
		ddx = Mathf.Clamp (ddx, -1 * ddmax, ddmax);
		ddx *= SPEEDUP;

	}

	//TODO fix the directions to make them turn independent (90 deg sideways is messing up a lot)
	public Vector3 getDirection (){
		return new Vector3(dx,dy,dz);
	}

	public Vector3 getLookDirection (){
		Vector3 v = new Vector3 (dx, 0, dz);
		v.Normalize ();
		return v;
	}


	public void OnCollisionEnter(Collision collision){

	}

	private void useInputs(){
		//ISSUE: register inputs to axis?-----------
		if (Input.GetKeyDown (KeyCode.LeftArrow)) {
			if(onground){
				turnleft = true;
				turnright = false;
			}
		}
		if (Input.GetKeyDown (KeyCode.RightArrow)) {
			//rotate dx,dz right
			if(onground){
				turnright = true;
				turnleft = false;
			}
		}
		if (Input.GetKeyDown (KeyCode.DownArrow)) {
			//slow down
			if(onground){
				brake = true;
			}
		}
		if (Input.GetKeyUp (KeyCode.LeftArrow)) {
			turnleft = false;
		}

		if (Input.GetKeyUp (KeyCode.RightArrow)) {
			turnright = false;
		}
		if (Input.GetKeyUp (KeyCode.DownArrow)) {
			brake = false;
		}

		//use inputs
		//TODO add theta acceleration so turning doesn't seem so jerky
		//TODO turning isn't perfectly aligned with player rotation, rotate player towards movement vector? 
		if (turnleft) {
			float theta = dtheta * Time.deltaTime;
			float tempdx = dx;
			dx = Mathf.Cos(theta) * dx - Mathf.Sin(theta) * dz;
			dz = Mathf.Cos (theta) * dz + Mathf.Sin(theta) * tempdx;
			//transform.Rotate(0,-theta*360.0f,0);
		}
		if (turnright) {
			float theta = -1 * dtheta * Time.deltaTime;
			float tempdx = dx;
			dx = Mathf.Cos(theta) * dx - Mathf.Sin(theta) * dz;
			dz = Mathf.Cos (theta) * dz + Mathf.Sin(theta) * tempdx;
			//transform.Rotate(0,-theta*360.0f,0);
		}
		if (brake) {
			//slow down (add drag)??
		}

	}



}
