using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Minimap : MonoBehaviour {

	private static Minimap minimap_inst;

	[Header("Minimap Icons")]
	public Texture enemy;
	public Texture player;
	public Texture ally;

	public GameObject minimap_object;

	public const float minimap_object_size = 20;

	private List<MinimapObject> minimap_objects;

	public LineRenderer player_attack_range;
	public const int number_of_attack_range_points = 40;

	[HideInInspector]
	public Transform minimap_camera;


	public Transform minimap_camera_range;
	private LineRenderer camera_range_lr;
	public const float camera_range_length = 1000;
	private Camera main_camera;

	public static Minimap minimap{
		get { 
			if (minimap_inst != null)
				return minimap_inst;
			return FindObjectOfType<Minimap> ();
		}
		set{ 
			minimap_inst = value;
		}
	}

	void Start () {
		Minimap.minimap = this;
		if (LevelManager.levelManager.scene_name == "Shop")
			gameObject.SetActive (false);

		if (minimap_camera == null) {
			minimap_camera = GetComponentInChildren<Camera> ().transform;
		}
		float pos = Screen.width * 0.03f;
		float size = Screen.height * 0.2f;
		minimap_camera.GetComponent<Camera> ().pixelRect = new Rect (pos, pos, size, size);

		camera_range_lr = minimap_camera_range.GetComponent<LineRenderer> ();
		main_camera = Camera.main;

		camera_range_lr.positionCount = 3;

		player_attack_range.positionCount = number_of_attack_range_points;
		float step = 2 * Mathf.PI / number_of_attack_range_points;
		int i = 0;
		for (float a = 0; a < 2 * Mathf.PI; a += step) {
			float x = Spaceship.max_attack_distance * Mathf.Cos (a);
			float y = Spaceship.max_attack_distance * Mathf.Sin (a);

			player_attack_range.SetPosition (i, new Vector3 (x, 0, y));
			i++;
		}
	}
	
	public void check_objects(){
		List<MinimapObject> minimap_objects = MinimapObject.all_minimap_objects;

		List<Spaceship> spaceships = Spaceship.active_spaceships;
		foreach (Spaceship s in spaceships) {
			if (!s.minimap_icon_created) {
				create_minimap_icon (s);
			}
		}

		List<DestroyableObject> objects = DestroyableObject.active_destroyable_objects;
		foreach (DestroyableObject d in objects) {
			if (!d.minimap_icon_created) {
				create_minimap_icon (d);
			}
		}


		List<MinimapObject> obj_to_rem = new List<MinimapObject> ();
		foreach (MinimapObject m in minimap_objects) {
			if (m == null) {
				obj_to_rem.Add (m);
				continue;
			}
			GameObject g = m.object_to_represent.gameObject;
			Spaceship s = Spaceship.get_spaceship (g);
			if (s != null) {
				if (s.destroyed || !s.gameObject.activeInHierarchy) {
					obj_to_rem.Add (m);
				}
			} else {
				DestroyableObject d = DestroyableObject.get_destroyable_object (g);
				if (d != null) {
					if (d.destroyed || !d.gameObject.activeInHierarchy) {
						obj_to_rem.Add (m);
					}
				}
			}
		}
		foreach (MinimapObject m in obj_to_rem) {
			MinimapObject.all_minimap_objects.Remove(m);
			if (m != null) {
				GameObject.Destroy (m.gameObject);
			}
		}

	}
	void create_minimap_icon(Spaceship s){
		GameObject g = GameObject.Instantiate (minimap_object, transform);

		MinimapObject m = g.GetComponent<MinimapObject> ();
		m.object_to_represent = s.transform;

		MeshRenderer rend = g.GetComponent<MeshRenderer> ();
		if (Player.player.spaceship == s) {
			rend.material.mainTexture = player;
			g.transform.position = Vector3.up;

			//player_attack_range.transform.SetParent (g.transform);

		} else {
			rend.material.mainTexture = (s.spaceship_group_type == ObjectGroupType.Enemy) ? enemy : player;
		}
		s.minimap_icon_created = true;
	}
	void create_minimap_icon(DestroyableObject d){
		GameObject g = GameObject.Instantiate (minimap_object, transform);

		MinimapObject m = g.GetComponent<MinimapObject> ();
		m.object_to_represent = d.transform;

		MeshRenderer rend = g.GetComponent<MeshRenderer> ();
		rend.material.mainTexture = d.object_group_type == ObjectGroupType.Enemy ? enemy : ally;//(s.spaceship_group_type == ObjectGroupType.Enemy) ? ItemImages.item_images.enemy : ItemImages.item_images.player;
		d.minimap_icon_created = true;
	}

	void Update () {
		Vector3 player_position = Player.player.spaceship.transform.position;
		minimap_camera.position = new Vector3 (player_position.x, 10000, player_position.z);

		/*float half_fov = 0.5f*(main_camera.fieldOfView*(main_camera.pixelWidth/(float)main_camera.pixelHeight))*Mathf.Deg2Rad;//-(main_camera.fieldOfView / 2)*Mathf.Deg2Rad;
		float cam_left = -half_fov;
		float cam_right = half_fov;//cam_left + main_camera.fieldOfView*Mathf.Deg2Rad;
		Vector3 left_point = new Vector3 (Mathf.Sin (cam_left) * camera_range_length, 0, Mathf.Cos (cam_left) * camera_range_length);
		Vector3 right_point = new Vector3 (Mathf.Sin (cam_right) * camera_range_length, 0, Mathf.Cos (cam_right) * camera_range_length);
		camera_range_lr.positionCount = 3;
		camera_range_lr.SetPositions(new Vector3[]{
			left_point,
			Vector3.zero,
			right_point
		});*/
		//Vector3 left_point = main_camera.ViewportToWorldPoint (new Vector3 (0, 0.5f, main_camera.farClipPlane));//.normalized*camera_range_length;
		//Vector3 right_point = main_camera.ViewportToWorldPoint (new Vector3 (1, 0.5f, main_camera.farClipPlane));//;.normalized*camera_range_length;+
		Vector3 left_world_point = main_camera.ViewportToWorldPoint (new Vector3 (0, 0.5f, 500))-main_camera.transform.position;
		left_world_point.y = 0;
		Vector3 left_point = left_world_point.normalized*camera_range_length+main_camera.transform.position;
		Vector3 right_world_point = main_camera.ViewportToWorldPoint (new Vector3 (1, 0.5f, 500))-main_camera.transform.position;
		right_world_point.y = 0;
		Vector3 right_point = right_world_point.normalized*camera_range_length+main_camera.transform.position;
		camera_range_lr.SetPositions(new Vector3[]{
			left_point,
			main_camera.transform.position,
			right_point
		});
		if (Player.player.player_environment_status == PlayerEnvironmentStatus.Space) {
			if (PlayerScript.playerScript.selected_enemy != null) {
				player_attack_range.transform.position = Player.player.spaceship.transform.position;
				player_attack_range.enabled = true;
			} else {
				player_attack_range.enabled = false;
			}
		}
	}
}
