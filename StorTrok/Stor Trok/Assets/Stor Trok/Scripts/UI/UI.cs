using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public struct UISpaceshipStats{
	public GameObject stats_parent;
	public Text structural_integ_text;
	public Image shield_part_top;
	public Image shield_part_left;
	public Image shield_part_bot;
	public Image shield_part_right;
}

public class UI : MonoBehaviour {

	public static UI ui;

	[Tooltip("Der große Text mit dem Namen der Mission, der am Anfang, wenn der Fokus der Camera auf dem Gegner liegt, erscheint")]
	public Text mission_title;
	[Tooltip("Das GO, welches die UI für den Respawn-Timer bzw. -Button enthält")]
	public GameObject death_timer;

	public Item selected_item;

	[Header("Selected Item")]
	public GameObject selected_item_go;
	public Text selected_item_mark_number_text;

	[Header("Selected Enemy")]
	public Sprite selected_enemy_tex;
	public Image selected_enemy_marker;
	public Image selected_enemy_image_bg;

	[Header("Different UI-Parents for different environment types")]
	public GameObject spaceUI;
	public GameObject groundUI;
	public GameObject otherUI;

	//public GameObject Minimap;<

	[Header("Pause Menu")]
	public PauseMenu pause_menu;

	[Header("Help Menu")]
	public GameObject help_object;

	[Header("Player Stats UI")]
	public GameObject speedometer;
	public UISpaceshipStats player_spaceship_ui;
	public Image ent_image;

	[Header("Enemy Stats UI")]
	public UISpaceshipStats enemy_spaceship_ui;

	[Header("Inventar UI")]
	public GameObject inventar;
	public GameObject raumschiff_inventar;

	[Header("Item Description")]
	public Image item_description_text_bg;
	public Text item_description_text;

	private bool inventory_open = false;

	//public bool item_description_text_active = false;

	public GameObject interactiveMessageObject;

	public static InteractiveMessage interactiveMessage;

	public static float ui_size_ratio;


	private bool standard_ui_enabled = false;

	void Awake () {
		//ui_size_ratio = Screen.width / 1000f / ((float)Screen.height/Screen.width);
		ui_size_ratio = Mathf.Min(Screen.height/800f, 1.4f);

		interactiveMessage = interactiveMessageObject.GetComponent<InteractiveMessage> ();
		UI.ui = this;
		if (LevelManager.player_data == null) {
			gameObject.SetActive (false);
			return;
		}
		if (LevelManager.player_data.selected_item != 0) {
			selected_item = Item.get_item_by_id (LevelManager.player_data.selected_item);
		} else {
			selected_item = null;
		}

		Vector2 size = new Vector2 (Screen.width, Screen.height);
		spaceUI.GetComponent<RectTransform> ().sizeDelta = size;
		groundUI.GetComponent<RectTransform> ().sizeDelta = size;
		otherUI.GetComponent<RectTransform> ().sizeDelta = size;

		if (!selected_enemy_marker.sprite) {
			selected_enemy_marker.sprite = selected_enemy_tex;
		}

		player_spaceship_ui.stats_parent.transform.position = new Vector3 (Screen.width * 0.5f, 0, 0);

		ent_image.transform.localPosition = new Vector3(-20, 50, 0);

		player_spaceship_ui.stats_parent.transform.localScale = player_spaceship_ui.stats_parent.transform.localScale * ui_size_ratio;
		
		speedometer.transform.localPosition = new Vector3 (-200, 50, 0);

		selected_enemy_image_bg.rectTransform.position = new Vector3 (Screen.width * 0.5f, Screen.height * 0.9f, 0);
		selected_enemy_image_bg.rectTransform.localScale = selected_enemy_image_bg.rectTransform.localScale * ui_size_ratio;

		selected_enemy_marker.rectTransform.sizeDelta = new Vector2(100,100);
		selected_enemy_marker.rectTransform.localScale = selected_enemy_marker.rectTransform.localScale * ui_size_ratio;

		pause_menu.gameObject.SetActive (false);
		pause_menu.transform.localScale = pause_menu.transform.localScale * ui_size_ratio;

		help_object.gameObject.SetActive (false);
	//	help_object.transform.localScale = help_object.transform.localScale * ui_size_ratio;
		help_object.transform.position = new Vector3 (Screen.width * 0.5f, Screen.height, 0);

		mission_title.transform.localScale = mission_title.transform.localScale * ui_size_ratio;
	
	}
	void OnEnable(){
		UI.ui = this;
	}

	public void enableUI(){
		gameObject.SetActive (true);
	}
	public void disableUI(){
		gameObject.SetActive (false);
	}
	public void disableStandardUI(){
		player_spaceship_ui.stats_parent.SetActive(false);
		interactiveMessage.gameObject.SetActive (false);
		selected_enemy_marker.gameObject.SetActive(false);
		selected_enemy_image_bg.gameObject.SetActive (false);
		if (!(LevelManager.levelManager.scene_name == "Shop"))
			Minimap.minimap.gameObject.SetActive (false);

		standard_ui_enabled = false;
	}
	public void enableStandardUI(){
		player_spaceship_ui.stats_parent.SetActive(true);
		interactiveMessage.gameObject.SetActive (true);
		selected_enemy_marker.gameObject.SetActive(true);
		selected_enemy_image_bg.gameObject.SetActive (true);
		if (!(LevelManager.levelManager.scene_name == "Shop"))
			Minimap.minimap.gameObject.SetActive (true);

		standard_ui_enabled = true;
	}

