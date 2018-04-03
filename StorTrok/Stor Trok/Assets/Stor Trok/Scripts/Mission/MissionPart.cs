using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class MissionEnemy{
	public Spaceship enemy;
	public bool warps_in_at_missionpart_start;
}


public enum MissionGoalTypes{
	None,
	DestroySpaceship,
	DestroyObject,
	ProtectTarget
}

[System.Serializable]
public class MissionGoal{
	public GameObject target;
	public MissionGoalTypes mission_goal_type;
	public bool warps_in_at_missionpart_start;
	public bool focus_camera_on_object_at_missionpart_start;

	[HideInInspector]
	public bool goal_achieved = false;
}


[System.Serializable]
public class MissionPart {
	public int ID;
	public string title;
	[Multiline]
	public string description;

	//public List<MissionEnemy> enemies_to_destroy;
	public List<MissionGoal> mission_goals;
}
