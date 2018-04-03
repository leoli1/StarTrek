using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum WeaponEffectTypes{
	MultiplePhaserTargets,
	PhaserSalve,
	TorpedoSalve
}

public class WeaponEffect : StatusEffect {
	public WeaponEffectTypes weapon_effect_type;
	public float subsequent_weapon_damage_multiplier;
	public int max_extra_objects;

	public int objects_fired = 0;

	public bool enabled = false;
}
