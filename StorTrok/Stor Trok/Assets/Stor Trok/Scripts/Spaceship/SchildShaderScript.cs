using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public struct ExplosionPoint{ // ein zentrum von dem aus eine farbveränderung ausgeht
	public Vector3 point;
	public float explosion_radius;
}

public class SchildShaderScript : MonoBehaviour { // animation auf dem schild bei kontakt mit phaser, torpedos...

	public List<ExplosionPoint> explosion_points = new List<ExplosionPoint>();
	public Color point_color = new Color(0,0,1,1);
	public Color normal_color = new Color(0.1f,0.1f,1,0.08f);

	protected bool is_explosion = false;

	public float max_explosion_radius = 9;
	public float explosion_radius_growth = 42;
	public float start_explosion_radius = 5;
	//float explosion_radius = 1;

	Color[] m_colors;// = new Color[mesh.vertices.Length];

	Mesh mesh;

	// Use this for initialization
	void Start () {
	//	System.GC.SuppressFinalize (this);
	//	System.Runtime.GCSettings.LatencyMode = System.Runtime.GCLatencyMode.LowLatency;
		mesh = this.GetComponent<MeshFilter> ().mesh;
		//this.enabled = false;

		m_colors = new Color[mesh.vertices.Length];

		update_colors ();
	}
		
	void Update () {
		//update_colors ();
	}

	public void update_colors(){
		for (int i = 0; i < mesh.vertices.Length; i++) {
			Vector3 p = mesh.vertices [i];
			Vector3 vert_position = transform.TransformPoint (p);
			if (is_explosion) {
				ExplosionPoint nearest_point = explosion_points[0];
				for (int k=1; k<explosion_points.Count; k++){
					if (Vector3.Distance (explosion_points [k].point, vert_position) < Vector3.Distance (nearest_point.point, vert_position)) {
						nearest_point = explosion_points [k];
					}
				}
				float dist = Vector3.Distance (nearest_point.point, vert_position);

				float v = Mathf.Clamp (dist / nearest_point.explosion_radius, 0, 1);

				m_colors [i] = Color.Lerp (point_color, normal_color, v);

			} else {
				try {
					m_colors[i] = Color.Lerp(mesh.colors[i], normal_color, 0.6f);
				} catch {
					m_colors [i] = normal_color;
				}
			}
		}
		mesh.colors = m_colors;

		for (int i = 0; i < explosion_points.Count; i++) {
			ExplosionPoint p = explosion_points[i];
			p.explosion_radius += Time.deltaTime * explosion_radius_growth;
			if (explosion_points [i].explosion_radius > max_explosion_radius) {
				explosion_points.Remove (explosion_points [i]);
				i--;
			}
		}
		if (explosion_points.Count == 0) {
			is_explosion = false;
		}

	}

	public void col(Vector3 pos){
		is_explosion = true;
		explosion_points.Add(new ExplosionPoint{
			point=pos, 
			explosion_radius=start_explosion_radius
		});
	}
}