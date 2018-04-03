using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RaumschiffInventory : MonoBehaviour {

	public static RaumschiffInventory raumschiffInventory;
	RectTransform rectTransform;

	public GameObject item_button;
	public RectTransform item_button_rt;


	public Text impuls_antrieb_text;
	public Text schild_text;

	public Text top_weapon_text;
	public Text bot_weapon_text;

	public GameObject impuls_antrieb_button;
	public GameObject schild_button;

	public List<GameObject> top_weapon_buttons;
	public List<GameObject> bot_weapon_buttons;


	const float offset = 10;

	void Start () {
		RaumschiffInventory.raumschiffInventory = this;

		rectTransform = this.GetComponent<RectTransform> ();
		//rectTransform.sizeDelta = new Vector2 (400, 300);
		rectTransform.position = new Vector3 (Screen.width * 0.2f, Screen.height * 0.5f, 0);
		reset_buttons ();
		set_buttons ();
	}

	void Update () {
		
	}

	public void reset_buttons(){
		if (impuls_antrieb_button != null) { GameObject.Destroy (impuls_antrieb_button); }
		if (schild_button != null) { GameObject.Destroy (schild_button); }
		foreach (GameObject t in top_weapon_buttons) {
			GameObject.Destroy (t);
		}
		foreach (GameObject b in bot_weapon_buttons) {
			GameObject.Destroy (b);
		}
		top_weapon_buttons = new List<GameObject> ();
		bot_weapon_buttons = new List<GameObject> ();

		float x_pos = this.rectTransform.sizeDelta.x - item_button_rt.sizeDelta.x - offset;
		float y_add = (item_button_rt.sizeDelta.y - impuls_antrieb_text.rectTransform.sizeDelta.y);

		impuls_antrieb_button = GameObject.Instantiate (item_button);
		impuls_antrieb_button.transform.SetParent (transform);
		impuls_antrieb_button.GetComponent<RectTransform> ().localPosition = new Vector3 (x_pos, impuls_antrieb_text.rectTransform.localPosition.y-item_button_rt.sizeDelta.y+y_add, 0);
	
		schild_button = GameObject.Instantiate (item_button);
		schild_button.transform.SetParent (transform);
		schild_button.GetComponent<RectTransform> ().localPosition = new Vector3 (x_pos, schild_text.rectTransform.localPosition.y-item_button_rt.sizeDelta.y+y_add, 0);
	
		for (int i = 0; i < Player.player.get_top_weapon_position().capacity; i++) {
			GameObject ntw = GameObject.Instantiate (item_button);
			ntw.transform.SetParent (transform);

			ntw.GetComponent<RectTransform> ().localPosition = new Vector3 (x_pos - i * item_button_rt.sizeDelta.x, top_weapon_text.rectTransform.localPosition.y - item_button_rt.sizeDelta.y+y_add, 0);
			top_weapon_buttons.Add (ntw);
		}


		for (int i = 0; i < Player.player.get_bot_weapon_position().capacity; i++) {
			GameObject ntw = GameObject.Instantiate (item_button);
			ntw.transform.SetParent (transform);

			ntw.GetComponent<RectTransform> ().localPosition = new Vector3 (x_pos - i * item_button_rt.sizeDelta.x, bot_weapon_text.rectTransform.localPosition.y - item_button_rt.sizeDelta.y+y_add, 0);
			bot_weapon_buttons.Add (ntw);
		}
	}

	public void set_buttons(){
		impuls_antrieb_button.GetComponent<ItemButton>().set_item(Player.player.spaceship.impuls_antrieb);
		impuls_antrieb_button.GetComponent<ItemButton> ().item_button_position_type = ItemButtonPositionType.ImpulsAntrieb;

		schild_button.GetComponent<ItemButton> ().set_item (Player.player.spaceship.schild_item);
		schild_button.GetComponent<ItemButton> ().item_button_position_type = ItemButtonPositionType.Schild;

		int i = 0;
		List<Weapon> tws = Player.player.get_top_weapons ();
		foreach (GameObject t in top_weapon_buttons) {
			t.GetComponent<ItemButton> ().item_button_position_type = ItemButtonPositionType.TopWeapons;
			t.GetComponent<ItemButton> ().index = i;
			if (i >= tws.Count) {
				break;
			}

			t.GetComponent<ItemButton> ().set_item (tws [i]);
			i++;
		}

		i = 0;
		List<Weapon> bws = Player.player.get_bot_weapons ();
		foreach (GameObject b in bot_weapon_buttons) {
			b.GetComponent<ItemButton> ().item_button_position_type = ItemButtonPositionType.BotWeapons;
			b.GetComponent<ItemButton> ().index = i;
			if (i >= bws.Count) {
				break;
			}
			b.GetComponent<ItemButton> ().set_item (bws [i]);
			i++;
		}
	}
}
