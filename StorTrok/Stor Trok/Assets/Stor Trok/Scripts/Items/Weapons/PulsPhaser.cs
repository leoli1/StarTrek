using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PulsPhaserType{
	StandardM1
}

[CreateAssetMenu(menuName = "Game-Items/Weapons/PulsPhaser")]
public class PulsPhaser : Weapon { // id = 3000

	public PulsPhaserType puls_phaser_type;

	[HideInInspector]
	public GameObject target_object;
	[HideInInspector]
	public int enemy_layer;

	public int salve_size;
	public float salve_reload_time;

	public bool single;

	[HideInInspector]
	public int missiles_shot = 0;
	public PulsPhaser(){
		arc_range = 65;
		special_weapon_position = PlayerWeaponPositions.Top;
		//AllPulsPhasers.Add (this);
		weapon_object = Weapons.weapons == null ? null : Weapons.weapons.puls_phaser;
	}
		

	public override bool can_shoot(){
		if (missiles_shot < salve_size && Time.time - last_time_shot >= reload_time) {
			return true;
		} else if (missiles_shot == salve_size) {
			if (Time.time - last_time_shot >= salve_reload_time) {
				this.missiles_shot = 0;
				return true;
			} else {
				return false;
			}
		}
		return false;
	}

	public override void shoot(){
		last_time_shot = Time.time;
		missiles_shot++;
		animation ();
	}

	public override void animation ()
	{
		if (single) {
			GameObject r = GameObject.Instantiate (weapon_object);
			r.transform.position = this.weapon_position.transform.position;
			r.GetComponent<PulsPhaserUpdate> ().target = this.target_object;
			r.GetComponent<PulsPhaserUpdate> ().enemy_layer = this.enemy_layer;
			r.GetComponent<PulsPhaserUpdate> ().puls_phaser = this;
		} else {
			List<Transform> ts = this.weapon_position.GetComponent<SpaceshipWeaponPosition> ().puls_phaser_weapons_positions;
			foreach (Transform trans in ts){
				GameObject r = GameObject.Instantiate (weapon_object);
				r.transform.position = trans.position;
				r.GetComponent<PulsPhaserUpdate> ().target = this.target_object;
				r.GetComponent<PulsPhaserUpdate> ().enemy_layer = this.enemy_layer;
				r.GetComponent<PulsPhaserUpdate> ().puls_phaser = this;
			}
		}

	}
	public override void deal_damage (int layer)
	{
	}

}
