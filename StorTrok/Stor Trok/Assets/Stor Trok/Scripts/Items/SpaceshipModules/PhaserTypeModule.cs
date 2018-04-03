using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Game-Items/SpaceshipModules/PhaserTypeModule")]
public class PhaserTypeModule : SpaceshipModule {

	public Color phaser_beam_color;

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

	Phaser create_phaser(){
		Phaser phaser_object = Phaser.CreateInstance<Phaser>();

		phaser_object.base_damage = 0;
		phaser_object.status_effects = module_effects;
		phaser_object.weapon_color = phaser_beam_color;
		phaser_object.weapon_position = cast_object;

		phaser_object.start_pos = cast_object;
		phaser_object.end_pos = target_object;

		phaser_object.phaser_anim_lifetime = module_effects[0].duration;
		return phaser_object;
	}
	public override void do_effect ()
	{
		if (!check_effect ())
			return;

		Phaser phaser_object = create_phaser ();

		phaser_object.deal_damage (target_object.layer);
		phaser_object.animation ();

		if (impact_effect != null) {
			Spaceship s = Spaceship.get_spaceship (target_object);
			Transform p = target_object.transform;
			if (s != null) {
				p = s.schild_collision_object.transform;
			}
			GameObject g = GameObject.Instantiate (impact_effect, p);
			g.transform.localPosition = Vector3.zero;

			GameObject.Destroy(g, module_effects[0].duration+0.1f);

			Renderer r = g.GetComponent<Renderer> ();
			if (r != null) {
				r.material.SetColor ("_TintColor", phaser_beam_color);//color = phaser_beam_color;
			}
		}

		last_time_used = Time.time;
	}

	public override void animation ()
	{
		throw new System.NotImplementedException ();
	}
}
