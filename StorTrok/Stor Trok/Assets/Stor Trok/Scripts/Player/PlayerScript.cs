using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerScript : MonoBehaviour { //input des spielers


	public static PlayerScript playerScript;
	public Spaceship spaceship;

	//public float max_x_rotation = 60; // der spieler kann sich nicht senkrecht bewegen, sondern maximal im 60  bzw. -60 ° winkel

	public bool mobile_input = false; // handy/tablet steuerung

	public GameObject selected_enemy;

	new Rigidbody rigidbody;

	public WeaponRawTypes weapon_types = WeaponRawTypes.None; // mit welchen waffen der spieler schießt (torpedo oder phaser/pulsphaser)

	//public string player_data_path;// = Application.persistentDataPath + "/playerdata";

	void Awake () {
		PlayerScript.playerScript = this;
		this.enabled = true;
	//	player_data_path = Application.dataPath + "/Resources/playerdata.dat";
		//player_data_path = "/playerdata.dat";

		spaceship = this.GetComponent<Spaceship> ();
		if (spaceship == null) {
			gameObject.AddComponent<Spaceship> ();
			spaceship = this.GetComponent<Spaceship> ();
		}

		if (Player.player == null) {
			Player.player = new Player (PlayerEnvironmentStatus.Space);
		}
		Player.player.spaceship = spaceship;
		Player.player.setup_ship_items ();

		rigidbody = this.GetComponent<Rigidbody> ();
		if (rigidbody == null) {
			gameObject.AddComponent<Rigidbody> ();
			rigidbody = this.GetComponent<Rigidbody> ();
			rigidbody.useGravity = false;
		}
	}

	void Update () {
		movement_input ();

		attack_input ();
		attack_enemy ();

		//TODO Raumschiff wechseln
		/*if (Input.GetKeyDown (KeyCode.Space)) {
			GameObject k = GameObject.Instantiate (kling_warbird, transform.position, transform.rotation);
			if (k.GetComponent<PlayerScript> () == null) {
				k.AddComponent <PlayerScript> ();
			}
			Spaceship s = k.GetComponent < Spaceship > ();
			s.impuls_antrieb = this.spaceship.impuls_antrieb;
			s.schild_item = this.spaceship.schild_item;
			s.speed = spaceship.speed;
			s.apply_item_stats ();

			PlayerScript.playerScript = k.GetComponent<PlayerScript> ();
			Player.player.spaceship = s;
			Player.player.setup_weapons ();

			RaumschiffInventory.raumschiffInventory.set_buttons ();

			gameObject.SetActive (false);
			Destroy (this);
		}*/
	}

	public Spaceship get_enemy(){
		return Spaceship.get_spaceship (selected_enemy);
	}

	public void attack_enemy(){
		if (selected_enemy) {
			Spaceship s = Spaceship.get_spaceship (selected_enemy);
			if (s != null) {
				if (s.destroyed)
					return;
			} else {
				DestroyableObject d = selected_enemy.GetComponent<DestroyableObject> ();
				if (d.destroyed) {
					return;
				}
			}
			spaceship.attack_enemy (selected_enemy, Player.enemy_layer, weapon_types);
		}
	}

	public void attack_input(){
		float max_dist = 10000;
		if (Input.GetMouseButtonDown (1)) {
			Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
			RaycastHit hit;
			if (Physics.Raycast(ray, out hit, max_dist, 1<<Player.enemy_layer)) {
				selected_enemy = hit.collider.gameObject;
			}
		}
		if (Input.GetKeyDown (KeyCode.Tab)) {
			if (selected_enemy == null) {

				List<GameObject> gos = new List<GameObject> ();
				foreach (Spaceship s in Spaceship.active_spaceships) {
					if (s.gameObject.layer != Player.player_layer && !s.destroyed) {
						gos.Add (s.gameObject);
					}
				}
				foreach (DestroyableObject d in DestroyableObject.active_destroyable_objects) {
					if (d.gameObject.layer != Player.player_layer && !d.destroyed) {
						gos.Add (d.gameObject);
					}
				}

				if (gos.Count != 0) {
					GameObject nearest = gos [0];
					foreach (GameObject g in gos) {
						float dist = (g.transform.position - transform.position).magnitude;
						if (dist < (nearest.transform.position - transform.position).magnitude) {
							nearest = g;
						}
					}
					if ((nearest.transform.position - transform.position).magnitude <= max_dist) {
						selected_enemy = nearest;
					}

				}
			} else {
				selected_enemy = null;
			}
		}
	}

	void movement_input(){
		rot_input ();
		velo_input ();
		auto_navigation_input ();
	}

	void rot_input() {
		bool d = Input.GetKey (KeyCode.D);
		bool a = Input.GetKey (KeyCode.A);
		bool w = Input.GetKey (KeyCode.W);
		bool s = Input.GetKey (KeyCode.S);
		if (d) {
			spaceship.rotate_left();
		}
		if (a) {
			spaceship.rotate_right();
		}
		if (w) {
			spaceship.rotate_down();
		}
		if (s) {
			spaceship.rotate_up ();
		}
		/*if (mobile_input) {
			this.transform.Rotate (new Vector3 (0, spaceship.rotation_speed*Input.acceleration.x*Time.deltaTime*1.5f, 0));
		}*/
		if (d || a || w || s) {
			spaceship.abort_auto_navigation ();
		} else if (!spaceship.is_auto_navigating){
			spaceship.fix_z_rotation ();
		}
		//print (transform.localRotation.eulerAngles);

		//spaceship.fix_rotation ();


	}

	void velo_input(){
		float start_speed = spaceship.speed;
		if (Input.GetKey (KeyCode.E)) {
			spaceship.speed += spaceship.accel * Time.deltaTime;
		}
		if (Input.GetKey (KeyCode.Q)) {
			spaceship.speed -= spaceship.accel * Time.deltaTime;
		}
		/*if (mobile_input && Input.touchCount > 0) {	
			if (Input.GetTouch (0).position.x < Screen.width/2) {
				spaceship.speed -= spaceship.accel * Time.deltaTime;
			} else {
				spaceship.speed += spaceship.accel * Time.deltaTime;
			}
		}*/
		if (spaceship.speed != start_speed) {
			spaceship.abort_auto_navigation ();
		}

		spaceship.clamp_speed ();
	}
	void auto_navigation_input(){
		if (Input.GetKeyDown (KeyCode.R) && selected_enemy!=null) {
			spaceship.auto_navigate_to_object (selected_enemy);
		}
	}
}
