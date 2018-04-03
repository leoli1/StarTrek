using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum EffectTypes{
	Speed,
	RotationSpeed,
	Schilde,
	DamageOutput,
	Stunned,
	Weapon
}

[System.Serializable]
public class StatusEffect {

	public static List<StatusEffect> status_effect_presets = new List<StatusEffect>{
		new StatusEffect { name = "Slow", description = "Deine Geschwindigkeit ist reduziert", duration = 2.5f, effect_type = EffectTypes.Speed, effect_intensity = 0.75f, multiple_effects_intensity_multplier = 0.75f },
		new StatusEffect { name = "Speed Buff", description = "Geschwindigkeit ist erhöht", duration = 1.5f, effect_type = EffectTypes.Speed, effect_intensity = 1.3f, multiple_effects_intensity_multplier = 0.8f}
	};

	public string name;
	public string description;

	[HideInInspector]
	public float start_time;
	public float duration;

	public EffectTypes effect_type;

	/// <summary>
	/// Wie dieses effect type dieses attribute des spaceships beeinflusst, 1 ist standardmäßig keine beeinflussung, >1 ein buff, <1 ein debuff
	/// </summary>
	public float effect_intensity;

	/// <summary>
	/// mehrere effekte des gleichen typs haben nicht dieselbe auswirkung auf die gesamte intensität, sondern sie nimmt mit der anzahl an effekten ab(/zu)
	/// </summary>
	public float multiple_effects_intensity_multplier;

	public static float complete_effects_intensity(EffectTypes type, List<StatusEffect> effects){
		float intensity = 1;
		float mul = 1;
		foreach (StatusEffect ef in effects) {
			if (ef.effect_type == type) {
				intensity *= ef.effect_intensity;
				mul /= ef.multiple_effects_intensity_multplier; //<-- TODO
			}
		}
	//	Debug.Log ("Intensity: " + intensity.ToString () + " slows: " + num + " mul: " +mul);
		return intensity*mul;
	}
	public static bool is_stunned(List<StatusEffect> effects){
		foreach (StatusEffect ef in effects) {
			if (ef.effect_type == EffectTypes.Stunned)
				return true;
		}
		return false;
	}
}
