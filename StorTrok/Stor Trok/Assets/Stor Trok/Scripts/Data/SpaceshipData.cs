using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class SpaceshipData { // daten über ein raumschiff

	public int spaceshipID; // id

	public int raumschiff_schild;
	public int impuls_antrieb;
	public int warp_antrieb;

	//module
	public List<int> modules;

	//waffen
	public List<int> top_weapons;
	public List<int> bot_weapons;

	public SpaceshipData(){
	}
}
