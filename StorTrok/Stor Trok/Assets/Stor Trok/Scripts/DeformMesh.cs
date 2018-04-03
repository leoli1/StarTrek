using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeformMesh : MonoBehaviour {
	/// <summary>
	/// Mesh des Objekts
	/// </summary>
	private Mesh mesh;

	/// <summary>
	/// Da mehrere Vertices, die auf der exakt gleichen Position sind, in die selbe Richtung verschoben werden sollen, wird jeder Startposition eine Richtung zugeordnet und nicht jedem vertx
	/// </summary>
	private Dictionary<Vector3, Vector3> vertex_directions;

	public bool deform_on_awake = false;
	public float deform_factor;
	public bool change_color = true;

	Material material;
	Vector3[] start_vertices;
	Vector3[] vertices;

	void Awake () {
		//create_mesh ();
		if (deform_on_awake)
			deform ();
	}
		
	/// <summary>
	/// Verformt das Mesh, auf dem dieses Script ist, indem alle Vertices in zufällige Richtungen verschoben werden.
	/// Zusätzlich wird noch die Farbe verändert
	/// </summary>
	public void deform(){
		vertex_directions = new Dictionary<Vector3, Vector3>();
		StartCoroutine (deform_mesh ());
	}

	IEnumerator deform_mesh(){
		mesh = this.GetComponent<MeshFilter> ().mesh;
		material = this.GetComponent<Renderer> ().material;

		start_vertices = mesh.vertices;
		Color start_color = material.GetColor ("_Color");
		Color target_color = new Color (0.35f, 0.35f, 0.35f);
			
	//	vertex_directions = new Vector3[mesh.vertices.Length];
		foreach (Vector3 vertex in mesh.vertices) {
			if (!vertex_directions.ContainsKey(vertex))
				vertex_directions.Add (vertex, Utils.random_vector3(Random.Range(0.9f,1.6f)));
		}

		//for (int i = 0; i < vertex_directions.Length; i++) {
		//	vertex_directions[i] = Utils.random_vector3 (1);
		//}
	
		mesh.MarkDynamic ();

		float start_time = Time.time;
		float duration = 2;

		while (Time.time - start_time < duration) {
			vertices = mesh.vertices;
			for (int i = 0; i < start_vertices.Length; i++) {
				vertices [i] += vertex_directions[start_vertices[i]] * Time.deltaTime * 0.3f * deform_factor;//*duration/(Time.time-start_time+0.1f);
			}
			mesh.vertices = vertices;

			material.color = Color.Lerp (start_color, target_color, (Time.time - start_time) / duration);

			yield return null;
		}
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
