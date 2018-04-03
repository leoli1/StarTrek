using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PhaserType{ // alle phaser typen
	StandardM1,
	ShuttleM1,
	BorgM1,
	KlingonM1
}

[CreateAssetMenu(menuName = "Game-Items/Weapons/Phaser")]
public class Phaser : Weapon {//id = 4000


	public Phaser get_copy(){
		return this.MemberwiseClone () as Phaser;
	}

	public PhaserType phaser_type;

	[HideInInspector]
	public GameObject start_pos; // wo der phaser beam los geht...
	[HideInInspector]
	public GameObject end_pos; //... und wo er endet

	[HideInInspector]
	public float phaser_anim_lifetime = 0.2f; // wie lange der beam da ist

	public Phaser(){
		this.arc_range = 270;
	//	AllPhasers.Add (this);
		phaser_anim_lifetime = 0.2f;

		weapon_object = Weapons.weapons == null ? null : Weapons.weapons.phaser;
	}


	// animation des phasers (linerender etc.)
	public override void animation ()
	{
		GameObject phaser = GameObject.Instantiate (weapon_object);
		LineRenderer lr = phaser.GetComponent<LineRenderer> ();

		List<Transform> phasers = weapon_position.GetComponent<SpaceshipWeaponPosition> ().phaser_positions_path;
		if (phasers.Count != 0) {
			Vector3 new_start_position = Utils.get_random_point_on_path (phasers);
			GameObject g = new GameObject ("phaser_start_point");
			g.transform.SetParent (weapon_position.transform);
			g.transform.position = new_start_position;
			GameObject.Destroy (g, phaser_anim_lifetime + 0.1f);
			this.start_pos = g;
		}

		lr.SetPositions (new Vector3[]{ 
			start_pos.transform.position, 
			end_pos.transform.position
		});
		weapon_color.a = 1;
		lr.startColor = this.weapon_color;
		lr.endColor = this.weapon_color;

		PhaserUpdate pu = phaser.GetComponent<PhaserUpdate> ();
		pu.start_pos = start_pos;
		pu.end_pos = end_pos;
		GameObject.Destroy (phaser, phaser_anim_lifetime);
	
	}


	public override void deal_damage(int layer){
		Ray ray = new Ray (start_pos.transform.position, end_pos.transform.position - start_pos.transform.position);
		RaycastHit hit;

		//this.end_pos = new GameObject ("phaser_end_point");
		//GameObject.Destroy (this.end_pos, 5);

		int layer_mask = ((1 << layer) | (1 << Player.environment_layer));// | Physics.IgnoreRaycastLayer);

		if (Physics.Raycast(ray, out hit, Spaceship.max_attack_distance, layer_mask)){
			
			this.end_pos.transform.position = hit.point;

			
			GameObject go = hit.collider.gameObject;

			this.end_pos.transform.SetParent (go.transform);

			if (go.layer == Player.environment_layer) {
				return;
			}
			
			if (go.GetComponent<SchildShaderScript> ()) { // Schild impact animation
				weapon_impact_effect (go.transform.InverseTransformPoint (hit.point), go.transform, go.GetComponent<SchildShaderScript>().normal_color);
			}

			Vector3 p = hit.point;

			//weapon_impact_effect (go.transform.InverseTransformPoint (p), go.transform);

			Spaceship spaceship = Spaceship.get_spaceship (go);
			if (spaceship != null) {
				float angle = spaceship.get_angle (p);

				Schild schild = spaceship.schild;
				SchildPartTypes sp = schild.get_schildpart_at_angle (angle);

				float dmg_to_si = this.calc_damage (schild.get_shield_intensity_at_part (sp));
				float dmg_to_shields = damage;// - dmg_to_si;
				spaceship.apply_damage (dmg_to_shields, sp, dmg_to_si);
				apply_status_effects (spaceship);
			} else {
				DestroyableObject d = DestroyableObject.get_destroyable_object (go);
				if (d != null) {
					d.apply_damage (damage);
				}
			}
		} else {
			end_pos.transform.position = start_pos.transform.position + (end_pos.transform.position - start_pos.transform.position).normalized * Spaceship.max_attack_distance;
		}

	}
}
