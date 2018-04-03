using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyableObject : MonoBehaviour {

	public static List<DestroyableObject> active_destroyable_objects = new List<DestroyableObject>();

	public static DestroyableObject get_destroyable_object(GameObject o){
		if (o == null)
			return null;
		if (o.GetComponent<DestroyableObject> () != null)
			return o.GetComponent<DestroyableObject> ();
		DestroyableObject[] s = o.GetComponentsInParent<DestroyableObject> (true);
		return s.Length > 0 ? s [0] : null;
	}
	public static List<DestroyableObject> get_active_objects(ObjectGroupType type){
		List<DestroyableObject> g = new List<DestroyableObject> ();
		foreach (DestroyableObject o in active_destroyable_objects) {
			if (o.object_group_type == type)
				g.Add (o);
		}
		return g;
	}

	public new string name;
	public float max_hitpoints;
	public float hitpoints;

	public ObjectGroupType object_group_type;

	public bool minimap_icon_created = false;

	public bool destroyed = false;

	void Start () {
		hitpoints = max_hitpoints;
	}

	void OnEnable(){
		active_destroyable_objects.Add (this);
	}
	void OnDisable(){
		active_destroyable_objects.Remove (this);
	}

	void Update () {
		
	}
	public void destroy(){
		hitpoints = 0;
		if (destroyed)
			return;
		destroyed = true;

		Minimap.minimap.check_objects ();

		GameObject ex = GameObject.Instantiate (OtherPrefabObjects.otherPrefabObjects.ship_explosion);
		ex.transform.position = transform.position;

		GameObject.Destroy (ex, 3);

		if (this == DestroyableObject.get_destroyable_object(PlayerScript.playerScript.selected_enemy)) {
			PlayerScript.playerScript.selected_enemy = null;
		}

		foreach (MissionGoal g in Mission.current_mission.fullMission.get_current_mission_part().mission_goals) {
			if (g.mission_goal_type == MissionGoalTypes.DestroyObject && g.target == gameObject) {
				g.goal_achieved = true;
			}
		}

		Invoke("destroy_object", 0.5f);

		Mission.current_mission.check_status ();
	}
	void destroy_object(){
		gameObject.SetActive (false);
	}

	void check_destroyed(){
		if (hitpoints <= 0) {
			destroy ();
		}
	}

	public void apply_damage(float dmg){
		hitpoints -= dmg;
		check_destroyed ();

	}
}
