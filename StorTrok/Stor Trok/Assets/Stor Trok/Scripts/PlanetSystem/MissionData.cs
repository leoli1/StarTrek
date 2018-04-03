using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "SceneData/MissionData")]
public class MissionData : ScriptableObject {

	public static List<MissionData> all_misson_datas = new List<MissionData>();

	public int ID;
	public int planet_system_ID;

	public string scene_name;

	public string title;
	[Multiline]
	public string description;

	public MissionData(){
		all_misson_datas.Add (this);
	}
}
