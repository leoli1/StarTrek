using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraViewporttest : MonoBehaviour {

	// Use this for initialization
	void Start () {
		transform.position = Camera.main.ViewportToWorldPoint (new Vector3 (1, 0, 0));
	}
	
	// Update is called once per frame
	void Update () {
		Vector3 v = Camera.main.ViewportToWorldPoint (new Vector3 (1, 0.5f, 5f));//-Camera.main.transform.position;
		//v.y = 0;
		//v.Normalize ();
		transform.position = v;// * 5 + Camera.main.transform.position;
	}
}