	public void enemy_ship_stats_update(){
		bool enemy_stats_enabled = PlayerScript.playerScript.selected_enemy != null && standard_ui_enabled;
		selected_enemy_marker.gameObject.SetActive(enemy_stats_enabled);
		selected_enemy_image_bg.gameObject.SetActive (enemy_stats_enabled);
		if (PlayerScript.playerScript.selected_enemy) {
			Vector3 screen_pos = Camera.main.WorldToScreenPoint (PlayerScript.playerScript.selected_enemy.transform.position);
			if (screen_pos.z < 0) {
				selected_enemy_marker.gameObject.SetActive (false);
			}
			screen_pos.z = 0;
			selected_enemy_marker.transform.position = screen_pos;

			/*enemy_spaceship_ui.stats_parent.transform.localPosition = new Vector3(0, 150, 0);
			Vector3 pos = enemy_spaceship_ui.stats_parent.transform.position;
			enemy_spaceship_ui.stats_parent.transform.position = new Vector3 (Mathf.Clamp(pos.x, 100, Screen.width-100), Mathf.Clamp (pos.y, 100, Screen.height-100), 0);*/

			if (Utils.is_spaceship (PlayerScript.playerScript.selected_enemy)) {
				Spaceship sp = Spaceship.get_spaceship (PlayerScript.playerScript.selected_enemy);
				set_ship_stats (enemy_spaceship_ui, sp);
			} else if (Utils.is_destroyable_object (PlayerScript.playerScript.selected_enemy)) {
				DestroyableObject d = PlayerScript.playerScript.selected_enemy.GetComponent<DestroyableObject> ();
				set_destroyable_object_stats (enemy_spaceship_ui, d);
			}
		
		}

	}

	public void player_ship_stats_update(){
		Spaceship sp = Player.player.spaceship;
		set_ship_stats (player_spaceship_ui, sp);
	}

	public void set_ship_stats(UISpaceshipStats ui_ship, Spaceship sp){
		float p = sp.schild.max_shield_power_per_quarter;
		ui_ship.shield_part_top.color = sp.get_shield_color (sp.schild.get_shield_intensity_at_part (SchildPartTypes.Top)/p);
		ui_ship.shield_part_left.color = sp.get_shield_color (sp.schild.get_shield_intensity_at_part (SchildPartTypes.Left)/p);
		ui_ship.shield_part_bot.color = sp.get_shield_color (sp.schild.get_shield_intensity_at_part (SchildPartTypes.Bot)/p);
		ui_ship.shield_part_right.color = sp.get_shield_color (sp.schild.get_shield_intensity_at_part (SchildPartTypes.Right)/p);
		string t = (Mathf.Round (sp.structural_integrity / sp.max_structural_integrity * 100)).ToString ();
		ui_ship.structural_integ_text.text = ((t == "0" && sp.structural_integrity!=0) ? "1" : t) +"%";

		ui_ship.shield_part_top.enabled = true;
		ui_ship.shield_part_left.enabled = true;
		ui_ship.shield_part_bot.enabled = true;
		ui_ship.shield_part_right.enabled = true;
	}
	public void set_destroyable_object_stats(UISpaceshipStats ui_ship, DestroyableObject d){
		string t = (Mathf.Round(d.hitpoints / d.max_hitpoints * 100)).ToString();
		ui_ship.structural_integ_text.text = (t == "0" ? "1" : t) +"%";

		ui_ship.shield_part_top.enabled = false;
		ui_ship.shield_part_left.enabled = false;
		ui_ship.shield_part_bot.enabled = false;
		ui_ship.shield_part_right.enabled = false;

	}

	public void set_environment_ui(PlayerEnvironmentStatus pes){
		spaceUI.SetActive (false);
		groundUI.SetActive (false);
		switch (pes) {
		case PlayerEnvironmentStatus.Space:
			spaceUI.SetActive (true);
			break;
		case PlayerEnvironmentStatus.Ground:
			groundUI.SetActive (true);
			break;
		}	
	}

	void update_space_ui(){
		enemy_ship_stats_update ();
		player_ship_stats_update ();
	}
	void enemy_ground_stats_update(){
		//selected_enemy_img.gameObject.SetActive(PlayerScript.playerScript.selected_enemy!=null && standard_ui_enabled);
		selected_enemy_marker.gameObject.SetActive(false);// && standard_ui_enabled);
		selected_enemy_image_bg.gameObject.SetActive(false);
		/*if (..selected_enemy) {

		}*/
	}
	void update_ground_ui(){
		enemy_ground_stats_update ();
	}

	void Update () {
		Player p = Player.player;
		if (p == null) {
			return;
		}

		input ();

		inventar.SetActive (inventory_open);
		raumschiff_inventar.SetActive (inventory_open);

		//item_description_text.rectTransform.position = Input.mousePosition+new Vector3(10,-10,0);
		item_description_text_bg.rectTransform.position = Input.mousePosition+new Vector3(10, -10, 0);

		selected_item_go.SetActive (selected_item != null && inventory_open);
		//selected_item_go.SetActive (false);
		if (selected_item != null) {
			selected_item_go.GetComponent<RectTransform> ().position = Input.mousePosition+new Vector3(20,-20,0);
			selected_item_go.GetComponent<Image> ().sprite = selected_item.inventory_tex;
			try{
				selected_item_mark_number_text.text = Utils.get_mark_text (((Weapon)selected_item).mark_number);
			} catch(System.InvalidCastException){ //falls selected item keine weapon ist
			}
		}

		switch (p.player_environment_status){
		case PlayerEnvironmentStatus.Space:
			update_space_ui ();
			break;
		case PlayerEnvironmentStatus.Ground:
			update_ground_ui ();
			break;
		}
	}
	void input(){
		if (Input.GetKeyDown (KeyCode.I)) {
			inventory_open = !inventory_open;
			if (!inventory_open)
				//item_description_text.gameObject.SetActive (false);
				item_description_text_bg.gameObject.SetActive(false);
		}
	}
}
