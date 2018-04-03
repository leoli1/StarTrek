using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PulsPhaserUpdate : MonoBehaviour {


	[HideInInspector] public GameObject target;

	[HideInInspector] public int enemy_layer;
	private int layer_mask;

	[HideInInspector] public PulsPhaser puls_phaser;
	public const float speed = 800;

	public const float life_time = 10;

	private Vector3 last_position;
	private Vector3 end_velocity;
	void Start () {
		Destroy (gameObject, life_time);
		last_position = transform.position;

		layer_mask = ((1 << enemy_layer) | (1 << Player.environment_layer));// | (1 << Physics.IgnoreRaycastLayer));

	}

	void Update () {
		if (target == null) {
			this.enabled = false;
			return;
		}

		if (target.is_destroyed ()) {
			transform.Translate (end_velocity.normalized * speed * Time.deltaTime, Space.World);
			return;
		}
		Vector3 v = (target.transform.position - transform.position).normalized * speed * Time.deltaTime;
		transform.Translate(v, Space.World);
		end_velocity = v;

		transform.LookAt (target.transform.position);

		Vector3 cur_pos = transform.position;
		Vector3 dir = cur_pos - last_position;

		Ray ray = new Ray (last_position, dir);
		RaycastHit hit;

		if (Physics.Raycast (ray, out hit, dir.magnitude, layer_mask)) {
			
			GameObject g = hit.collider.gameObject;

			Spaceship s = Spaceship.get_spaceship (g);
			if (g.GetComponent<SchildShaderScript> ()) {
			//	g.GetComponent<SchildShaderScript> ().col (hit.point);
				puls_phaser.weapon_impact_effect (g.transform.InverseTransformPoint (hit.point), g.transform, g.GetComponent<SchildShaderScript>().normal_color);
			}
			//puls_phaser.weapon_impact_effect (g.transform.InverseTransformPoint (hit.point), g.transform);

			if (g.layer == Player.environment_layer) {
				Destroy (gameObject);
			}

			if (s != null) {
				Vector3 point = hit.point;

				int layer = g.layer;
				//print ("collision object name: " + s.gameObject.name + " layer: " + layer.ToString () + " | target layer is: " + enemy_layer.ToString ());
				if (layer == enemy_layer) {
					float angle = s.get_angle (point);

					Schild schild = s.schild;
					SchildPartTypes sp = schild.get_schildpart_at_angle (angle);

					float dmg_to_si = puls_phaser.calc_damage (schild.get_shield_intensity_at_part (sp));
					float dmg_to_shields = puls_phaser.damage;// - dmg_to_si;
					s.apply_damage (dmg_to_shields, sp, dmg_to_si);
					puls_phaser.apply_status_effects (s);

					Destroy (gameObject);
				}
			} else {
				DestroyableObject d = DestroyableObject.get_destroyable_object (g);
				if (d != null) {
					d.apply_damage (puls_phaser.damage);
					Destroy (gameObject);
				}
			}
		}

		last_position = transform.position;
	}
}
