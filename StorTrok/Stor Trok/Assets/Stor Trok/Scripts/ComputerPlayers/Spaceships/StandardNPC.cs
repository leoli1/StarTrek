using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class StandardNPC : MonoBehaviour {

	Spaceship spaceship;
	ComputerPlayer computer_player;


	WaitForSeconds enemy_update_delay;

	void Start () {
		spaceship = this.GetComponent<Spaceship> ();
		computer_player = this.GetComponent<ComputerPlayer> ();

		if (computer_player == null)
			print ("no enemy class attached to " + name);

		enemy_update_delay = new WaitForSeconds (3);
		Invoke ("start_scanning",Random.Range(0,5f));
	}

	void start_scanning(){
		StartCoroutine (scan_for_enemy());
	}
	
	GameObject get_nearest_enemy(){
		GameObject nearest = null;
	
		foreach (Spaceship s in computer_player.get_enemies()) {
			if (nearest == null || Vector3.Distance (transform.position, s.transform.position)<Vector3.Distance (transform.position, nearest.transform.position)) {
				nearest = s.gameObject;
			}
		}
		return nearest;
	}

	IEnumerator scan_for_enemy() {
		while (true) {
			computer_player.selected_enemy = get_nearest_enemy ();
			yield return enemy_update_delay;
		}
	}

	void Update () {
		if (computer_player.selected_enemy != null && computer_player.selected_enemy.is_destroyed ()) 
			computer_player.selected_enemy = get_nearest_enemy ();
		if (computer_player.selected_enemy != null) {
			spaceship.attack_enemy (computer_player.selected_enemy, computer_player.get_enemy_layer(), WeaponRawTypes.All);
		}
	}
}