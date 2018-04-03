using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ObjectAuraType{
	PlanetSystem,
	ErdeRaumdock,
	ItemDrop,
	ShopExit
}

public class ObjectAura : MonoBehaviour {

	public static List<ObjectAura> objectAurasPlayerIsIn = new List<ObjectAura> ();
	public bool player_in_aura;
	public ObjectAuraType objectAuraType;

	[HideInInspector]
	public bool interactive_message_generated = false;
	public List<InteractiveMessageButton> interactive_message_buttons = new List<InteractiveMessageButton> ();

	[HideInInspector]
	public bool check_distance = false;

	[HideInInspector]
	public float radius;

	void OnEnable(){
	/*	Collider c = GetComponent<Collider> ();
		c.enabled = false;*/
		Invoke ("enable_playercheck", 0.4f);
		radius = GetComponent<SphereCollider> ().radius * transform.lossyScale.x;

	}
	void enable_playercheck(){
		check_distance = true;
	/*	if (Vector3.Distance (Player.player.spaceship.gameObject.transform.position, transform.position) < radius) {
			create_message ();
		}*/
	}

	/*public void OnTriggerEnter(Collider col){
		if (col.tag == "Player" && use_trigger) {
			create_message ();
		}
	}*/

	void Update(){
		if (!check_distance)
			return;
		if (Vector3.Distance (LevelManager.levelManager.player_gameObject.transform.position, transform.position) < radius) {
			if (!interactive_message_generated) {
				create_message ();
			}
		} else if (interactive_message_generated) {
			destroy_message ();
		}
	}

	void create_message(){
		player_in_aura = true;
		objectAurasPlayerIsIn.Add (this);
		if (objectAuraType == ObjectAuraType.ItemDrop) {
			UI.interactiveMessage.enable (InteractiveMessageContentType.ItemDrop);
		} else {
			UI.interactiveMessage.enable (InteractiveMessageContentType.LoadScene);
		}
	}
	/*public void OnTriggerExit(Collider col){
		if (col.tag == "Player") {
			destroy_message ();
		}
	}*/

	void destroy_message(){
		foreach (InteractiveMessageButton btn in interactive_message_buttons) {
			UI.interactiveMessage.remove_button (btn);
		}
		interactive_message_buttons = new List<InteractiveMessageButton> ();
		objectAurasPlayerIsIn.Remove (this);
		interactive_message_generated = false;
		if (objectAurasPlayerIsIn.Count == 0)
			player_in_aura = false;
		if (UI.interactiveMessage.buttons.Count==0)
			UI.interactiveMessage.gameObject.SetActive (false);
	}

	void OnDestroy(){
		objectAurasPlayerIsIn.Remove (this);
		interactive_message_generated = false;
		if (objectAurasPlayerIsIn.Count == 0)
			player_in_aura = false;
	}
}
