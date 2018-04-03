using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public enum ItemButtonPositionType{
	Inventar,
	ShopInventar,
	ImpulsAntrieb,
	WarpAntrieb, // TODO <-
	Schild,
	TopWeapons,
	BotWeapons
}

public class ItemButton : MonoBehaviour {

	Item item;
	public Image img;


	public Text mark_number_mark_text;

	[HideInInspector]
	public ItemButtonPositionType item_button_position_type;
	[HideInInspector]
	public int index;

	public static Sprite standard_tex;
	public static Color standard_color;

	void Awake () {
		standard_tex = img.sprite;
		standard_color = img.color;

	}
	void Update () {
	}

	public void set_item(Item i){
		item = i;
		if (item != null && item.ID!=0) {
			
			try {
				img.sprite = item.inventory_tex;
				img.color = Color.white;
				set_mark_text();
			} catch (System.Exception){
				Invoke ("set_image", 0.5f);
			}
			img.gameObject.SetActive (true);
		} else {
			img.sprite = standard_tex;
			img.color = standard_color;
			mark_number_mark_text.text = "";
			img.gameObject.SetActive (false);
		}

	}

	void set_image(){
		try{
			img.sprite = item.inventory_tex;
			set_mark_text();
		}catch{
			img.sprite = standard_tex;
		}
	}
	void set_mark_text(){
		try{
			mark_number_mark_text.text = Utils.get_mark_text (((Weapon)item).mark_number);
		} catch (System.InvalidCastException){
		}
	}
		
	public void pointer_enter(){
		if (Utils.item_is_null(item)) {return;}
		//UI.ui.item_description_text.gameObject.SetActive (true);
		UI.ui.item_description_text_bg.gameObject.SetActive(true);
		try{
			UI.ui.item_description_text.text = item.get_description_text ();
		}catch{
			UI.ui.item_description_text.text = "";
		}

	}
	public void pointer_exit (){
		//UI.ui.item_description_text.gameObject.SetActive (false);
		UI.ui.item_description_text_bg.gameObject.SetActive(false);
	}
	bool replace_property(Item i, bool remove, bool remove_schild = true){ // return false, wenn !remove und i nicht zum itembuttonpositiontype passt und tut nichts, sonst true
		switch (item_button_position_type) {
		case ItemButtonPositionType.Inventar:
			Player.player.player_inventar.items [index] = remove ? null : i;
			break;
		case ItemButtonPositionType.ImpulsAntrieb:
			if (!remove && i.item_type != ItemType.ImpulsAntrieb) {
				print ("kein imp antrieb");
				return false;
			}
			Player.player.spaceship.impuls_antrieb = remove ? null : (ImpulsAntrieb)i;
			Player.player.spaceship.apply_item_stats ();
			break;
		case ItemButtonPositionType.Schild:
			if (!remove && i.item_type!=ItemType.Schild) {
				print ("kein schild");
				return false;
			}
			//Player.player.spaceship.schild_item = remove ? null : (SchildItem)i;
			//Player.player.spaceship.apply_item_stats ();
			if (remove_schild)
				Player.player.spaceship.neues_schild(remove ? null : (SchildItem)i);
			break;
		case ItemButtonPositionType.TopWeapons:
			if (!remove && i.item_type != ItemType.Weapon) {
				print ("keine weapon");
				return false;
			}

			if ((((Weapon)i).special_raw_spaceship_type & Player.player.spaceship.raumschiff.raw_raumschiff_type) != Player.player.spaceship.raumschiff.raw_raumschiff_type) {
				print ("Weapon passt nicht zu aktuellem Raumschiff");
				return false;
			}
			if ((((Weapon)i).special_weapon_position & PlayerWeaponPositions.Top) != PlayerWeaponPositions.Top) {
				print ("Weapon passt nicht zu top");
				return false;
			}
				//
			SpaceshipWeaponPosition t = Player.player.get_top_weapon_position ();
			if (remove) {
				t.weapons.Remove ((Weapon)i);
			} else {
				((Weapon)i).weapon_position = t.gameObject;
				t.weapons.Add ((Weapon)i);
			}
			break;
		case ItemButtonPositionType.BotWeapons:
			if (!remove && i.item_type != ItemType.Weapon) {
				print ("keine weapon");
				return false;
			}

			if ((((Weapon)i).special_raw_spaceship_type & Player.player.spaceship.raumschiff.raw_raumschiff_type) != Player.player.spaceship.raumschiff.raw_raumschiff_type) {
				print ("Weapon passt nicht zu aktuellem Raumschiff");
				return false;
			}

			if ((((Weapon)i).special_weapon_position & PlayerWeaponPositions.Bot) != PlayerWeaponPositions.Bot) {
				print ("Weapon passt nicht zu bot");
				return false;
			}

			SpaceshipWeaponPosition b = Player.player.get_bot_weapon_position ();
			if (remove) {
				b.weapons.Remove ((Weapon)i);
			} else {
				((Weapon)i).weapon_position = b.gameObject;
				b.weapons.Add ((Weapon)i);
			}
			break;
		}
		if (Player.player.player_environment_status == PlayerEnvironmentStatus.Space)
			UIWeaponStats.uiweaponstats.update_weapon_images ();
		LevelManager.save_player_data ();
		if (remove) {
			set_item (null);
		} else {
			set_item (i);
		}
		return true;
	}

	public void click(){
		if (Utils.item_is_null(UI.ui.selected_item)) {
			if (Utils.item_is_null(item)) {
				return;
			}
			UI.ui.selected_item = item;
			replace_property (item, true);
		} else {
			if (Utils.item_is_null(item)) {
				Item s_i = UI.ui.selected_item;
				UI.ui.selected_item = null;
				if (!replace_property (s_i, false)) {
					UI.ui.selected_item = s_i;
				}
			} else {
				Item old_item = item;
				replace_property (item, true, false);
				if (replace_property (UI.ui.selected_item, false)) {
					UI.ui.selected_item = old_item;
				} else {
					replace_property (old_item, false);
				}
			}
		}
	}
}
