using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapons : MonoBehaviour {

	public GameObject weapon_impact_effect;

	public static Weapons weapons;
	[Header("Phaser")]
	public GameObject phaser;
	[Header("Torpedos")]
	public GameObject torpedo;
	public GameObject quanten_torpedo;
	[Header("Puls-Phaser")]
	public GameObject puls_phaser;

	public GameObject emp_effect;

	public Weapons(){
		Weapons.weapons = this;
	}

	void Start () {
		Weapons.weapons = this;
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
