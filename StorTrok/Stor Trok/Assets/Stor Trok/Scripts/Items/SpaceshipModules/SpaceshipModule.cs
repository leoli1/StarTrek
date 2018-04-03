using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ModuleTargets{
	Self,
	Ally,
	Enemy
}
	
[CreateAssetMenu(menuName = "Game-Items/SpaceshipModules/SimpleModule")]
public class SpaceshipModule : Item { // 6000

	public static T get_module_of_type<T>(List<SpaceshipModule> modules) where T : SpaceshipModule{
		foreach (SpaceshipModule m in modules) {
			if (m.GetType () == typeof(T)) {
				return (T)m;
			}
		}
		return null;
	}
		
	public StatusEffect[] module_effects;	
	public ModuleTargets target_type;

	public GameObject graphics_object;
	public GameObject impact_effect;

	[HideInInspector]
	public GameObject target_object;
	[HideInInspector]
	public GameObject cast_object;

	public float use_cooldown;

	public bool can_be_used_in_battle = true;
	public bool can_be_used_while_cloaking = false;
	//public AntriebType antrieb_type;

	[HideInInspector]
	public float last_time_used = -1000;

	public override string get_description_text ()
	{
		throw new System.NotImplementedException ();
	}

	public SpaceshipModule(){
		item_type = ItemType.SpaceshipModule;
	}

	public bool can_be_used(){
		return Time.time - last_time_used > use_cooldown;
	}
	/// <summary>
	/// Gets the target object. (Only Player!!!)
	/// </summary>
	/// <returns>The target object.</returns>
	public GameObject get_target_object(){
		switch (target_type) {
		case ModuleTargets.Self:
			return Player.player.spaceship.gameObject;
		case ModuleTargets.Ally:
			throw new System.NotImplementedException ();
		case ModuleTargets.Enemy:
			return PlayerScript.playerScript.selected_enemy;
		}
		return null;
	}

	public virtual bool check_effect(){
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
		if (!can_be_used_while_cloaking && s.is_cloaking && this.GetType()!=typeof(CloakingDevice)) {
			s.decloak ();
		}
		return true;
	}

	public virtual void do_effect(){
		if (!check_effect ())
			return;
		last_time_used = Time.time;
		animation ();
	}

	public void apply_effects(){
		Spaceship s = Spaceship.get_spaceship (target_object);
		if (s != null)
			s.apply_statuseffects (module_effects);
	}
	public void apply_effect(StatusEffect ef){
		Spaceship s = Spaceship.get_spaceship (target_object);
		if (s != null)
			s.apply_statuseffect (ef);
	}

	public virtual void animation (){
		apply_effects ();
	}

	/*public void add_module_object_to_spaceship(){
		Spaceship s = Spaceship.get_spaceship (cast_object);
		s.current_active_modules.Add(
	}*/
}