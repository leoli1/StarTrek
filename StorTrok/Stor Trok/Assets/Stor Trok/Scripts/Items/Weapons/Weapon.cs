using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public enum PlayerWeaponPositions{ // waffen positionen des spielers ( andere schiffe, z.b. borg, haben noch mehr positionen)
	Top = 1,
	Bot = 2,
	All = Top|Bot
}

public enum WeaponRawTypes{ // die groben waffen typen
	None = 1,
	Phaser = 2,
	Torpedo = 4,
	PulsPhaser = 8,
	All = WeaponRawTypes.Phaser | WeaponRawTypes.Torpedo | WeaponRawTypes.PulsPhaser
}

public abstract class Weapon : Item {


	public const float shield_half_dmg_reduction = 13;

	public PlayerWeaponPositions special_weapon_position = PlayerWeaponPositions.All;
	public RawRaumschiffType special_raw_spaceship_type = RawRaumschiffType.All;

	[HideInInspector]
	public float damage_output_multiplier = 1;

	public float arc_range; // in welchem winkel dieses geschütz schießen kann, z.b 360°, 180° etc
	public float base_damage; // schaden des geschützes
	public float shield_pen; // schild-durchdringung,
	public float reload_time = 1; // wie lange das geschütz braucht, um neu zu schießen

	public GameObject weapon_object;
	public Color weapon_color;
	[HideInInspector]
	public float last_time_shot = -10; // wann das geschütz das letzte mal geschossen hat

	public float damage{
		get{ 
			return base_damage * damage_output_multiplier;
		}
	}

	[HideInInspector]
	public GameObject weapon_position;

	//public List<StatusEffect> status_effects = new List<StatusEffect>();
	public StatusEffect[] status_effects; 


	public Weapon(){
		this.item_type = ItemType.Weapon;
	}

	public virtual bool can_shoot(){
		return Time.time - last_time_shot > reload_time; // wenn diff zwischen jetzt und dem letzten schuss größer als reload zeit ist, kann geschossen werden
	}

	public virtual void shoot(){
		last_time_shot = Time.time;
		animation ();
	}

	public static float calc_damage(float damage, float shield_pen, float shield){
		float actual_shield = shield * (1 - shield_pen);
		return damage * (shield_half_dmg_reduction / (shield_half_dmg_reduction + actual_shield));
	}
	public float calc_damage(float shield){
		float actual_shield = shield * (1 - shield_pen);
		//float shield_dmg_reduction = 1-shield_half_dmg_reduction / (shield_half_dmg_reduction + actual_shield);
		//return this.damage * (1 - shield_dmg_reduction);
		return this.damage*(shield_half_dmg_reduction/(shield_half_dmg_reduction+actual_shield));
	}

	public override string get_description_text ()
	{
		string s = this.name+"\nSchaden: "+this.base_damage.ToString();
		return s;
	}

	public void apply_status_effects(Spaceship s){ // <------- call in deal_damage!!
		if (status_effects == null)
			return;
		foreach (StatusEffect e in status_effects) {
			s.apply_statuseffect (e);
		}
	}

	public abstract void animation (); // was passiert, wenn dieses geschütz abgefeuert wurde (phaser, torpedo etc)
	public abstract void deal_damage (int layer);

	public void weapon_impact_effect(Vector3 local_position, Transform parent, Color color){
		GameObject ef = GameObject.Instantiate (Weapons.weapons.weapon_impact_effect, parent);
		ef.transform.localPosition = local_position;
		ef.transform.localScale = ef.transform.localScale;

		ParticleSystem ps = ef.GetComponent<ParticleSystem> ();
		var main = ps.main;
		color.a = 1;
		main.startColor = color;

		//ef.transform.rotation = Quaternion.LookRotation (parent.TransformPoint (local_position) - parent.position);

		Destroy (ef, 2);
	}
}
