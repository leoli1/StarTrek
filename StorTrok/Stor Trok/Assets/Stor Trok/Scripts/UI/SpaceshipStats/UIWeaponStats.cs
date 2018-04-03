using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIWeaponStats : MonoBehaviour {
	public static UIWeaponStats uiweaponstats;
	public GameObject uiweapon;

	public GameObject phaserToggleButton;
	public GameObject torpedoToggleButton;

	public int image_width = 32;
	public int offset = 3;

	private Vector3 x_y_min_localposition;

	public Image background;
	public Text weapons_text;
	public Text front_weapons_text;
	public Text back_weapons_text;

	void Start () {
		UIWeaponStats.uiweaponstats = this;

		x_y_min_localposition = new Vector3 (30, 50, 0);//UI.ui.ent_image.rectTransform.position + new Vector3(235,0,0);

		setup_weapon_images ();
			
		setup_toggle_buttons ();
	}

	public void setup_weapon_images(){
		UIWeaponImage.allUIWeaponImages = new List<UIWeaponImage> ();

		int top = Player.player.get_top_weapon_position ().capacity;
		for (int i = 0; i < top; i++) {
			GameObject g = GameObject.Instantiate (uiweapon, transform);
			//g.GetComponent<RectTransform> ().position = x_y_min_position + new Vector3 ((image_width+offset)*i, image_width+offset, 0);
			g.GetComponent<RectTransform>().localPosition = x_y_min_localposition + new Vector3 ((image_width+offset)*i+90, image_width+offset, 0);
			g.GetComponent<RectTransform> ().sizeDelta = new Vector2 (image_width, image_width);
			g.GetComponent<UIWeaponImage> ().pos = PlayerWeaponPositions.Top;
			if (Player.player.get_top_weapon_position ().weapons.Count > i) {
				g.GetComponent<UIWeaponImage> ().weapon = Player.player.get_top_weapon_position ().weapons [i];
			}
		}

		int bot = Player.player.get_bot_weapon_position ().capacity;
		for (int i = 0; i < bot; i++) {
			GameObject g = GameObject.Instantiate (uiweapon, transform);
			g.GetComponent<RectTransform> ().localPosition = x_y_min_localposition + new Vector3 ((image_width+offset)*i+90, 0, 0);
			g.GetComponent<RectTransform> ().sizeDelta = new Vector2 (image_width, image_width);
			g.GetComponent<UIWeaponImage> ().pos = PlayerWeaponPositions.Bot;
			if (Player.player.get_bot_weapon_position ().weapons.Count > i) {
				g.GetComponent<UIWeaponImage> ().weapon = Player.player.get_bot_weapon_position ().weapons [i];
			}
		}
		background.rectTransform.localPosition = x_y_min_localposition;// + new Vector3 (-90, 0, 0);

		weapons_text.rectTransform.localPosition = new Vector3 (4, 2.5f * image_width + 2 * offset);
		front_weapons_text.rectTransform.localPosition = new Vector3 (4, 1.5f*image_width + offset, 0);
		back_weapons_text.rectTransform.localPosition = new Vector3 (4, 0.5f*image_width, 0);
	}

	public void setup_toggle_buttons(){
		int y = 2 * image_width + 3*offset;
		phaserToggleButton.GetComponent<RectTransform> ().localPosition = x_y_min_localposition + new Vector3 (90, y, 0);
		torpedoToggleButton.GetComponent<RectTransform> ().localPosition = x_y_min_localposition + new Vector3 (image_width+offset+90, y, 0);
	}

	public void update_weapon_images(){
		int t = 0;
		int b = 0;
		foreach (UIWeaponImage i in UIWeaponImage.allUIWeaponImages) {
			switch (i.pos) {
			case PlayerWeaponPositions.Top:
				if (Player.player.get_top_weapon_position ().weapons.Count > t) {
					i.weapon = Player.player.get_top_weapon_position ().weapons [t];
				} else {
					i.weapon = null;
				}
				t++;
				break;
			case PlayerWeaponPositions.Bot:
				if (Player.player.get_bot_weapon_position ().weapons.Count > b) {
					i.weapon = Player.player.get_bot_weapon_position ().weapons [b];
				} else {
					i.weapon = null;
				}
				b++;
				break;
			}
			i.set_image ();
		}
	}
	
	// Update is called once per frame
	void Update () {
		WeaponRawTypes t = WeaponRawTypes.None;
		t = phaserToggleButton.GetComponent<ToggleButton> ().enabled ? (t | WeaponRawTypes.Phaser) | WeaponRawTypes.PulsPhaser : t;
		t = torpedoToggleButton.GetComponent<ToggleButton> ().enabled ? t | WeaponRawTypes.Torpedo : t;
		PlayerScript.playerScript.weapon_types = t;

	}
}
