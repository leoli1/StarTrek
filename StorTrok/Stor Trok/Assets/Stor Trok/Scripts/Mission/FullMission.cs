using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class FullMission {
	
	public int ID;
	public string name;
	[Multiline]
	public string description;

	public int current_mission_part_id;

	public List<MissionPart> mission_parts;

	public MissionPart get_current_mission_part(){
		foreach (MissionPart p in mission_parts) {
			if (p.ID == 0) {
				Debug.Log (p.title + " mission part no id");
				break;
			}
			if (p.ID == current_mission_part_id) {
				return p;
			}
		}
		return null;
	}

	public MissionPart next_mission_part {
		get{ 
			for (int i = 0; i < mission_parts.Count; i++) {
				if (mission_parts [i].ID == current_mission_part_id && i!=mission_parts.Count-1) {
					return mission_parts [i + 1];
				}
			}
			return null;
		}
	}

	public MissionPart first_mission_part {
		get{ 
			return mission_parts.Count == 0 ? null : mission_parts[0];
		}
	}
	public MissionPart last_mission_part {
		get{ 
			return mission_parts[mission_parts.Count-1];
		}
	}
}
