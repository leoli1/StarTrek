using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class PlayerData { // Eine Instanz dieser Klasse wird gespeichert, in der die Informationen über einen/den Spieler sind
	public string name;
	public int money;
	public List<int> inventar; // das Inventar dieses Spielers
	public int selected_item; // sofern das spiel geschlossen wurde, während ein item ausgewählt war, wird dieses auch gespeichert

	public List<SpaceshipData> owned_spaceships_with_items; //alle raumschiffe, die dieser spieler besitzt und ihre ausgerüsteten items
	public int current_spaceship; // die id des raumschiffs, das der spieler gerade benutzt

	public SerializableVector3 sektorenraum_koordinaten; // die position, die der spieler beim verlassen des sektorenraums hatte

	public SpaceshipData get_current_spaceship(){ // sucht aus der liste der raumschiffe, die im besitz des spielers sind das zurzeit benutzte
		foreach (SpaceshipData d in owned_spaceships_with_items) {
			if (d.spaceshipID == current_spaceship) {
				return d;
			}
		}
		return null;
	}

	public PlayerData(){
	}

	public void add_spaceship(int id){
		Raumschiff r = Raumschiff.get_raumschiff_by_id (id);

		List<int> modules = new List<int> ();
		foreach (SpaceshipModule m in r.standard_spaceship_modules) {
			modules.Add (m.ID);
		}

		List<int> top_weapons = new List<int> ();
		foreach (Weapon w in r.standard_top_weapons) {
			top_weapons.Add (w.ID);
		}
		List<int> bot_weapons = new List<int> ();
		foreach (Weapon w in r.standard_bot_weapons) {
			bot_weapons.Add (w.ID);
		}

		SpaceshipData data = new SpaceshipData {
			spaceshipID = id,
			raumschiff_schild = r.standard_schild.ID,
			impuls_antrieb = r.standard_impuls_antrieb.ID,
			warp_antrieb = r.standard_warp_antrieb.ID,
			modules = modules,
			top_weapons = top_weapons,
			bot_weapons = bot_weapons
		};

		owned_spaceships_with_items.Add (data);
	}

	public void set_ids(){ // legt die entsprechenden werte fest

		name = Player.player.name;
		money = Player.player.money;

		//inventar
		inventar = new List<int> ();
		foreach (Item item in Player.player.player_inventar.items) {
			inventar.Add (item==null ? 0 : item.ID);
		}

		//selected item
		selected_item = UI.ui.selected_item==null ? 0 : UI.ui.selected_item.ID;

		//zurzeit ausgewähltes raumschiff
		this.current_spaceship = Player.player.spaceship.raumschiff.ID;

		//---
		// erstellt eine neue SpaceshipData-Instanz um die Items des zurzeit ausgewählten Raumschiff zu speichern
		SpaceshipData current_spaceship_data = new SpaceshipData ();
		current_spaceship_data.spaceshipID = Player.player.spaceship.raumschiff.ID;
		current_spaceship_data.raumschiff_schild = Utils.item_is_null(Player.player.spaceship.schild_item) ? 0 : Player.player.spaceship.schild_item.ID;
		current_spaceship_data.impuls_antrieb = Utils.item_is_null(Player.player.spaceship.impuls_antrieb) ? 0 : Player.player.spaceship.impuls_antrieb.ID;
		current_spaceship_data.warp_antrieb = Utils.item_is_null(Player.player.spaceship.warp_antrieb) ? 0 : Player.player.spaceship.warp_antrieb.ID;

		current_spaceship_data.modules = new List<int> ();
		foreach (SpaceshipModule mod in Player.player.spaceship.modules) {
			if (mod!=null)
				current_spaceship_data.modules.Add (mod.ID);
		}

		current_spaceship_data.top_weapons = new List<int> ();
		foreach (Weapon w in Player.player.get_top_weapons()) {
			current_spaceship_data.top_weapons.Add (w.ID);
		}

		current_spaceship_data.bot_weapons = new List<int> ();
		foreach (Weapon w in Player.player.get_bot_weapons()) {
			current_spaceship_data.bot_weapons.Add (w.ID);
		}
		//---

		// prüft, ob das ausgewählte raumschiff schon im besitz des spielers ist...
		bool newS = true;
		int i = 0;
		foreach (SpaceshipData data in owned_spaceships_with_items) {
			if (data.spaceshipID == current_spaceship) { // wenn ja:
				newS = false;
				owned_spaceships_with_items [i] = current_spaceship_data; // die informationen über die ausgerüsteten items werden durch die neuen infos ersetzt
			}
			i++;
		}
		if (newS){ // wenn nicht:
			owned_spaceships_with_items.Add(current_spaceship_data); // neues raumschiff
		}

		if (LevelManager.levelManager.scene_name == "SektorenRaum") {
			//sektorenraum_koordinaten = new SerializableVector3(Player.player.spaceship.transform.position);
			sektorenraum_koordinaten = new SerializableVector3(Player.player.last_coordinate);
		}
	}

	public static PlayerData new_player(int spaceship_id){ // erstellt eine komplett neue PlayerData-instanz mit startitems für einen neuen spieler

		PlayerData p = new PlayerData ();

		p.name = "Untitled0";
		p.money = 1000; // Einheit in Stücken

		p.inventar = new List<int> (16); // leeres inventar

		p.selected_item = 0;

		p.owned_spaceships_with_items = new List<SpaceshipData> ();


		SpaceshipData d = new SpaceshipData ();

		Raumschiff r = Raumschiff.get_raumschiff_by_id(spaceship_id);

		if (r != null) {

			d.spaceshipID = r.ID;
			d.modules = new List<int> ();


			foreach (SpaceshipModule m in r.standard_spaceship_modules) {
				d.modules.Add (m.ID);
			}

			d.top_weapons = new List<int> ();
			foreach (Weapon m in r.standard_top_weapons) {
				d.top_weapons.Add (m.ID);
			}

			d.bot_weapons = new List<int> ();
			foreach (Weapon m in r.standard_bot_weapons) {
				d.bot_weapons.Add (m.ID);
			}


			d.impuls_antrieb = r.standard_impuls_antrieb.ID;
			d.warp_antrieb = r.standard_warp_antrieb.ID;
			d.raumschiff_schild = r.standard_schild.ID;
		}

		p.owned_spaceships_with_items.Add (d);

		p.current_spaceship = p.owned_spaceships_with_items [0].spaceshipID;

		p.sektorenraum_koordinaten = new SerializableVector3 (Vector3.zero);

		return p;
	}

}
