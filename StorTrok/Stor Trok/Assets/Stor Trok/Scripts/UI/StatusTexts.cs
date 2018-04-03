using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StatusTexts : MonoBehaviour {

	public static StatusTexts status_texts;

	public GameObject StatusText;
	public float lifeTime = 3;
	public int y = 50;
	const int y_diff = 20;

	List<GameObject> texts;

	void Awake () {
		StatusTexts.status_texts = this;
		texts = new List<GameObject> ();
	}

	public void new_text(string text){
		GameObject t = GameObject.Instantiate (StatusText);

		t.GetComponent<Text> ().text = text;
		t.transform.SetParent(transform);
		t.GetComponent<RectTransform> ().localPosition = new Vector3 (0, y, 0);

		//Invoke ("add_y", lifeTime);
		//y -= y_diff;

		GameObject.Destroy (t, lifeTime);
		t.GetComponent<Text> ().CrossFadeAlpha (0, lifeTime, false);

		List<GameObject> rem = new List<GameObject> ();
		foreach (GameObject g in texts) {
			if (g == null) {
				rem.Add (g);
				continue;
			}
			Vector3 pos = g.transform.position;
			//g.transform.position = 
			pos.y += y_diff;
			g.transform.position = pos;
		}
		foreach (GameObject r in rem) {
			texts.Remove (r);
		}
		texts.Add (t);
	}

	void add_y(){
		y += y_diff;
	}
}
