using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OtherPrefabObjects : MonoBehaviour {
	public static OtherPrefabObjects otherPrefabObjects;
	public GameObject drop_object;
	public GameObject interactive_message_button;
	public GameObject text3D;
	public GameObject ship_explosion;

	// Use this for initialization
	void Awake () {
		OtherPrefabObjects.otherPrefabObjects = this;
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
