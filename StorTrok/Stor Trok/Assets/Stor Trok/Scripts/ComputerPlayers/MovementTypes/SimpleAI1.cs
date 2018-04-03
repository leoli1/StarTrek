using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleAI1 : MonoBehaviour {

	Spaceship spaceship;
	public bool do_movement_ai;
	void Start () {
		spaceship = Spaceship.get_spaceship (gameObject);
	}

	float dist_to_player {
		get { 
			return Vector3.Distance (transform.position, PlayerScript.playerScript.gameObject.transform.position);
		}
	}
	GameObject player {
		get {
			return PlayerScript.playerScript.gameObject;
		}
	}
	
	void simpleAI1(){
		float d = dist_to_player;
		float angle = Vector3.Angle (transform.forward, player.transform.position - transform.position);
		if (d > 300 || angle>135) {
			if (!spaceship.is_auto_navigating || (spaceship.is_auto_navigating && spaceship.auto_navigation_target_object==null)) {
				spaceship.auto_navigate_to_object (player);
			}
		} else if (d<200){
			spaceship.abort_auto_navigation ();
			spaceship.set_target_speed (3.5f);
			transform.Rotate (new Vector3 (0, spaceship.rotation_speed * Time.deltaTime, 0));
		}
	}

	void Update () {
		if (spaceship.warping_in || !do_movement_ai)
			return;
		simpleAI1 ();
	}
}
