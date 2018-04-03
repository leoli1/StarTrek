using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhaserUpdate : MonoBehaviour {

	[HideInInspector] public GameObject start_pos; // startpunkt des beams
	[HideInInspector] public GameObject end_pos; // endpunkt des beams
	public LineRenderer line_renderer; //line renderer component
	public ParticleSystem ps;
	public ParticleSystem ps_start_effect;

	void Start () {

		AudioMaster.play_random_sound_effect (SoundEffectTypes.Phaser, start_pos.transform.position, 2);
		if (!line_renderer) {
			line_renderer = this.GetComponent<LineRenderer> ();
		}

		Color color = line_renderer.startColor;

		ps = GetComponentInChildren<ParticleSystem> ();
		if (ps != null) {
			Vector3 diff = end_pos.transform.position - start_pos.transform.position;
			float dist = diff.magnitude;
			ps.transform.position = start_pos.transform.position + diff * 0.5f;

			Quaternion new_rot = Quaternion.LookRotation (diff);
			ps.transform.rotation = new_rot;

			var m = ps.main;
			m.maxParticles = (int)(dist) * 4;
			m.startColor = color;


			var shape = ps.shape;
			shape.enabled = true;
			shape.scale = new Vector3(0.1f,0.1f,dist);

		}
		if (ps_start_effect != null) {
			var main = ps_start_effect.main;
			main.startColor = color;
		}
		
	}
	
	// Update is called once per frame
	void Update () {
		if (start_pos == null)
			return;
		
		line_renderer.SetPositions (new Vector3[]{ 
			start_pos.transform.position,
			end_pos.transform.position
		});

		Vector3 diff = end_pos.transform.position - start_pos.transform.position;
		ps.transform.position = start_pos.transform.position + diff * 0.5f;

		Quaternion new_rot = Quaternion.LookRotation (diff);
		ps.transform.rotation = new_rot;

		var shape = ps.shape;
		shape.scale = new Vector3(0.1f,0.1f,diff.magnitude);

		ps_start_effect.transform.position = start_pos.transform.position;
		if (end_pos.is_destroyed ())
			Destroy (gameObject);
	}
}
