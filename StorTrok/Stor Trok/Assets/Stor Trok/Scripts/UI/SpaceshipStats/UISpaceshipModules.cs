using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UISpaceshipModules : MonoBehaviour {
	public static UISpaceshipModules ui_spaceship_modules;

	[Tooltip("Button Prefab")]
	public GameObject spaceship_module_button;

	private Vector3 x_y_min_localposition;

	public Image background;

	void Start () {
		ui_spaceship_modules = this;
		x_y_min_localposition = new Vector3 (-130, 210, 0);

		update_spaceship_module_buttons ();
		background.rectTransform.localPosition = x_y_min_localposition + new Vector3 (-50, 0, 0);
	}
	
	public void update_spaceship_module_buttons(){
		foreach (UISpaceshipModuleButton btn in UISpaceshipModuleButton.allUISpaceshipModuleButtons) {
			Destroy (btn.gameObject);
		}
		UISpaceshipModuleButton.allUISpaceshipModuleButtons = new List<UISpaceshipModuleButton> ();

		int size = 64;
		int offset = 3;

		List<SpaceshipModule> spaceship_modules = Player.player.spaceship.modules;

		if (spaceship_modules.Count == 0) {
			gameObject.SetActive (false);
			return;
		}
		gameObject.SetActive (true);
		for (int i = 0; i < spaceship_modules.Count; i++) {
			GameObject g = GameObject.Instantiate (spaceship_module_button, transform);
			g.GetComponent<RectTransform> ().localPosition = x_y_min_localposition + new Vector3 (((int)(size*spaceship_module_button.transform.localScale.x) + offset) * i, -16, 0);
			g.GetComponent<RectTransform> ().sizeDelta = new Vector2 (size, size);
			g.GetComponent<UISpaceshipModuleButton> ().spaceship_module = spaceship_modules [i];
		}
	}
	void Update () {
		
	}
}
