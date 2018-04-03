using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public enum InteractiveMessageContentType{
	None,
	LoadScene,
	ItemDrop,
	StartMission,
	QuitMission,
}
	
public class InteractiveMessageButton{
	public InteractiveMessageContentType content_type;
	public string description_text;
	public Button button;
	public Text button_text;
	public Drop drop;
	public bool is_mission = false;

//	public System.Action<InteractiveMessageButton> action;

	public InteractiveMessageButton(){
		GameObject g = GameObject.Instantiate (OtherPrefabObjects.otherPrefabObjects.interactive_message_button);
		button = g.GetComponent<Button> ();
		button_text = g.GetComponentInChildren<Text> ();
	}
}

public class InteractiveMessage : MonoBehaviour {

	public Text description_text;
	public List<InteractiveMessageButton> buttons = new List<InteractiveMessageButton>();
	public GameObject image;

	void Awake(){
		foreach (InteractiveMessageButton button in buttons) {
			Destroy (button.button.gameObject);
		}
		buttons = new List<InteractiveMessageButton> ();
		ObjectAura.objectAurasPlayerIsIn = new List<ObjectAura> ();
	}

	void OnEnable () {
		if (buttons==null)
			buttons = new List<InteractiveMessageButton> ();
		UI.interactiveMessage = this;
		//if (this.content_type == InteractiveMessageContentType.None) {
		if (buttons.Count==0){
			gameObject.SetActive (false);
		}
		this.GetComponent<RectTransform> ().sizeDelta = new Vector2 (Screen.width, Screen.height);
	}
	void clean_buttons(InteractiveMessageButton new_button){
		if (new_button.content_type == InteractiveMessageContentType.StartMission || new_button.content_type == InteractiveMessageContentType.QuitMission) {
			int i = 0;
			while (i < buttons.Count) {
				if (buttons [i].content_type == InteractiveMessageContentType.StartMission && buttons[i]!=new_button) {
					remove_button (buttons [i]);
					i--;
				}
				i++;
			}
		}
	}
	void set_description_text(){
		string t = "";
		foreach (InteractiveMessageButton imb in buttons) {
			if (imb.description_text == "")
				continue;
			t += imb.description_text+"\n";
		}
		description_text.text = t;
	}
	void set_buttons_positions(){
		RectTransform r = image.GetComponent<RectTransform> ();
		float y_step = buttons.Count != 0 ? buttons [0].button.GetComponent<RectTransform> ().sizeDelta.y + 3 : 0;
		for (int i = 0; i < buttons.Count; i++) {
			float y_coord = (buttons.Count-i-1) * y_step;
			buttons [i].button.GetComponent<RectTransform> ().localPosition = new Vector3 (-r.sizeDelta.x/2, y_coord-r.sizeDelta.y/2, 0);
		}
	}
	void update_buttons_indices(){
		for (int i = 0; i < buttons.Count; i++) {
			int _i = i;
			buttons [i].button.onClick.RemoveAllListeners ();
			buttons[i].button.onClick.AddListener(()=>button_clicked(_i));
		} 
	}	
	void update_buttons(){
		set_description_text ();
		set_buttons_positions ();
		update_buttons_indices ();
	}

	public void remove_button(InteractiveMessageButton btn) {
		buttons.Remove (btn);
		update_buttons ();
		Destroy (btn.button.gameObject);

	}

