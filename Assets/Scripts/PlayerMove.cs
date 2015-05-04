using UnityEngine;
using System.Collections;

//TODO make it harder to rotate uphill, easier downhill
//FIXME add lowspeed braking
//FIXME clipping through ground after fall
//could be setting dy to 0 makes shift too large, or ballistic is going through ground entirely
//ballistic checks straight down w/o considering dx,dz
public class PlayerMove : MonoBehaviour {
	//movement
	private float dx =0, dy=0, dz=0;
	private float ddx = 0f, ddy = -0.35f, ddz = 0f;
	//limits
	private float dmax = 100;
	private float ddmax = 0.35f;
	private float dthetamax = 1.2f;
	//angular
	private float dtheta = 0.81f;
	private float ddtheta = 1.2f;

	//correction
	private float ycorrect = 0.0f;

	//acceleration
	public float SPEEDUP; //set elsewhere
	public float DRAG; 
	public float AIRDRAG;
	private float drag_multiplier = 1f;
	private float braking = 1f;

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

	Vector3 up2 = new Vector3(0, 2, 0);


	void OnCollisionEnter(Collision collision){
		Vector3 v = collision.relativeVelocity;
Debug.Log (v);
	}

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
		//TODO instead of project straight down, factor in dx,dz (clipping through ground on ballistic falls)
		Physics.Raycast (transform.position, -Vector3.up, out to_ground,100f, ground_mask);
		//in air, NO HIT -> falling
		if (to_ground.distance > 0.2f) {
			onground = false;
			//dy = dy + ddy * dt
			dy += ddy * Time.deltaTime;
			//will hit ground
			if ( to_ground.distance - dy < 0) {
				transform.Translate (0, -to_ground.distance, 0);
				//check normal of ground relative to movement? will determine if dy is conserved
				dy = 0;
				onground = true;
			}
			//in air, y is ballistic
			transform.Translate (dx, dy, dz);
		} else { //on ground
			onground = true;
			findLocalGrad();
			//factor in drag
			float drag = drag_multiplier * braking * DRAG;
			ddx = ddx - drag * dx * dx * Mathf.Sign(dx);
			ddz = ddz - drag * dz * dz * Mathf.Sign(dz);

			dx += ddx * Time.deltaTime;
			dz += ddz * Time.deltaTime;
			//factor in max speed
			float speedsq = dx*dx + dz * dz;
			if(speedsq > dmax * dmax){

			}
			//move after ddx,ddz,dx,dz are calculated
			transform.Translate (dx, 0, dz);
		}
		//correct y
		ycorrect = 0;
		if(onground){
			bool hit = 	Physics.Raycast (transform.position + up2, -Vector3.up, out to_ground,10f, ground_mask);
			//able to clip to ground
			if(hit){
				ycorrect =	to_ground.point.y - transform.position.y;
				//based on gravity, correction is plausible
				if(Mathf.Abs(ycorrect - dy) <= 0.3){
					transform.Translate (0, ycorrect+0.01f, 0);
					dy = ycorrect;
				}
			}
		}//end if onground
	}//end of late update

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
	
	public Vector3 getDirection (){
		return new Vector3(dx,dy,dz);
	}

	public Vector3 getLookDirection (){
		Vector3 v = new Vector3 (dx, 0, dz);
		v.Normalize ();
		return v;
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
				braking = 8;
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
			braking = 1;
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

		}

	}//end late update



}//end class
