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
		if (Input.GetKey (KeyCode.W)) {
			this.transform.Rotate (new Vector3 (rotation_speed * Time.deltaTime, 0, 0));
		}
		if (Input.GetKey (KeyCode.S)) {
			this.transform.Rotate (new Vector3 (-rotation_speed * Time.deltaTime, 0, 0));
		}
		Quaternion rot = transform.localRotation;
		//rot.eulerAngles = new Vector3 (Mathf.Clamp (rot.eulerAngles.x, -70, 70), rot.eulerAngles.y, rot.eulerAngles.z);
		//rot.eulerAngles = new Vector3(rot.eulerAngles.x, rot.eulerAngles.y, Mathf.Lerp(rot.eulerAngles.z,0,0.5f));
		float z = rot.eulerAngles.z;
		if (Mathf.Abs (z - 360) < Mathf.Abs (z)) {
			z = Mathf.Lerp (rot.eulerAngles.z, 360, 0.5f);
		} else {
			z = Mathf.Lerp (rot.eulerAngles.z, 0, 0.5f);
		}
		rot.eulerAngles = new Vector3 (rot.eulerAngles.x, rot.eulerAngles.y, z);
		transform.localRotation = rot;
	}

	void velo_input(){
		if (Input.GetKey (KeyCode.E)) {
			velocity += accel*Time.deltaTime;
		}
		if (Input.GetKey (KeyCode.Q)) {
			velocity -= accel*Time.deltaTime;
		}
		velocity = Mathf.Clamp (velocity, -1, 4);
	}
}
