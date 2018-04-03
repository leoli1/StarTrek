using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StartSceneMaster : MonoBehaviour {

	public InputField user_name_inputfield;

	public GameObject load_button;
	public GameObject create_button;
	public GameObject engage_button;

	public Text money_text;
	private GameObject money_panel;
	public Text name_text;
	private GameObject name_panel;

	private PlayerData playerData;

	void Start () {
		Item.load_items ();
		GamePreferences.load ();

		user_name_inputfield.text = GamePreferences.game_preferences.last_username;

		PlayerMovementData.data_path = Application.dataPath + "/Resources/player_movement_data.dat";

		engage_button.SetActive(false);

		money_panel = money_text.GetComponentInParent<Image> ().gameObject;
		name_panel = name_text.GetComponentInParent<Image> ().gameObject;

		money_panel.SetActive (false);
		name_panel.SetActive (false);
	}
	void load_ship(){
		GameObject g = GameObject.Instantiate (Raumschiff.get_raumschiff_by_id (playerData.current_spaceship).spaceship_object, new Vector3(), Quaternion.identity);

		Camera.main.transform.LookAt (g.transform);
		g.GetComponent<Spaceship> ().enabled = false;
		g.GetComponent<Spaceship> ().shields.gameObject.SetActive (false);
		g.GetComponent<Spaceship> ().schild_collision_object.gameObject.SetActive (false);
	}

	public void on_load_button(){
		string name = user_name_inputfield.text;
		string path = Application.dataPath + "/Resources/pdata_" + name + ".dat";

		if (!SaveData.file_exists (path))
			return;

		playerData = SaveData.ReadFromFile<PlayerData> (path);
		load_player (name);
	}

	public void on_create_button(){
		string name = user_name_inputfield.text;
		if (name == "")
			return;
		
		string path = Application.dataPath + "/Resources/pdata_" + name + ".dat";

		if (SaveData.file_exists (path))
			return;

		playerData = PlayerData.new_player (2);
		SaveData.SaveToFile<PlayerData> (path, playerData);
		load_player (name);
	}

	public void on_engage_button(){
		SceneManager.LoadScene ("SektorenRaum");
	}

	void load_player(string name){
		new Player (PlayerEnvironmentStatus.None);
		Player.player.name = name;
		Player.player.money = playerData.money;

		GamePreferences.game_preferences.last_username = name;

		user_name_inputfield.gameObject.SetActive (false);
		create_button.SetActive(false);
		load_button.SetActive(false);

		engage_button.SetActive(true);
		money_panel.SetActive (true);
		name_panel.SetActive (true);

		money_text.text = Player.player.money + "$";
		name_text.text = Player.player.name;
	
		load_ship ();
		GamePreferences.save ();
	}
}
