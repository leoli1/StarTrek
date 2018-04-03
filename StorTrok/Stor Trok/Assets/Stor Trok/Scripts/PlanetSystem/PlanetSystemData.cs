using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "SceneData/PlanetSystemData")]
public class PlanetSystemData : ScriptableObject {
	public static List<PlanetSystemData> all_planet_systems = new List<PlanetSystemData>();

	public static PlanetSystemData get_planet_system_data_by_id(int id){
		foreach (PlanetSystemData data in PlanetSystemData.all_planet_systems) {
			if (data.ID == id)
				return data;
		}
		return null;
	}

	public int ID;
	public string system_name;

	[Multiline]
	public string description;

	public string scene_name;

	public bool only_mission = false;
	public MissionData mission_data;



	public PlanetSystemData(){
		all_planet_systems.Add (this);
	}
}
