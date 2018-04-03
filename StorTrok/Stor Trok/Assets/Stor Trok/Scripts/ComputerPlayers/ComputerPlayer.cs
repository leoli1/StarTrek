using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using UnityEditor;

public enum EnemyAIMovementType{
	None,
	Custom,
	FollowNavigationPath
}

public class ComputerPlayer : MonoBehaviour {

	private Spaceship spaceship_value;
	public Spaceship spaceship{
		get{ 
			if (spaceship_value == null)
				spaceship_value = GetComponent<Spaceship> ();
			return spaceship_value;
		}
	}

	[HideInInspector]
	public GameObject selected_enemy;

	public MonoBehaviour movement_script;

	[HideInInspector]
	public bool no_movement = false;



	void Start () {

		setup ();
	}
	void setup(){
		if (spaceship.spaceship_group_type == ObjectGroupType.Enemy) {
			setup_with_gameobject_info ();
		} else {
			setup_with_asset_info ();
		}
		spaceship.setup_standard_items ();
		spaceship.apply_item_stats ();
	}

	void setup_with_gameobject_info(){
		foreach (SpaceshipWeaponPosition pos in spaceship.spaceshipWeaponPositions) {
			List<Weapon> weapons = pos.weapons;
			List<int> ids = new List<int> ();
			foreach (Weapon w in weapons) {
				ids.Add (w.ID);
			}
			pos.weapons = new List<Weapon> ();
			foreach (int id in ids) {
				Weapon weapon = (Weapon)Weapon.get_item_by_id (id);
				weapon.weapon_position = pos.gameObject;
				pos.weapons.Add (weapon);
			}
		}
	}

	void setup_with_asset_info(){
		Raumschiff raumschiff = spaceship.raumschiff;

		List<SpaceshipModule> modules = raumschiff.standard_spaceship_modules;

		spaceship.modules = new List<SpaceshipModule> ();
		foreach (SpaceshipModule m in modules) {
			spaceship.modules.Add ((SpaceshipModule)Item.get_item_by_id (m.ID));
		}

		List<Weapon> top_weapons = raumschiff.standard_top_weapons;
		List<Weapon> bot_weapons = raumschiff.standard_bot_weapons;

		spaceship.get_top_weapon_position ().weapons = new List<Weapon> ();
		spaceship.get_bot_weapon_position ().weapons = new List<Weapon> ();

		foreach (Weapon t in top_weapons) {
			Weapon w = (Weapon)Item.get_item_by_id (t.ID);
			w.weapon_position = spaceship.get_top_weapon_position ().gameObject;
			spaceship.get_top_weapon_position().weapons.Add(w);
		}
		foreach (Weapon b in bot_weapons) {
			Weapon w = (Weapon)Item.get_item_by_id (b.ID);
			w.weapon_position = spaceship.get_bot_weapon_position ().gameObject;
			spaceship.get_bot_weapon_position().weapons.Add(w);
		}
	}

	public List<Spaceship> get_enemies(){
		return spaceship.spaceship_group_type == ObjectGroupType.Enemy ? Spaceship.get_active_player_spaceships () : Spaceship.get_active_enemy_spaceships ();
	}
	public int get_enemy_layer(){
		return spaceship.spaceship_group_type == ObjectGroupType.Enemy ? Player.player_layer : Player.enemy_layer;
	}

	public void enable_movement(){
		movement_script.enabled = true;
		no_movement = false;
	}
	public void disable_movement(){
		movement_script.enabled = false;
		no_movement = true;
	}

	void Update () {
		if (spaceship.warping_in) {
			spaceship.abort_auto_navigation ();
			return;
		}
		if (no_movement) {
			adapt_rotation_to_weak_shields ();
		}
		if (spaceship.destroyed)
			this.enabled = false;
		
	}

	void adapt_rotation_to_weak_shields(){
		if (selected_enemy == null)
			return;
		//TODO
		SchildPart strongest = spaceship.schild.get_best_shield_part();
		float angle = spaceship.get_angle (selected_enemy.transform.position);
		angle += angle<-45 ? 360 : 0;
		float target_angle = strongest.angle_range.medium;
	//	print ("current angle: " + angle + "; target angle: " + target_angle);
		float dir = angle > target_angle ? -1 : 1;
		transform.Rotate (new Vector3 (0, dir, 0)*spaceship.rotation_speed*Time.deltaTime, Space.World);
	}
		
}
