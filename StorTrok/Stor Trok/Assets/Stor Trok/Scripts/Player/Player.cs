using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum PlayerEnvironmentStatus{
	None,
	Space,
	Ground
}

public class Player {// : MonoBehaviour { infos über den spielers

	static Player player_inst; // static variable, um aus jedem script direkt auf die einzige instanz der klasse zugreifen zu können

	public const int environment_layer = 10;
	public const int player_layer = 9;
	public const int enemy_layer = 8;

	public string name;
	public int money;

	public Spaceship spaceship; // spaceship instanz des spielers

	public PlayerEnvironmentStatus player_environment_status = PlayerEnvironmentStatus.None;

	public Inventar player_inventar; // inventar des spielers

	public Vector3 last_coordinate;

	public string player_data_path {
		get{ 
			return Application.dataPath + "/Resources/pdata_"+name+".dat";
		}	
	}

	public static Player player{
		get{ 
			if (Player.player_inst!=null)
				return Player.player_inst;
			System.Console.WriteLine ("Warning no Player.player");
			return null;
		}
		set {
			player_inst = value;
		}

	}

	public Player(Spaceship s){
		spaceship = s;
		Player.player = this;
	}
	public Player(PlayerEnvironmentStatus pes){
		player_environment_status = pes;
		Player.player = this;
	}

	public void setup_ship_items(){
		setup_modules ();
		setup_weapons ();
		setup_inventory ();
		setup_antrieb ();
		setup_schild ();
		this.spaceship.apply_item_stats ();
	}

	public void setup_modules(){
		List<int> modules = LevelManager.player_data.get_current_spaceship ().modules;

		spaceship.modules = new List<SpaceshipModule> ();
		foreach (int m in modules) {
			spaceship.modules.Add ((SpaceshipModule)Item.get_item_by_id (m));
		}
	}

	public void setup_weapons(){
		List<int> top_weapons = LevelManager.player_data.get_current_spaceship().top_weapons;
		List<int> bot_weapons = LevelManager.player_data.get_current_spaceship().bot_weapons;

		get_top_weapon_position ().weapons = new List<Weapon> ();
		get_bot_weapon_position ().weapons = new List<Weapon> ();

		foreach (int t in top_weapons) {
			Weapon w = (Weapon)Item.get_item_by_id (t);
			w.weapon_position = get_top_weapon_position ().gameObject;
			get_top_weapon_position().weapons.Add(w);
		}
		foreach (int b in bot_weapons) {
			Weapon w = (Weapon)Item.get_item_by_id (b);
			w.weapon_position = get_bot_weapon_position ().gameObject;
			get_bot_weapon_position().weapons.Add(w);
		}
	}

	public void setup_inventory(){
		List<int> inv = LevelManager.player_data.inventar;
		player_inventar = new Inventar ();
		for (int i = 0; i < inv.Count; i++) {
			player_inventar.items [i] = Item.get_item_by_id(inv [i]);
		}
	}
	public void setup_antrieb(){
		int a = LevelManager.player_data.get_current_spaceship().impuls_antrieb;
		this.spaceship.impuls_antrieb = (ImpulsAntrieb)Item.get_item_by_id (a);
		int w = LevelManager.player_data.get_current_spaceship().warp_antrieb;
		this.spaceship.warp_antrieb = (WarpAntrieb)Item.get_item_by_id (w);
	}
	public void setup_schild(){
		int a = LevelManager.player_data.get_current_spaceship().raumschiff_schild;
		this.spaceship.schild_item = (SchildItem)Item.get_item_by_id (a);
	}

	public List<Weapon> get_top_weapons(){
		foreach (SpaceshipWeaponPosition wpos in spaceship.spaceshipWeaponPositions) {
			if (wpos.local_direction.normalized == new Vector3 (0, 0, 1)) {
				return wpos.weapons;
			}
		}
		return null;
	}
	public List<Weapon> get_bot_weapons(){
		foreach (SpaceshipWeaponPosition wpos in spaceship.spaceshipWeaponPositions) {
			if (wpos.local_direction.normalized == new Vector3 (0, 0, -1)) {
				return wpos.weapons;
			}
		}
		return null;
	}

	public SpaceshipWeaponPosition get_top_weapon_position(){
		foreach (SpaceshipWeaponPosition wpos in spaceship.spaceshipWeaponPositions) {
			if (wpos.local_direction.normalized == new Vector3 (0, 0, 1)) {
				return wpos;
			}
		}
		return null;
	}
	public SpaceshipWeaponPosition get_bot_weapon_position(){
		foreach (SpaceshipWeaponPosition wpos in spaceship.spaceshipWeaponPositions) {
			if (wpos.local_direction.normalized == new Vector3 (0, 0, -1)) {
				return wpos;
			}
		}
		return null;
	}
}
