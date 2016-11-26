using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour {

	public static GameObject player;

	public float velocity;
	public float accel = 2;

	public float rotation_speed = 20;

	Rigidbody rigidbody;

	void Start () {
		Player.player = gameObject;
		rigidbody = this.GetComponent<Rigidbody> ();
		if (rigidbody == null) {
			gameObject.AddComponent<Rigidbody> ();
			rigidbody = this.GetComponent<Rigidbody> ();
			rigidbody.useGravity = false;
		}
	}
	
	// Update is called once per frame
	void Update () {
		velo_input ();
		rigidbody.velocity = velocity*transform.forward*5;
		rot_input ();
	}

	void rot_input() {
		if (Input.GetKey (KeyCode.D)) {
			this.transform.Rotate (new Vector3 (0, rotation_speed * Time.deltaTime, 0));
		}
		if (Input.GetKey (KeyCode.A)) {
			this.transform.Rotate (new Vector3 (0, -rotation_speed * Time.deltaTime, 0));
		}
	}

	void velo_input(){
		if (Input.GetKey (KeyCode.W)) {
			velocity += accel*Time.deltaTime;
		}
		if (Input.GetKey (KeyCode.S)) {
			velocity -= accel*Time.deltaTime;
		}
		velocity = Mathf.Clamp (velocity, -1, 4);
	}
}
