using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinimapObject : MonoBehaviour {

	public static List<MinimapObject> all_minimap_objects = new List<MinimapObject>();

	public Transform object_to_represent;
	public bool use_minimap_object_size = true;

	void Start () {
		all_minimap_objects.Add (this);
		transform.localScale = Vector3.one * Minimap.minimap_object_size;
	}

	void Update () {
		Vector3 position = new Vector3 (object_to_represent.position.x, transform.position.y, object_to_represent.position.z);
		transform.position = position;
		transform.rotation = Quaternion.Euler (new Vector3 (0, object_to_represent.transform.rotation.eulerAngles.y, 0));
	}
}
