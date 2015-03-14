using UnityEngine;
using System.Collections;


public class PlayerMove : MonoBehaviour {
	private float dx =0, dy=0, dz=0;
	private float ddx = 0f, ddy = -0.1f, ddz = 0f;
	private float dmax = 100;

	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void FixedUpdate(){

	}



	//apparently raycasts need to happen after fixed update so do that here?
	void LateUpdate(){
		RaycastHit to_ground;
		//ISSUE: transform.position is player center not bottom
		//ISSUE: move sideways then downs so always grounded.
		Physics.Raycast (transform.position, -Vector3.up, out to_ground);
		//in air
		if (to_ground.distance > 3.2) {
			dy += ddy;
			//will hit ground
			if (to_ground.distance < dy) {
				dy = to_ground.distance + 2.5f;
			}
		} else { //on ground
			//method call here only makes sense for grounded objects
			dy = 0;
			findLocalGrad();
		}
		transform.Translate (new Vector3 (dx, dy, dz));

		Debug.Log ("local grad dx:"+dx+", dz:"+dz);


	}

	void findLocalGrad(){
		float delta = 0.15f;
		Vector3 zshift = new Vector3(0f,0f,delta);
		Vector3 xshift = new Vector3 (delta, 0f, 0f);
		Vector3 down = new Vector3(0f,-1f,0f);
		Vector3 dleft = down + zshift;
		Vector3 dright = down - zshift;
	
		RaycastHit r1, r2;
		Physics.Raycast (transform.position, dleft, out r1);
		Physics.Raycast (transform.position, dright, out r2);

		Debug.Log ("z distances: "+r1.distance+", "+r2.distance);

		dz = r1.distance - r2.distance;

		Vector3 dforward = down + xshift;
		Vector3 dback = down - xshift;

		Physics.Raycast (transform.position, dforward, out r1);
		Physics.Raycast (transform.position, dback, out r2);
		
		dx = r1.distance - r2.distance;

		Debug.DrawRay (transform.position, dleft, Color.blue);
		Debug.DrawRay (transform.position, dforward, Color.green);
		Debug.DrawRay (transform.position, dright, Color.cyan);
		Debug.DrawRay (transform.position, dback, Color.red);

	}

	public Vector3 getDirection (){
		return new Vector3(dx,dy,dz);
	}

	public Vector3 getLookDirection (){
		return new Vector3(dx,dy/3,dz);
	}




}
