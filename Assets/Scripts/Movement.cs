using UnityEngine;
using System.Collections;

public class Movment : MonoBehaviour {

	public GameObject player;

	private Vector3 offset = Vector3.zero;

	// Use this for initialization
	void Start () {
		offset = new Vector3 (0, 2, 5);
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void FixedUpdate(){
		if (player != null) {
			transform.LookAt(player.transform);
			transform.position = player.transform.position + offset;
		}


	}
}
