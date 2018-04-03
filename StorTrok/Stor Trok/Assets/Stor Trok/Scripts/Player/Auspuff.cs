using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Auspuff : MonoBehaviour {

	public LineRenderer line_renderer; // LineRenderer component des auspuffs

	public int number_of_positions = 60; // wie viele letzte positition des auspuffs aufgezeichnet werden

	public const bool use_unity_trailrenderer = true;

	void Start () {
		if (line_renderer == null) {
			line_renderer = this.GetComponent<LineRenderer> ();
		} // sofern line_renderer noch nicht im editor angegeben wurde, wird er sich so geholt
		TrailRenderer tr;
		if (use_unity_trailrenderer) {
			tr = gameObject.AddComponent<TrailRenderer> ();
			tr.material = line_renderer.material;
			tr.time = 2;
			tr.startColor = line_renderer.startColor;
			tr.endColor = line_renderer.endColor;
			line_renderer.enabled = false;
			this.enabled = false;
		}

		if (LevelManager.levelManager != null && LevelManager.levelManager.scene_name == "SektorenRaum") {
			line_renderer.startWidth *= LevelManager.levelManager.sektorenraum_scale_factor;
			line_renderer.endWidth *= LevelManager.levelManager.sektorenraum_scale_factor;
			if (use_unity_trailrenderer) {
				tr.endWidth *= LevelManager.levelManager.sektorenraum_scale_factor;
				tr.startWidth *= LevelManager.levelManager.sektorenraum_scale_factor;
			}
		}

		Vector3[] start_positions = new Vector3[number_of_positions]; // alle positionen des LineRenderes zu beginn
		for (int i = 0; i < number_of_positions; i++) {
			start_positions [i] = transform.position;
		}
		line_renderer.positionCount = number_of_positions;
		line_renderer.SetPositions(start_positions);

	}

	void Update () {
		Vector3 new_position = transform.position; // neue position des raumschiffs/auspuffs
		Vector3[] new_positions = new Vector3[number_of_positions];  // die entsprechenden neuen positionen für den linerenderer
		for (int i = 0; i < number_of_positions-1; i++) { // alte positionen werden um eine stelle nach hinten verschoben
			new_positions[i+1] = line_renderer.GetPosition(i);
		}
		new_positions [0] = new_position; // und die erste position ist die neue
		line_renderer.SetPositions (new_positions);
	}
}
