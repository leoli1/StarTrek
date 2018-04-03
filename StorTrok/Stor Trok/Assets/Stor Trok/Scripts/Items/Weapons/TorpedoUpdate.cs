using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class TorpedoUpdate : MonoBehaviour {

	public GameObject target;
	public int enemy_layer;

	private Rigidbody r;

	public const float speed = 300;

	public const float life_time = 20;

	public Torpedo torpedo;

	void Start () {
		r = this.GetComponent<Rigidbody> ();
		//transform.LookAt (target.transform);
		Destroy (gameObject, life_time);
	}
	
	// Update is called once per frame
	void Update () {
		if (target == null) {
			this.enabled = false;
			return;
		}
		if (target.is_destroyed())
			return;
		Vector3 t_speed = (target.transform.position - transform.position).normalized * speed;
	
		r.velocity = Vector3.Lerp (r.velocity, t_speed, Time.deltaTime*4);
	}

	public void OnTriggerEnter(Collider col){
		if (col.gameObject.layer == enemy_layer) {
			if (col.gameObject == target || col.gameObject.GetComponentInParent<Spaceship> ().gameObject == target) {
				GameObject go = col.gameObject;

				Vector3 hit_point = transform.position;



				if (go.GetComponent<SchildShaderScript> ()) {
					//go.GetComponent<SchildShaderScript> ().col (hit_point);
					torpedo.weapon_impact_effect (go.transform.InverseTransformPoint (hit_point), go.transform, go.GetComponent<SchildShaderScript>().normal_color);
				}
				//torpedo.weapon_impact_effect (go.transform.InverseTransformPoint (hit_point), go.transform);

				Spaceship spaceship = Spaceship.get_spaceship (go);
				if (spaceship != null) {
					Vector3 p = transform.position;

					float angle = spaceship.get_angle (p);

					Schild schild = spaceship.schild;
					SchildPartTypes sp = schild.get_schildpart_at_angle (angle);

					float dmg_to_si = torpedo.calc_damage (schild.get_shield_intensity_at_part (sp));
					float dmg_to_shields = torpedo.damage;// - dmg_to_si;
					spaceship.apply_damage (dmg_to_shields, sp, dmg_to_si);
					torpedo.apply_status_effects (spaceship);

					destroy ();
				} else {
					DestroyableObject d = DestroyableObject.get_destroyable_object (go);
					if (d != null) {
						d.apply_damage (torpedo.damage);
						destroy ();
					}
				}
			}
		} else if (col.gameObject.layer == Player.environment_layer) {
			destroy ();
		}
	}
	void destroy(){
		/*AudioSource a = GetComponent<AudioSource> ();
		if (a != null) {
			//a.Play ();
			AudioSource.PlayClipAtPoint(a.clip, transform.position);
		}*/
		AudioMaster.play_random_sound_effect (SoundEffectTypes.Explosion, transform.position);
		Destroy (gameObject);
	}
}
