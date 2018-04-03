using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Speedometer : MonoBehaviour {

	public GameObject background;
	public GameObject zero_line;
	public int rand = 0;
	public int width = 20;
	RectTransform bar_rt;
	void Start () {
		bar_rt = this.GetComponent<RectTransform> ();
		RectTransform rt = background.GetComponent<RectTransform> ();

		bar_rt.sizeDelta = new Vector2 (width, 5*width);
		rt.sizeDelta = new Vector2 (bar_rt.sizeDelta.x + 2 * rand, bar_rt.sizeDelta.y + 3 * rand);
		rt.pivot = new Vector2 (rand / rt.sizeDelta.x, rand / rt.sizeDelta.y);

		zero_line.GetComponent<RectTransform> ().localPosition = new Vector3(0, width,0);
		zero_line.GetComponent<RectTransform> ().sizeDelta = new Vector2 (width + 2 * rand, 2);
		zero_line.GetComponent<RectTransform> ().pivot = new Vector2 (rand / rt.sizeDelta.x, 0.5f);


		bar_rt.sizeDelta = new Vector2 (width, width);
		bar_rt.localPosition = new Vector3 (0, width, 0);
	}
	
	// Update is called once per frame
	void Update () {
		set_bar ();
	}

	void set_bar(){
		float speed = Player.player.spaceship.speed;
		//bar_rt.sizeDelta = new Vector2 (width, Mathf.Abs(speed) * width);
		bar_rt.localScale = new Vector3(1,speed,1);
		if (speed >= 0) {
			//bar_rt.localPosition = new Vector3 (0, width, 0);
		} else {
			//bar_rt.localPosition = new Vector3 (0, 0, 0);
			//bar_rt.rotation = Quaternion.Euler (new Vector3 (0, 0, 180));
		}
	}
}
