using UnityEngine;
using System.Collections;

public class Grid : MonoBehaviour {

	public float tile_size = 10;
	public Material grid_material;
	void Start () {
		if (grid_material == null) {
			grid_material = this.GetComponent<MeshRenderer> ().material;
		}
		grid_material.mainTextureScale = new Vector2 (transform.localScale.x / tile_size, transform.localScale.z / tile_size);
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
