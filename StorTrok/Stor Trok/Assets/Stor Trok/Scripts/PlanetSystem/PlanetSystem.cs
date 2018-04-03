using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlanetSystem : MonoBehaviour {

	public PlanetSystemData system_data;

	public ObjectAura objectAura;


	void Start () {
		GameObject text = GameObject.Instantiate (OtherPrefabObjects.otherPrefabObjects.text3D, transform);
		text.transform.position = transform.position + Vector3.up * 157;
		text.GetComponent<TextMesh> ().text = system_data.system_name;
	}

	void Update () {
	}

	public MissionData mission_data{
		get{ 
			return system_data.mission_data;
		}
	}
}
