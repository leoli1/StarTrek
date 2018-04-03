using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum MissionStatus{
	Inactive,
	Active,
	Completed,
	Failed
}

public class Mission : MonoBehaviour {

	public static Mission current_mission;

	[Header("Mission Attributes")]
	public PlayerEnvironmentStatus environment_type;
	public FullMission fullMission;

	public GameObject camera_start_point;

	public MissionStatus mission_status;

	void Awake () {
		Mission.current_mission = this;
		mission_status = MissionStatus.Inactive;
	}
	void Update () {
		
	}

	public void disable_all_enemies(){
		foreach (MissionPart p in fullMission.mission_parts) {
			foreach (MissionGoal g in p.mission_goals) {
				if (g.mission_goal_type == MissionGoalTypes.DestroySpaceship) {
					g.target.SetActive (false);
				}
			}
		}
	}

	public void load_new_mission_part(MissionPart m, bool keep_destroyed_spaceships){
		disable_all_enemies ();

		GameObject enemy_look_at = null;
		bool camera_focus = false;
		if (m == null) {
			return;
		}
		if (keep_destroyed_spaceships) {
			foreach (MissionPart mp in fullMission.mission_parts) {
				if (mp == fullMission.get_current_mission_part ())
					break;
				foreach (MissionGoal g in mp.mission_goals) {
					if (g.mission_goal_type == MissionGoalTypes.DestroySpaceship) {
						g.target.SetActive (true);
					}
				}
			}
		}

		foreach (MissionGoal g in m.mission_goals) {
			g.target.SetActive (true);
			if (g.mission_goal_type == MissionGoalTypes.DestroySpaceship) {
				Spaceship s = Spaceship.get_spaceship (g.target);
				if (s != null) {
					s.set_layer (Player.enemy_layer);
				}
			} else if (g.mission_goal_type == MissionGoalTypes.DestroyObject) {
				DestroyableObject s = DestroyableObject.get_destroyable_object (g.target);
				if (s != null) {
					s.gameObject.layer = Player.enemy_layer;
				}
			} else if (g.mission_goal_type == MissionGoalTypes.ProtectTarget) {
				g.goal_achieved = true;
				Spaceship s = Spaceship.get_spaceship (g.target);
				if (s != null) {
					s.set_layer (Player.player_layer);
				} else {
					DestroyableObject so = DestroyableObject.get_destroyable_object (g.target);
					if (so != null) {
						so.gameObject.layer = Player.player_layer;
					}
				}
			}

			if (g.warps_in_at_missionpart_start) {
				print (g.target.name + " warps in!");
				g.target.GetComponent<Spaceship> ().warp_in ();
			}
			if (g.focus_camera_on_object_at_missionpart_start) {
				enemy_look_at = g.target;
				camera_focus = true;
			}

		}
		UI.interactiveMessage.enable (InteractiveMessageContentType.StartMission);
		if (enemy_look_at != null && camera_focus) {
			Camera.main.transform.position = camera_start_point.transform.position;
			CameraScript.camera_script.lookat_object = enemy_look_at;
			CameraScript.camera_script.lookat_player_and_controllable = false;
			UI.ui.disableStandardUI ();
			Spaceship s = enemy_look_at.GetComponent<Spaceship> ();
			if (s == null) {
				Invoke ("start_change_camera_focus", 2.5f);
			}
			UI.ui.mission_title.text = m.title;
			UI.ui.mission_title.gameObject.SetActive (true);
		}

		Minimap.minimap.check_objects ();
	}

	public void start_change_camera_focus(){
		Camera.main.transform.localRotation = Quaternion.identity;
		CameraScript.camera_script.lookat_player_and_controllable = true;
		CameraScript.camera_script.lookat_object = null;
		//UI.ui.enableUI ();
		UI.ui.enableStandardUI();
		UI.ui.mission_title.gameObject.SetActive (false);

		if (fullMission.get_current_mission_part() == fullMission.first_mission_part) {

			List<Spaceship> enemies = Spaceship.get_active_enemy_spaceships ();
			Transform enemy;
			if (enemies.Count > 0) {
				enemy = enemies [0].transform;
			} else {
				enemy = DestroyableObject.get_active_objects (ObjectGroupType.Enemy) [0].transform;
			}
			CameraScript.camera_script.lookat_point (enemy.position);
		}
	}

	public void check_status(){
		if (mission_status == MissionStatus.Completed || mission_status == MissionStatus.Failed)
			return;

		MissionPart cur_part = fullMission.get_current_mission_part ();

		bool mission_finished = true;
		bool mission_failed = false;

		foreach (MissionGoal g in cur_part.mission_goals) {
			mission_finished = !g.goal_achieved ? false : mission_finished;
			if (g.mission_goal_type == MissionGoalTypes.ProtectTarget) {
				if (g.target.is_destroyed()){
					mission_failed = true;
				}
			}
		}
		if (mission_finished && !mission_failed) {
			print (cur_part.title + " geschafft");
			if (fullMission.next_mission_part == null) {
				print (fullMission.name + " geschafft");
				mission_status = MissionStatus.Completed;
				UI.ui.death_timer.SetActive (false);
				quit_mission ();
				return;
			} else {
				Invoke ("load_next_missionpart", 2);
			}
		}

		if (mission_failed) {
			mission_status = MissionStatus.Failed;
			quit_mission ();
		}

	}

	void load_next_missionpart(){
		MissionPart p = fullMission.next_mission_part;
		fullMission.current_mission_part_id = p.ID;
		load_new_mission_part (p, true);
	}

	public void quit_mission(){
		UI.interactiveMessage.enable (InteractiveMessageContentType.QuitMission);
		print ("mission completed/failed");
	}

	public void start_mission(){
		mission_status = MissionStatus.Active;
	}
}
