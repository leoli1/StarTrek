using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class SpaceshipWeaponPosition : MonoBehaviour{


	public Vector3 local_direction;

	public List<Weapon> weapons = new List<Weapon>();

	public List<Transform> phaser_positions_path;

	public List<Transform> puls_phaser_weapons_positions;

	public int capacity = 1;

	public void Start(){
		if (local_direction==Vector3.zero) {
			Debug.Log ("ERRRRrrrrrrrrrrrrrrooooooooorrr");
		}
	}
}
