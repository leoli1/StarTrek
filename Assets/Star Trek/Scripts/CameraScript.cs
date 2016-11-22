using UnityEngine;
using System.Collections;

public class CameraScript : MonoBehaviour {

	public Vector3 offset = new Vector3 (0, 0, 0);
	public GameObject player;

	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		transform.position = player.transform.position + offset;

		Quaternion idealRotation = Quaternion.LookRotation(player.transform.position-transform.position);
		transform.rotation = Quaternion.Slerp(transform.rotation, idealRotation, Time.timeScale);
	}
}
