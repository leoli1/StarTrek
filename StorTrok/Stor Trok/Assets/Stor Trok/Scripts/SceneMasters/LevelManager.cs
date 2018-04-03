using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class LevelManager : MonoBehaviour {

	public static PlayerData player_data;
	public static LevelManager levelManager;

	[HideInInspector] public bool level_has_mission;

	PlayerEnvironmentStatus environment_type;

	[HideInInspector] public string scene_name;

	public readonly float sektorenraum_scale_factor = 0.04f;

	[Header("Fade Scene Image")]
	public Image fade_scene_image;

	[Header("UI object")]
	public GameObject ui;

	[Header("Player spawnpoint")]
	public GameObject spawn_point;

	[Header("Ground/Raumstation only")]
	public Transform spaceship_spawn_point;
	public GameObject ground_player_object;

	[HideInInspector]
	public bool game_paused = false;

	[HideInInspector]
	public GameObject raumschiff_object;

	[HideInInspector]
	public GameObject player_gameObject;

	void Awake () {
		Cursor.lockState = CursorLockMode.None;
		Item.load_items ();
		GamePreferences.load ();

		if (Player.player == null) {
			new Player (PlayerEnvironmentStatus.None);
			Player.player.name = GamePreferences.game_preferences.last_username;
		}

		print ("data path: " + Application.dataPath + "/...");
		LevelManager.levelManager = this;

		PlayerMovementData.data_path = Application.dataPath + "/Resources/player_movement_data.dat";

		scene_name = UnityEngine.SceneManagement.SceneManager.GetActiveScene ().name;

		environment_type = PlayerEnvironmentStatus.Space;

		Mission m = GameObject.FindObjectOfType<Mission> ();
		if (m != null) {
			Invoke ("setup_mission", 0.01f);
			environment_type = m.environment_type;
			level_has_mission = true;

		} else {
			print ("NO MISSION");
			level_has_mission = false;
		}

		if (scene_name == "Shop") {
			environment_type = PlayerEnvironmentStatus.Ground;
		}


		load_player_data ();

		if (scene_name=="SektorenRaum")
			spawn_point.transform.position = player_data.sektorenraum_koordinaten.unity_vector3;

		switch (environment_type) {
		case PlayerEnvironmentStatus.Space:
			create_player_space ();
			player_gameObject = Player.player.spaceship.gameObject;
			break;
		case PlayerEnvironmentStatus.Ground:
			GameObject g = GameObject.Find ("StandardCamera");
			g.SetActive (false);
			create_player_ground ();
			player_gameObject = ground_player_object;
			break;
		}
		if (Player.player!=null)
			Player.player.player_environment_status = environment_type;
		
		ui.SetActive (true);
		UI.ui.enableStandardUI ();
		UI.ui.set_environment_ui (environment_type);

		Minimap.minimap.check_objects ();

		fade_scene_image.gameObject.SetActive (false);
	}

	public static void load_player_data(){
		LevelManager.player_data = SaveData.ReadFromFile<PlayerData>(Player.player.player_data_path);
		Player.player.money = LevelManager.player_data.money;
	}

	void setup_mission() {
		Mission.current_mission.load_new_mission_part (Mission.current_mission.fullMission.first_mission_part, true);
	}

	public void create_player_space(){
		GameObject p = GameObject.Instantiate (Raumschiff.get_raumschiff_by_id (player_data.current_spaceship).spaceship_object, spawn_point.transform.position, spawn_point.transform.rotation);
		//GameObject p = GameObject.Instantiate (((Raumschiff)Item.get_item_by_id (player_data.current_spaceship)).spaceship_object, spawn_point.transform.position, Quaternion.identity);
		Spaceship s = Spaceship.get_spaceship(p);
		s.set_layer (Player.player_layer);
		if (scene_name == "SektorenRaum") {
			s.antrieb_type = AntriebType.Warp;
			s.apply_item_stats ();
			p.transform.localScale = p.transform.localScale * sektorenraum_scale_factor;
			foreach (Light light in p.GetComponentsInChildren<Light>()) {
				//light.intensity *= sektorenraum_scale_factor;
				light.range *= sektorenraum_scale_factor;
			}
			Camera.main.GetComponent<CameraScript> ().camera_center.transform.localScale = Camera.main.GetComponent<CameraScript> ().camera_center.transform.localScale * sektorenraum_scale_factor;
		}
			//Spaceship.get_spaceship (p).raumschiff = Raumschiff.get_raumschiff_by_id (player_data.current_spaceship);
		p.AddComponent<PlayerScript> ();

		foreach (LearnPlayerMovementData l in GameObject.FindObjectsOfType<LearnPlayerMovementData>()) {
			l.learn_object = p;
		}
	}
	public void create_player_ground(){
		Player.player.player_environment_status = PlayerEnvironmentStatus.Ground;
		spawn_spaceship ();
	}

	public void spawn_spaceship(){
		raumschiff_object = GameObject.Instantiate (Raumschiff.get_raumschiff_by_id (player_data.current_spaceship).spaceship_object, spaceship_spawn_point.transform.position, spaceship_spawn_point.transform.rotation);

		Player.player.spaceship = Spaceship.get_spaceship(raumschiff_object);
		Player.player.setup_ship_items ();
		Player.player.spaceship.shields.SetActive (false);
	}

	public static void save_player_data(bool set_ids){
		if (set_ids)
			player_data.set_ids ();

		SaveData.SaveToFile<PlayerData> (Player.player.player_data_path, player_data);
	}
	public static void save_player_data(){
		save_player_data (true);
	}

	public void load_new_scene(string scene_name){
		//save_player_data ();
		if (Player.player.spaceship!=null){
			Player.player.spaceship.set_target_speed (0);
			if (PlayerScript.playerScript!=null)
				PlayerScript.playerScript.enabled = false;
		}
		StartCoroutine(_load_scene(scene_name));
	}

	IEnumerator _load_scene(string scene_name){
		yield return scene_transition_animation ();
		save_player_data ();
		SceneManager.LoadScene (scene_name);
		yield return null;
	}

	IEnumerator scene_transition_animation(){
		fade_scene_image.gameObject.SetActive (true);
		fade_scene_image.color = new Color (0, 0, 0, 0);
		fade_scene_image.rectTransform.sizeDelta = new Vector2 (Screen.width, Screen.height);
		while (true) {
			//float step = Time.deltaTime * 1.2f;
			float step = 0.02f;
			if (Mathf.Abs (fade_scene_image.color.a - 1) <= step) {
				break;
			}
			fade_scene_image.color = new Color (0, 0, 0, fade_scene_image.color.a + step);
			yield return null;
		}
	}

	void OnDisable(){
		save_player_data ();
	}

	void Update () {
		if (Input.GetKeyDown (KeyCode.Escape)) {
			set_pause_state (!UI.ui.pause_menu.gameObject.activeSelf);
		}
	}

	public void set_pause_state(bool state){
		UI.ui.pause_menu.gameObject.SetActive (state);
		Time.timeScale = state ? 0 : 1;
		game_paused = state;
	}
}