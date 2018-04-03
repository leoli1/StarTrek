using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UISpaceshipModuleButton : MonoBehaviour {

	public static List<UISpaceshipModuleButton> allUISpaceshipModuleButtons = new List<UISpaceshipModuleButton> ();

	public SpaceshipModule spaceship_module;

	public GameObject cooldown_obj;

	void Start () {
		allUISpaceshipModuleButtons.Add (this);
		cooldown_obj.GetComponent<RectTransform> ().sizeDelta = this.GetComponent<RectTransform> ().sizeDelta;
		set_image ();
	}
	void OnDestroy(){
		allUISpaceshipModuleButtons.Remove (this);
	}

	public void set_image(){
		this.GetComponent<Image> ().sprite = spaceship_module == null ? ItemButton.standard_tex : spaceship_module.inventory_tex;
	}

	void Update () {
		if (spaceship_module != null) {
			float t = Mathf.Clamp (1 - ((Time.time - spaceship_module.last_time_used) / spaceship_module.use_cooldown), 0, 1);
			cooldown_obj.transform.localScale = new Vector3 (1, t, 1);
		} else {
			cooldown_obj.transform.localScale = new Vector3 (1, 0, 1);
		}
	}

	public void on_click(){
		if (spaceship_module.can_be_used ()) {
			spaceship_module.target_object = spaceship_module.get_target_object ();
			spaceship_module.cast_object = PlayerScript.playerScript.spaceship.get_top_weapon_position ().gameObject;
			spaceship_module.do_effect ();
		}
	}
}
