using UnityEngine;
using System.Collections;

public class CameraScript : MonoBehaviour {

	public static CameraScript camera_script;

	public float camera_ortho_size = 30; // die größe des blickfeldes, falls die camera im orthorgraphic-modus ist
	public float camera_angle = 30; // winkel zwischen camera, spieler und er x-z ebene, ebenfalls im orthographic-modus

	public float mouse_x_sensitivity = 2; // wie stark die maus die camera in x- bzw.  in y-richtung beeinflusst
	public float mouse_y_sensitivity = 2;

	public GameObject camera_center; // punkt, um den sich die camera dreht, standartmäßig der spieler

	public bool lookat_player_and_controllable = true;

	public GameObject lookat_object;

	private float max_zoom = 4;
	private float min_zoom = 0.5f;

	new Camera camera; // Camera component

	void Awake(){
		camera_script = this;
	}
	void Start () {
		camera = this.GetComponent<Camera> ();
		camera.orthographicSize = camera_ortho_size;

		if (LevelManager.levelManager.scene_name == "SektorenRaum") {
			max_zoom *= LevelManager.levelManager.sektorenraum_scale_factor;
			min_zoom *= LevelManager.levelManager.sektorenraum_scale_factor;
		}
	}
	
	// Update is called once per frame
	void Update () {
		float lfactor = 0;
		Vector3 pos_pre = transform.position;
		Quaternion rot_pre = transform.rotation;
		if (lookat_player_and_controllable) {
			lfactor = 1;
			camera_center.transform.position = PlayerScript.playerScript.transform.position;


			// anpassung der rotation, des camera-zentrums, wobei x,y,z die neuen rotations-werte sind
			Vector3 r = camera_center.transform.localRotation.eulerAngles;
			Quaternion new_r = Quaternion.identity;
			float x = r.x;
			x = Utils.clamp_angle (x, -70, 70);
			float y = r.y;
			if (Input.GetMouseButton (0)) {
				y += Input.GetAxis ("Mouse X") * mouse_x_sensitivity;
				x -= Input.GetAxis ("Mouse Y") * mouse_y_sensitivity;
			}
			float z = r.z;
			new_r.eulerAngles = new Vector3 (x, y, z);
			camera_center.transform.localRotation = new_r;

			//camera zoom
			float m = Input.GetAxis ("Mouse ScrollWheel");
			camera_center.transform.localScale *= m > 0 && camera_center.transform.localScale.magnitude < max_zoom ? 1.1f : m < 0 && camera_center.transform.localScale.magnitude > min_zoom ? 0.9f : 1;

			if (camera.orthographic) {
				transform.localPosition = new Vector3 (0, 0, this.camera_ortho_size * (-3));
			} else {
				transform.localPosition = new Vector3 (0, 0, -100);
			}
		} else if (lookat_object != null) {
			lfactor = 0.75f;
			this.transform.LookAt (lookat_object.transform);
		}
		Vector3 pos_after = transform.position;
		Quaternion rot_after = transform.rotation;
		transform.position = pos_pre;
		transform.rotation = rot_pre;
		lerp_to_posrot (pos_after, rot_after, lfactor);
	}

	public void lookat_point(Vector3 point){
		camera_center.transform.LookAt (point);
	}

	void lerp_to_posrot(Vector3 pos, Quaternion rot, float lerpfactor){
		transform.position = Vector3.Lerp (transform.position, pos, lerpfactor);
		transform.rotation = Quaternion.Lerp (transform.rotation, rot, lerpfactor);
	}
		
}
