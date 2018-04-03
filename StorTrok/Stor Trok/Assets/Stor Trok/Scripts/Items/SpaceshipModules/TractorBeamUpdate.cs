using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TractorBeamUpdate : MonoBehaviour {

	[HideInInspector] public GameObject target_object;
	[HideInInspector] public TractorBeam beam;

	private float start_diff;

	private bool status_effect_on_enemy = true;
	private GameObject enemy_with_status_effect;

	int check_target_timer = 0;


	void Start () {
		start_diff = (target_object.transform.position - transform.position).magnitude;
		enemy_with_status_effect = target_object;


	}
	
	// Update is called once per frame
	void Update () {
		if (check_target_timer%1==0)
			check_target ();
		check_target_timer += 1;

	}

	void check_target(){
		Vector3 diff = target_object.transform.position - transform.position;
		float dist = diff.magnitude;

		Ray ray = new Ray (transform.position, diff);
		RaycastHit hit;
		int layer_mask = ((1<<target_object.layer) | (1<<Player.environment_layer));// | Physics.IgnoreRaycastLayer);

		if (Physics.Raycast (ray, out hit, diff.magnitude, layer_mask)) {

			transform.LookAt (hit.point, transform.up);

			float diff_ratio = (hit.point - transform.position).magnitude / start_diff;
			transform.localScale = new Vector3 (1, 1, diff_ratio);

			transform.Rotate (new Vector3 (0, 0, Time.deltaTime * 15));

			GameObject go = hit.collider.gameObject;

			if (go.layer == Player.environment_layer) {
				if (status_effect_on_enemy) {
					Spaceship s_old = Spaceship.get_spaceship (enemy_with_status_effect);
					if (s_old != null) {
						s_old.remove_statuseffects(beam.module_effects);
						status_effect_on_enemy = false;
					}
				}
				return;
			}

			Spaceship s = Spaceship.get_spaceship (go);
			if (s != null) {
				if (status_effect_on_enemy) {
					if (enemy_with_status_effect != s.gameObject) {
						Spaceship s1 = Spaceship.get_spaceship (enemy_with_status_effect);
						if (s1 != null) {
							s1.remove_statuseffects(beam.module_effects);
						}
							
						s.apply_statuseffects(beam.module_effects);
						enemy_with_status_effect = s.gameObject;
					}
				} else {
					status_effect_on_enemy = true;
					s.apply_statuseffects(beam.module_effects);
				}
			}



		} else {
			Destroy (gameObject);
		}
	}

	void OnDestroy(){
		if (enemy_with_status_effect) {
			Spaceship s = Spaceship.get_spaceship (enemy_with_status_effect);
			if (s) {
				s.remove_statuseffects (beam.module_effects);
			}
		}
	}
}
