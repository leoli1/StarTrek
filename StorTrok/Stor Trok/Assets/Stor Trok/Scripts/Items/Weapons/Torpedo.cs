using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TorpedoType{
	StandardM1,
	QuantenTorpedo
}

[CreateAssetMenu(menuName = "Game-Items/Weapons/Torpedo")]
public class Torpedo : Weapon { // id = 2000


	public TorpedoType torpedo_type;

	//public GameObject torpedo_object;

	[HideInInspector]
	public GameObject target_object;
	[HideInInspector]
	public int enemy_layer;

	public Torpedo(){
		arc_range = 150;
	//	AllTorpedos.Add (this);
		weapon_object = Weapons.weapons == null ? null : Weapons.weapons.torpedo;
	}

	public override void animation ()
	{
		GameObject t = GameObject.Instantiate (weapon_object);

		t.transform.position = this.weapon_position.transform.position;

		TorpedoUpdate tu = t.GetComponent<TorpedoUpdate> ();
		if (tu == null)
			t.AddComponent<TorpedoUpdate> ();
		tu.target = this.target_object;
		tu.enemy_layer = this.enemy_layer;
		tu.torpedo = this;
		weapon_color.a = 1;
		t.GetComponent<Renderer> ().material.color = weapon_color;
		t.GetComponent<Renderer> ().material.SetColor ("_EmissionColor", weapon_color);
		ParticleSystem ps = t.GetComponentInChildren<ParticleSystem> ();
		if (ps) {
			var m = ps.main;
			m.startColor = weapon_color;
		}

		Vector3 variation = Utils.random_vector3 (0.4f);

		t.GetComponent<Rigidbody> ().velocity = (this.weapon_position.transform.TransformVector (this.weapon_position.GetComponent<SpaceshipWeaponPosition> ().local_direction) + variation).normalized*
			Spaceship.get_spaceship (this.weapon_position).GetComponent<Rigidbody> ().velocity.magnitude * 2.5f;
	}
	public override void deal_damage (int layer)
	{
	}
}
