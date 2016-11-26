using UnityEngine;
using System.Collections;

public class CameraScript : MonoBehaviour {

	//public Vector3 offset = new Vector3 (0, 0, 0);
	public float camera_ortho_size = 30;
	public float camera_angle = 30;
	//public GameObject player;

	public GameObject camera_center;

	Camera camera;

	void Start () {
		camera = this.GetComponent<Camera> ();
		camera.orthographicSize = camera_ortho_size;
	
	}
	
	// Update is called once per frame
	void Update () {
		camera_center.transform.position = Player.player.transform.position;
		Vector3 r = camera_center.transform.localRotation.eulerAngles;
		Quaternion new_r = Quaternion.identity;
		new_r.eulerAngles = new Vector3 (30, r.y, r.z);
		camera_center.transform.localRotation = new_r;
		if (camera.orthographic) {
			transform.localPosition = new Vector3 (0, 0, this.camera_ortho_size * (-3));
		} else {
			transform.localPosition = new Vector3 (0, 0, -100);
		}

		/*transform.position = player.transform.position + offset;

		Quaternion idealRotation = Quaternion.LookRotation(player.transform.position-transform.position);
		transform.rotation = Quaternion.Slerp(transform.rotation, idealRotation, Time.timeScale);*/
	}
}
