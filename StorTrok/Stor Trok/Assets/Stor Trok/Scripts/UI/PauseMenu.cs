using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseMenu : MonoBehaviour {

	public GameObject help_canvas;

	public void resume_button(){
		LevelManager.levelManager.set_pause_state (false);
	}

	public void main_menu_button(){
		LevelManager.levelManager.load_new_scene ("StartScene");
	}
	public void help_button(){
		help_canvas.SetActive (!help_canvas.activeSelf);
	}
}