	public void enable(InteractiveMessageContentType type){

		InteractiveMessageButton new_button = new InteractiveMessageButton();
		new_button.button.transform.SetParent (image.transform);
		new_button.content_type = type;

		switch(new_button.content_type){
		case InteractiveMessageContentType.LoadScene:
			ObjectAura oa = null;
			bool aura_found = false;
			foreach (ObjectAura a in ObjectAura.objectAurasPlayerIsIn) {
				if (a.objectAuraType != ObjectAuraType.ItemDrop) {
					aura_found = true;
					oa = a;
					break;
				}
			}
			if (!aura_found) {
				print ("no load scene Object aura");
				return;
			}
				
			oa.interactive_message_buttons.Add(new_button);
			switch (oa.objectAuraType) {
			case ObjectAuraType.PlanetSystem:
				PlanetSystem ps = oa.GetComponentInParent<PlanetSystem> ();
				if (ps.system_data.only_mission) {
					MissionData md = ps.mission_data;
					if (md != null) {
						new_button.button_text.text = "Starte Mission: " + md.title;
						new_button.description_text = md.description;
						new_button.is_mission = true;
					} else {
						print ("Error: " + ps.system_data.system_name + " has no mission data but is marked as only_mission");
					}
				} else {
					new_button.description_text = ps.system_data.description;
					new_button.button_text.text = "Betrete " + ps.system_data.system_name;
					MissionData md = ps.mission_data;
					if (md != null) {
						
						InteractiveMessageButton new_button2 = new InteractiveMessageButton ();
						new_button2.button.transform.SetParent (image.transform);
						new_button2.content_type = type;
						new_button2.is_mission = true;

						new_button2.button.gameObject.SetActive (true);
						new_button2.button_text.text = "Starte Mission: " + md.title;
						new_button2.description_text = md.description;
						buttons.Add (new_button2);
						oa.interactive_message_buttons.Add (new_button2);
					}
				}
				oa.interactive_message_generated = true;
				break;
			case ObjectAuraType.ErdeRaumdock:
				new_button.description_text = "Raumdock der Erde";
				new_button.button_text.text = "Andocken";
				oa.interactive_message_generated = true;
				break;
			case ObjectAuraType.ShopExit:
				new_button.description_text = "Zum Sektorenraum";
				new_button.button_text.text = "Verlassen";
				oa.interactive_message_generated = true;
				break;
			}
			break;
		case InteractiveMessageContentType.ItemDrop:
			ObjectAura oa_item = null;
			foreach (ObjectAura a in ObjectAura.objectAurasPlayerIsIn) {
				if (a.objectAuraType == ObjectAuraType.ItemDrop && !a.interactive_message_generated) {
					oa_item = a;
					break;
				} else if (a.objectAuraType == ObjectAuraType.ItemDrop) {
					print ("item drop aura but button already generated: "+a.gameObject.name);
				}
			}

			if (oa_item == null) {
				print ("no item object aura in Object auras");

				Destroy (new_button.button.gameObject);
				return;
			}

			Drop drop = oa_item.gameObject.GetComponent<Drop> ();
			if (drop == null) {
				print ("Error: no drop script on that object");
				return;
			}
			new_button.drop = drop;

			Item item = drop.item;
			if (Utils.item_is_null(item)) {
				print ("Error: no item in drop script");
				return;
			}

			oa_item.interactive_message_generated = true;
			oa_item.interactive_message_buttons.Add(new_button);

			new_button.description_text = item.name;//get_description_text ();
			new_button.button_text.text = "Aufnehmen";
			break;
		case InteractiveMessageContentType.StartMission:
			new_button.description_text = Mission.current_mission.fullMission.get_current_mission_part().description;
			new_button.button_text.text = "Starte Mission: "+Mission.current_mission.fullMission.get_current_mission_part().title;
			break;
		case InteractiveMessageContentType.QuitMission:
			switch (Mission.current_mission.mission_status) {
			case MissionStatus.Completed:
				new_button.description_text = "Glückwunsch, Captain!";
				new_button.button_text.text = "In den Sektorenraum zurückkehren";
				break;
			case MissionStatus.Failed:
				new_button.description_text = "Mission Failed";
				new_button.button_text.text = "In den Sektorenraum zurückkehren";

				InteractiveMessageButton new_button2 = new InteractiveMessageButton ();
				new_button2.button.transform.SetParent (image.transform);
				new_button2.content_type = type;
				new_button2.is_mission = true;

				new_button2.button.gameObject.SetActive (true);
				new_button2.button_text.text = "Mission neustarten";
				new_button2.description_text = "";
				buttons.Add (new_button2);
				break;
			default:
				print ("Error: MissionStatus not set");
				break;
			}
			break;
		default:
			print ("Error: InteractiveMessageContentType not set");
			break;
		}	

		buttons.Add (new_button);


		clean_buttons (new_button);

		update_buttons ();

		gameObject.SetActive (true);

	}

	public void button_clicked(int button_number){
		update_buttons ();
		if (buttons.Count == 0) {
			gameObject.SetActive (false);
			return;
		}
		InteractiveMessageButton btn = buttons [button_number];
		InteractiveMessageContentType content_type = btn.content_type;
		//btn.action ();
		switch (content_type) {
		case InteractiveMessageContentType.LoadScene:
			ObjectAura oa = null;
			foreach (ObjectAura a in ObjectAura.objectAurasPlayerIsIn) {
				if (a.objectAuraType != ObjectAuraType.ItemDrop) {
					oa = a;
					break;
				}
			}
			if (oa == null) {
				print ("no load scene Object aura : " + oa.gameObject.name);
				break;
			}
			switch (oa.objectAuraType) {
			case ObjectAuraType.PlanetSystem:
				PlanetSystem ps = oa.GetComponentInParent<PlanetSystem> ();
				if (btn.is_mission) {
					LevelManager.levelManager.load_new_scene(ps.mission_data.scene_name);
				} else {
					LevelManager.levelManager.load_new_scene (oa.GetComponentInParent<PlanetSystem> ().system_data.scene_name);
				}
				break;
			case ObjectAuraType.ErdeRaumdock:
				LevelManager.levelManager.load_new_scene ("Shop");
				break;
			case ObjectAuraType.ShopExit:
				LevelManager.levelManager.load_new_scene ("SektorenRaum");
				break;
			}
			break;
		case InteractiveMessageContentType.ItemDrop:
			Drop drop = btn.drop;
			if (drop == null) {
				print ("Error: no drop script on that object");
				return;
			}

			Item item = drop.item;

			if (Player.player.player_inventar.add_item (item)) {
				Destroy (drop.gameObject);
			} else {
				return;
			}

			break;
		case InteractiveMessageContentType.StartMission:
			Mission.current_mission.start_mission ();
			break;
		case InteractiveMessageContentType.QuitMission:
			LevelManager.levelManager.load_new_scene ("SektorenRaum");
			break;
		default:
			print ("ERROR: contenttype not set");
			break;
		}
		buttons.Remove (btn);
		Destroy (btn.button.gameObject);
		//remove_button (btn);

		update_buttons ();

		if (buttons.Count == 0)
			gameObject.SetActive (false);
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown (KeyCode.F)) {
			if (buttons.Count > 0) {
				button_clicked (0);
			}
		}
	}
}
