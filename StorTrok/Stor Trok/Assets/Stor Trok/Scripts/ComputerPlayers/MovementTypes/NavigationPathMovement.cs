using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(ComputerPlayer))]
public class NavigationPathMovement : MonoBehaviour {

	Spaceship spaceship;

	public NavigationPath navigation_path;

	[HideInInspector]
	public bool finished_following_path = false;


	void Start () {
		spaceship = Spaceship.get_spaceship (gameObject);

		if (navigation_path.waypoints.Length == 0) {
			GetComponent<ComputerPlayer> ().disable_movement ();
		}
	}

	void Update () {
		if (spaceship.destroyed)
			this.enabled = false;
		follow_path ();
	}

	void follow_path(){
		if (spaceship.warping_in) {
			return;
		}

		if (!spaceship.is_auto_navigating) {
			spaceship.auto_navigate_to_object (navigation_path.waypoints [navigation_path.current_target_waypoint].gameObject, navigation_path.target_speed);
		}

		if (Vector3.Distance (navigation_path.waypoints [navigation_path.current_target_waypoint].position, transform.position) <= Spaceship.auto_navigation_target_distance) {
			navigation_path.current_target_waypoint += 1;

			spaceship.abort_auto_navigation ();

			if (navigation_path.current_target_waypoint == navigation_path.waypoints.Length) {
				if (navigation_path.looping) {
					navigation_path.current_target_waypoint = 0;
				} else {
					finished_following_path = true;
					spaceship.set_target_speed (0);
					GetComponent<ComputerPlayer> ().disable_movement ();
				}
			}
		}
	}

	void OnDrawGizmosSelected(){
		if (navigation_path == null || navigation_path.waypoints == null || navigation_path.waypoints.Length == 0)
			return;
		
		Gizmos.color = Color.green;

		Vector3 last_point = transform.position;
		for (int i = navigation_path.current_target_waypoint; i < navigation_path.waypoints.Length; i++) {
			if (navigation_path.waypoints [i] == null)
				break;
			Gizmos.DrawLine (last_point, navigation_path.waypoints [i].transform.position);
			last_point = navigation_path.waypoints [i].transform.position;
		}
		if (navigation_path.looping) {
			Gizmos.DrawLine (last_point, navigation_path.waypoints [0].position);
		}
	}
}
