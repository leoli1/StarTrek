using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KlingWarbird : MonoBehaviour {

	Spaceship spaceship;

//	private GameObject selected_enemy;
	private bool attack = false;

	ComputerPlayer computer_player;

	private Vector3 last_known_player_coordinates;

	void Start () {
		spaceship = this.GetComponent<Spaceship> ();
		computer_player = this.GetComponent<ComputerPlayer> ();

		if (computer_player == null)
			print ("no computer player class attached to " + name);
	
		computer_player.disable_movement ();
	}


	void Update () {
		Spaceship s = Spaceship.get_spaceship (computer_player.selected_enemy);
		if (s != null) {
			if (s.is_cloaking) {
				last_known_player_coordinates = computer_player.selected_enemy.transform.position;
				computer_player.selected_enemy = null;
			}
			if (s.destroyed) {
				computer_player.disable_movement ();
				spaceship.auto_navigate_to_point (spaceship.start_pos);
				computer_player.selected_enemy = null;
			}
		}
		if (attack && computer_player.selected_enemy != null) {
			spaceship.attack_enemy (computer_player.selected_enemy, Player.player_layer, WeaponRawTypes.All);
			computer_player.enable_movement ();

		} else {
			computer_player.disable_movement ();
			attack = true;

			if (Vector3.Distance (PlayerScript.playerScript.gameObject.transform.position, transform.position) <= Spaceship.max_attack_distance && !Player.player.spaceship.destroyed && !Player.player.spaceship.is_cloaking) {
				computer_player.selected_enemy = PlayerScript.playerScript.gameObject;
			} else if (Player.player.spaceship.is_cloaking) {
				spaceship.auto_navigate_to_point (last_known_player_coordinates);
			}

		}

		if (spaceship.destroyed)
			this.enabled = false;
	}
}
