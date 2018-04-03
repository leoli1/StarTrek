using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BorgCube : MonoBehaviour {

	Spaceship spaceship;

	private bool attack = false;

	[HideInInspector]
	public ComputerPlayer computer_player;

	void Start () {
		spaceship = this.GetComponent<Spaceship> ();
		computer_player = this.GetComponent<ComputerPlayer> ();

	}
		

	void Update () {
		if (attack && computer_player.selected_enemy != null) {
			if (Spaceship.get_spaceship (computer_player.selected_enemy).is_cloaking || Spaceship.get_spaceship(computer_player.selected_enemy).destroyed) {
				computer_player.selected_enemy = null;
			} else {
				spaceship.attack_enemy (computer_player.selected_enemy, Player.player_layer, WeaponRawTypes.All);
			}
		} else {
			attack = true;
			if (!Player.player.spaceship.is_cloaking) {
				computer_player.selected_enemy = PlayerScript.playerScript.gameObject;
			}
		}
		if (spaceship.destroyed)
			this.enabled = false;
	}
}
