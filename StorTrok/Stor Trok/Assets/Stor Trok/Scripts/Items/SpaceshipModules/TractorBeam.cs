using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Game-Items/SpaceshipModules/TractorBeam")]
public class TractorBeam : SpaceshipModule {

	public float effect_end_width = 10;

	public TractorBeam(){
		target_type = ModuleTargets.Enemy;
	}

	public override bool check_effect(){
		Spaceship s = Player.player.spaceship;
		if (!can_be_used_in_battle) {
			if (s.in_battle) {
				StatusTexts.status_texts.new_text (name + " kann nicht im Kampf benutzt werden");
				return false;
			}
		}
		if (target_object == null) {
			StatusTexts.status_texts.new_text ("Kein Ziel spezifiziert");
			return false;
		}

		if (Vector3.Distance (target_object.transform.position, cast_object.transform.position) > Spaceship.max_attack_distance) {
			StatusTexts.status_texts.new_text ("Ziel außer Reichweite");
			return false;
		}

		if (!can_be_used_while_cloaking && s.is_cloaking) {
			s.decloak ();
		}
		return true;
	}

	public override void animation ()
	{
		float diff = (target_object.transform.position - cast_object.transform.position).magnitude;
		float angle = Mathf.Atan (effect_end_width * 0.5f / diff)*Mathf.Rad2Deg;


		GameObject gfx = GameObject.Instantiate (graphics_object, cast_object.transform);

		gfx.GetComponent<TractorBeamUpdate> ().target_object = target_object;
		gfx.GetComponent<TractorBeamUpdate> ().beam = this;

		gfx.transform.localPosition = Vector3.zero;
		gfx.transform.LookAt (target_object.transform.position);

		ParticleSystem ps = gfx.GetComponent<ParticleSystem>();


		var main = ps.main;
		main.startLifetime = diff;
		//main.startSpeed = Mathf.Sqrt(diff*diff+effect_end_width*effect_end_width*0.25f)*2;
		//main.startLifetimeMultiplier = 0.5f;

		var shape = ps.shape;
		shape.angle = angle;

		ps.Clear ();
	//	ps.Simulate (module_effect.duration);
		ps.Simulate (module_effects[0].duration);

		Spaceship s = Spaceship.get_spaceship (target_object);
		if (s != null) {
			//s.apply_statuseffect (module_effect);
			s.apply_statuseffects(module_effects);
		}

		//Destroy (gfx, module_effect.duration);
		Destroy(gfx, module_effects[0].duration+0.1f);

	}
}
