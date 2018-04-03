using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DeathTimer : MonoBehaviour {

	public float timer_start;

	public const float death_timer = 10;

	public Text timer_text;
	public Button respawn_button;

	void OnEnable () {
		timer_start = Time.time;
		respawn_button.interactable = false;
		if (!Player.player.spaceship.destroyed) {
			gameObject.SetActive (false);
		}
	}
	
	// Update is called once per frame
	void Update () {
		float rem_time = Mathf.Clamp (death_timer-(Time.time - timer_start), 0, death_timer);

		timer_text.text = "Zeit bis zur Wiederbelebung: " + (Mathf.Round(rem_time*10)/10f).ToString () + "s";
		if (rem_time == 0) {
			respawn_button.interactable = true;
		}
	}

	void reset_player(){
		GameObject.Destroy (PlayerScript.playerScript.gameObject);
		LevelManager.levelManager.create_player_space ();
		UIWeaponStats.uiweaponstats.update_weapon_images ();
	}

	public void respawn_button_clicked(){
		reset_player ();

		gameObject.SetActive (false);
	}
}
