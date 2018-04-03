using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TimeScale : MonoBehaviour {

	private InputField input_field;
	void Start () {
		input_field = GetComponent<InputField> ();
	}
	
	// Update is called once per frame
	void Update () {
		if (LevelManager.levelManager.game_paused) {
			Time.timeScale = 0;
		} else {
			try {
				Time.timeScale = float.Parse (input_field.text);
			} catch (System.FormatException) {
				Time.timeScale = 1;
			}
		}
	}
}
