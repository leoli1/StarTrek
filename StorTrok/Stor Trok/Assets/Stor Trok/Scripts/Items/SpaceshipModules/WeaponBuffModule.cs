using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Game-Items/SpaceshipModules/WeaponBuffModule")]
public class WeaponBuffModule : SpaceshipModule {

	public WeaponEffectTypes weapon_effect_type;

	public float subsequent_weapon_damage_multiplier = 1;

	public int max_extra_objects = -1;

	public WeaponEffect effect{
		get{ 
			WeaponEffect weapon_effect = new WeaponEffect {
				duration = module_effects [0].duration,
				effect_type = EffectTypes.Weapon,
				subsequent_weapon_damage_multiplier = subsequent_weapon_damage_multiplier,
				max_extra_objects = max_extra_objects
			};
			weapon_effect.weapon_effect_type = weapon_effect_type;
			return weapon_effect;
		}
	}

	public override void animation ()
	{
		apply_effect (effect);
	}
}
