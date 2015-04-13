using UnityEngine;
using System.Collections;


public class PlayerMove : MonoBehaviour {
	private float dx =0, dy=0, dz=0;
	private float ddx = 0f, ddy = -0.1f, ddz = 0f;
	private float dmax = 100;
	public float SPEEDUP = 0.02f;
	public float DRAG = 0.01f; //add other kinds of drag?

	int ground_mask = 1<<8;



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
		RaycastHit to_ground;
		//ISSUE: move sideways then downs so always grounded.
		Physics.Raycast (transform.position, -Vector3.up, out to_ground,100f, ground_mask);
		//in air, NO HIT -> falling
		if (to_ground.distance > 0.1) {
			dy += ddy;
			//will hit ground
			if (to_ground.distance < dy) {
				dy = to_ground.distance + 0.1f;
			}
		} else { //on ground
			//method call here only makes sense for grounded objects
			dy = 0;
			findLocalGrad();
			dx += ddx;
			dz += ddz;
			//factor in max speed and drag?
		}
		transform.Translate (new Vector3 (dx, dy, dz));
		useInputs ();
		Debug.Log ("local grad dx:"+dx+", dz:"+dz);


	}

	void findLocalGrad(){
		float delta = 0.1f;
		Vector3 zshift = new Vector3(0f,0f,delta);
		Vector3 xshift = new Vector3 (delta, 0f, 0f);
		Vector3 down = new Vector3(0f,-1f,0f);
		Vector3 dleft = down + zshift;
		Vector3 dright = down - zshift;
		Vector3 position = transform.position; position.y += 1;
	
		RaycastHit r1, r2;
		Physics.Raycast (position, dleft, out r1, 10f, ground_mask);
		Physics.Raycast (position, dright, out r2, 10f, ground_mask);

		Debug.Log ("z distances: "+r1.distance+", "+r2.distance);

		ddz = r1.distance - r2.distance;
		ddz *= SPEEDUP;

		Vector3 dforward = down + xshift;
		Vector3 dback = down - xshift;

		Physics.Raycast (position, dforward, out r1, 10f, ground_mask);
		Physics.Raycast (position, dback, out r2, 10f, ground_mask);
		
		ddx = r1.distance - r2.distance;
		ddx *= SPEEDUP;

  

	}

	public Vector3 getDirection (){
		return new Vector3(dx,dy,dz);
	}

	public Vector3 getLookDirection (){
		return new Vector3(dx,dy/3,dz);
	}


	public void OnCollisionEnter(Collision collision){

	}

	private void useInputs(){
		//ISSUE: register inputs to axis?-----------
		if (Input.GetKeyDown (KeyCode.LeftArrow)) {
			//rotate dx,dz left
		}
		if (Input.GetKeyDown (KeyCode.RightArrow)) {
			//rotate dx,dz right
		}
		if (Input.GetKeyDown (KeyCode.DownArrow)) {
			//slow down
		}

	}



}
