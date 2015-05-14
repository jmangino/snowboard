using UnityEngine;
using System.Collections;

//TODO make it harder to rotate uphill, easier downhill
//FIXME add lowspeed braking
//FIXME clip through ground after jumping up at hill

public class PlayerMove : MonoBehaviour {
	//input
	private int PLAYER = 0;
	private string VERT ="";
	private string HORIZ ="";
	private bool gamepads = false;
	private float turn_strength=1;
	private float brake_strength=1;
	//movement
	private float dx =0, dy=0, dz=0;
	private float ddx = 0f, ddy = -0.35f, ddz = 0f;
	//limits
	private static float dmax = 100;
	private static float ddmax = 0.35f;
	private static float dthetamax = 0.92f;
	//angular
	private static float ddtheta = 0.55f;
	private static float dthetanot = 0.45f;
	private float dtheta = dthetanot;

	//correction
	private float ycorrect = 0.0f;

	//acceleration
	public float SPEEDUP; //set elsewhere
	public float DRAG; 
	public float AIRDRAG;
	private float JUMP = 0.12f;
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


	//*** GETTERS ***
	public Vector3 getDirection (){
		return new Vector3(dx,dy,dz);
	}
	
	public Vector3 getLookDirection (){
		Vector3 v = new Vector3 (dx, 0, dz);
		v.Normalize ();
		return v;
	}
	
	//*** END GETTERS ***




	// Use this for initialization
	void Start () {
		switch(gameObject.tag){
		case "Player1": 
			PLAYER = 1; 
			VERT = "VerticalP1";
			HORIZ = "HorizontalP1";
			break;
		case "Player2": 
			PLAYER = 2; 
			VERT = "VerticalP2";
			HORIZ = "HorizontalP2";
			break;
		default: 
			PLAYER = 1; 
			VERT = "VerticalP1";
			HORIZ = "HorizontalP1";
			break;
		}
		gamepads = Input.GetJoystickNames().Length > 1;
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void FixedUpdate(){

	}

	void OnCollisionEnter(Collision collision){
		//TODO player to wipe out on hard collision
		//FIXME player going through obj if normal is too close to vel
		Vector3 relative = collision.rigidbody.velocity - new Vector3(dx,dy,dz);
		ContactPoint[] contacts =  collision.contacts;
		Vector3 normal = new Vector3();
		foreach (ContactPoint p in contacts){
			normal += p.normal;
		}
		normal.Normalize();
		Debug.Log ("deflection: "+Vector3.Reflect(-relative, normal));
		float m2 = collision.rigidbody.mass;
		//TODO we are assuming player mass is 1
		float scale = 1 + Mathf.Pow(m2,2)+2*m2*Vector3.Dot(relative, normal);
		scale = Mathf.Sqrt(scale) / (1+m2);
		Debug.Log ("scale: "+scale);
		Vector3 vel = scale * Vector3.Reflect(-relative, normal);
		dx = vel.x; dy = vel.y; dz = vel.z;
		
		//impact object2 
		float angle = Mathf.Acos(Vector3.Dot (-relative, normal));
		Vector3 vel2 = -relative * 2/(1+m2) * Mathf.Sin(angle /2);
		collision.rigidbody.AddForce(vel2,ForceMode.Impulse);
		Debug.Log (vel2);
	}
	

	//Raycast after fixed update
	void LateUpdate(){
		useInputs ();
		//TODO instead of project straight down, factor in dx,dz (clipping through ground on ballistic falls)
		Physics.Raycast (transform.position, -Vector3.up, out to_ground,100f, ground_mask);
		//in air, NO HIT -> falling
		if (to_ground.distance > 0.03f) {
			onground = false;
			//dy = dy + ddy * dt
			dy += ddy * Time.deltaTime;
			//will hit ground
			if ( to_ground.distance + dy <= 0.03f) {
				transform.Translate (0, -to_ground.distance+0.01f, 0);
				//check normal of ground relative to movement? will determine if dy is conserved
				dy = 0;
				onground = true;
			}
			//airdrag
			float drag =  AIRDRAG;
			ddx = ddx - drag * dx * dx * Mathf.Sign(dx);
			ddz = ddz - drag * dz * dz * Mathf.Sign(dz);
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
			float speed = Mathf.Sqrt (dx*dx + dz * dz);
			if(speed > dmax ){
				dx = (dmax/speed) * dx;
				dz = (dmax/speed) * dz;
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


	private void useInputs(){
		//KEY INPUTS
		if (Input.GetKeyDown (KeyCode.LeftArrow)) {
			turnleft = true;
			turnright = false;
		}
		if (Input.GetKeyDown (KeyCode.RightArrow)) {
			//rotate dx,dz right
			turnright = true;
			turnleft = false;
		}
		if (Input.GetKeyDown (KeyCode.DownArrow)) {
			//slow down
			brake = true;
		}
		if (Input.GetKeyUp (KeyCode.LeftArrow)) {
			//stop turn left
			turnleft = false;
			dtheta = dthetanot;
		}

		if (Input.GetKeyUp (KeyCode.RightArrow)) {
			//stop turn right
			turnright = false;
			dtheta = dthetanot;
		}
		if (Input.GetKeyUp (KeyCode.DownArrow)) {
			//stop brake
			brake = false;
			braking = 1;
		}
		if(Input.GetKeyUp (KeyCode.Space)){
			//jump
			if(onground){
				dy = Mathf.Min(dy+JUMP, JUMP);
				onground = false;
				transform.Translate (0, 0.31f, 0);
			}
		}
		//AXES INPUTS
		if(gamepads){
			float vert = Input.GetAxis(VERT);
			float horiz = Input.GetAxis(HORIZ);
			//TODO axis intensity in controls
			turnleft = horiz < 0;
			turnright = horiz > 0;
			//stronger horizontal 0 to 1 -> % turn strength
			turn_strength = Mathf.Abs(horiz);
			brake = vert < 0;
			//1 to 0 with a strong emphasis towards 0 (no brake)
			brake_strength = 1 - Mathf.Pow(vert, 2);
		}

		//PROCESS INPUTS
		if (turnleft && onground) {
			dtheta = dtheta + ddtheta * Time.deltaTime * turn_strength;
			dtheta = Mathf.Min(dtheta, dthetamax);
			float theta = dtheta * Time.deltaTime;
			float tempdx = dx;
			dx = Mathf.Cos(theta) * dx - Mathf.Sin(theta) * dz;
			dz = Mathf.Cos (theta) * dz + Mathf.Sin(theta) * tempdx;
		}
		if (turnright && onground) {
			dtheta = dtheta + ddtheta * Time.deltaTime * turn_strength;
			dtheta = Mathf.Min(dtheta, dthetamax);
			float theta = -1 * dtheta * Time.deltaTime;
			float tempdx = dx;
			dx = Mathf.Cos(theta) * dx - Mathf.Sin(theta) * dz;
			dz = Mathf.Cos (theta) * dz + Mathf.Sin(theta) * tempdx;
		}
		if (brake) {
			//change drag factor
			braking = onground ? (12 * brake_strength) : 1;
		}

	}//end inputs



}//end class
