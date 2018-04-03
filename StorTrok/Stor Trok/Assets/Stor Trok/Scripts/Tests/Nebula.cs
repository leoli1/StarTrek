using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Nebula : MonoBehaviour {

	public GameObject nebula_plane;
	public int number_of_planes = 20;

	void Start () {
		for (int i = 0; i < number_of_planes; i++) {
			GameObject g = GameObject.Instantiate (nebula_plane, transform);
			g.transform.rotation = Random.rotation;
		}
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
