using UnityEngine;
using System.Collections;

public class SchildScript : MonoBehaviour {

	public Vector3 point_position;
	protected Color point_color = new Color(0,0,1,1);
	protected Color normal_color = new Color(0.2f,0.2f,1,0.05f);

	protected bool is_explosion = false;

	float anim_start = 0;
	public const float max_explosion_radius = 7;
	public const float explosion_radius_growth = 42;
	float explosion_radius = 1;

	Mesh mesh;




	// Use this for initialization
	void Start () {
		mesh = this.GetComponent<MeshFilter> ().mesh;
	}

	// Update is called once per frame
	void Update () {
		Color[] m_colors = new Color[mesh.vertices.Length];
		for (int i = 0; i < mesh.vertices.Length; i++) {
			Vector3 p = mesh.vertices [i];
			Vector3 scale = transform.localScale;
			Vector3 vert_position = transform.TransformPoint(p);
			float dist = Vector3.Distance (point_position, vert_position);

			float v = Mathf.Clamp (dist / explosion_radius, 0, 1);

			if (is_explosion) {
				m_colors [i] = Color.Lerp (point_color, normal_color, v);
			} else {
				try {
					m_colors[i] = Color.Lerp(mesh.colors[i], normal_color, Mathf.Clamp (dist / max_explosion_radius, 0.5f, 1));
				} catch {
					m_colors [i] = normal_color;
				}
			}
		}
		mesh.colors = m_colors;

		if (is_explosion) {
			explosion_radius += Time.deltaTime*explosion_radius_growth;
		}
		if (explosion_radius > max_explosion_radius) {
			reset_explosion ();
		}
	}

	public void reset_explosion(){
		is_explosion = false;
		explosion_radius = 1;
	}


	public void col(Vector3 pos){
		anim_start = Time.time;

		is_explosion = true;
		point_position = pos;
	}
}
