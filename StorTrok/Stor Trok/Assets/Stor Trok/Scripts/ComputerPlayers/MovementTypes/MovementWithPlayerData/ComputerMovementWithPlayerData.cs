using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(ComputerPlayer))]
public class ComputerMovementWithPlayerData : MonoBehaviour {

	Spaceship spaceship;

	//[Tooltip("Ab welcher Distanz, dieses Raumschiff automatisch wieder zum Spieler zurückkehrt")]
	[System.NonSerialized]
	public float return_to_enemy_distance = 1000;
	//[Tooltip("Beim Erreichen dieser Distanz wird der Auto-Navigator abgeschaltet und der normale Algorithmus wird wieder benutzt")]
	[System.NonSerialized]
	public float return_to_enemy_target_distance = 500;

	private bool returning_to_player = false;

	private bool do_movement = false;

	private ComputerPlayer computer_player;

	void Start () {
		spaceship = this.GetComponent<Spaceship> ();
		computer_player = this.GetComponent<ComputerPlayer> ();

		if (!PlayerMovementData.data_set_loaded)
			PlayerMovementData.load_data_set ();


		Invoke ("enable_movement", Random.Range (0, 2f));
	}

	void enable_movement(){
		do_movement = true;
	}

	float dist_to_enemy {
		get { 
			return Vector3.Distance (transform.position, enemy.transform.position);
		}
	}
	GameObject enemy {
		get {
			return computer_player.selected_enemy;
		}
	}
	float angle_to_player{
		get{ 
			return Vector3.Angle (transform.forward, enemy.transform.position - transform.position);
		}
	}

	void Update () {
		if (spaceship.warping_in || computer_player.selected_enemy == null)
			return;
		if (spaceship.destroyed)
			enabled = false;
		if (do_movement)
			movement ();
	}

	void movement(){
		if (dist_to_enemy>return_to_enemy_distance){
			returning_to_player = true;
			spaceship.auto_navigate_to_object (enemy);
		}
		if (dist_to_enemy <= return_to_enemy_target_distance)
			returning_to_player = false;
		if (returning_to_player) {
			return;
		}
		SpaceshipRelativeSituation situation = new SpaceshipRelativeSituation ();
		situation.relative_position = new SerializableVector3(transform.position-PlayerScript.playerScript.spaceship.transform.position);
		situation.rotation = new SerializableVector3(transform.rotation.eulerAngles);
		PlayerMovementData data = PlayerMovementData.find_best_match (situation);
		if (data == null)
			return;
		foreach (MovementInputKeys key in data.player_input.rotation_input) {
			spaceship.rotation_input (key);
		}
		spaceship.set_target_speed (data.player_input.speed);
		spaceship.abort_auto_navigation ();
		spaceship.fix_rotation ();
	}

	void OnDrawGizmos(){
	/*	if (PlayerScript.playerScript == null || spaceship.destroyed == true)
			return;
		Gizmos.DrawLine (transform.position, player.transform.position);
		Gizmos.DrawLine (transform.position, transform.position + transform.forward * 100);
		Gizmos.DrawLine (transform.position, transform.position + (Vector3.Cross (transform.forward.normalized, (player.transform.position - transform.position).normalized).normalized) * 100);*/
	}


}
