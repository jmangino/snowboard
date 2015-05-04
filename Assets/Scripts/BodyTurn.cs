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

	private Vector3 direction;
	private Vector3 adjusted_direction = new Vector3();

	private float theta;
	private float theta0;
	private Quaternion target;
	// Use this for initialization
	void Start () {
		pm = this.GetComponentInParent<PlayerMove> ();
		theta0 = normal ? NORMAL : GOOFY;
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void LateUpdate(){
		//FIXME weird things happen if direction is 0
		//FIXME behaves poorly with low speed
		direction = pm.getLookDirection ();
		if (normal) {
			adjusted_direction.Set(direction.z,0,-direction.x);
		} else {
			adjusted_direction.Set(-direction.z,0,direction.x);
		}
		target.SetLookRotation (adjusted_direction);
		transform.rotation = Quaternion.RotateTowards (transform.rotation, target, STEP * Time.deltaTime);
	}
}
