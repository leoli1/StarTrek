using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RaumschiffShop : MonoBehaviour {

	public GameObject arrow;
	public float arrow_height;

	[HideInInspector]
	public Vector3 arrow_target_position;

	public GameObject[] raumschiffe;

	void Awake () {
		foreach (GameObject g in raumschiffe) {
			g.GetComponent<Spaceship> ().enabled = false;
		}
		arrow_height = arrow.transform.position.y;
		update_arrow ();
	}

	void Update () {

		arrow.transform.position = Vector3.Lerp (arrow.transform.position, arrow_target_position, 0.5f);

		if (Input.GetMouseButtonDown (0)) {
			Ray ray = Camera.main.ViewportPointToRay (new Vector3 (0.5f, 0.5f, 0));
			RaycastHit hit;
			if (Physics.Raycast (ray, out hit)) {
				Spaceship spaceship = Spaceship.get_spaceship (hit.collider.gameObject);
				if (spaceship != null) {
					Raumschiff r_data = spaceship.raumschiff;
					int id = r_data.ID;
					bool owned = spaceship_owned (id);
					if (!owned)
						LevelManager.player_data.add_spaceship (id);
					
					LevelManager.player_data.current_spaceship = id;

					GameObject.Destroy (LevelManager.levelManager.raumschiff_object);
					LevelManager.levelManager.spawn_spaceship ();
				
					LevelManager.save_player_data ();

					RaumschiffInventory.raumschiffInventory.reset_buttons ();
					RaumschiffInventory.raumschiffInventory.set_buttons ();

					print ("Spaceship changed to : " + id + "; " + Raumschiff.get_raumschiff_by_id (id).name);

					LevelManager.load_player_data ();

					update_arrow ();

				}
			}
		}
	}
	bool spaceship_owned(int id){
		return LevelManager.player_data.owned_spaceships_with_items.FindIndex(spaceshipData => spaceshipData.spaceshipID == id) != -1; 
	}

	void update_arrow() {
		int spaceship_id = LevelManager.player_data.current_spaceship;
		GameObject spaceship = null;
		foreach (GameObject g in raumschiffe) {
			if (Spaceship.get_spaceship (g).raumschiff.ID == spaceship_id) {
				spaceship = g;
				break;
			}
		}
		if (spaceship == null) {
			print ("error");
			return;
		}

		arrow_target_position = new Vector3 (spaceship.transform.position.x, arrow_height, spaceship.transform.position.z);
	}
}

