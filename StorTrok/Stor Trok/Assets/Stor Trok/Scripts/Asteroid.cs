using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Asteroid : MonoBehaviour {

	public GameObject target_object;
	public float damage;
	public float shield_pen;

	public float acceleration = 100;
	void Start () {
		Rigidbody r = GetComponent<Rigidbody> ();
		if (target_object == null) {
			r.constraints = RigidbodyConstraints.FreezeAll;
			gameObject.isStatic = true;
			return;
		}
		r.AddForce ((target_object.transform.position-transform.position).normalized*acceleration*r.mass);
		r.AddTorque ((new Vector3 (Random.Range (-1, 1), Random.Range (-1, 1), Random.Range (-1, 1))).normalized*10000*r.mass);
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void OnCollisionEnter(Collision col){
		GameObject g = col.collider.gameObject;
		if (g == target_object) {
			if (Utils.is_destroyable_object (g)) {
				DestroyableObject d = DestroyableObject.get_destroyable_object (g);
				d.apply_damage (damage);
			} else if (Utils.is_spaceship (g)) {
				Spaceship s = Spaceship.get_spaceship (g);
				float angle = s.get_angle (col.contacts[0].point);

				Schild schild = s.schild;
				SchildPartTypes sp = schild.get_schildpart_at_angle (angle);

				float dmg_to_si = Weapon.calc_damage (damage, shield_pen, schild.get_shield_intensity_at_part (sp));//schild.get_shield_intensity_at_part (sp));
				float dmg_to_shields = damage;// - dmg_to_si;
				s.apply_damage (dmg_to_shields, sp, dmg_to_si);
			}
			GetComponent<DestroyableObject> ().destroy ();
		}
	}
}
