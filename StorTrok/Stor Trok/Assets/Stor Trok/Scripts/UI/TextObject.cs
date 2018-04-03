using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TextObject : MonoBehaviour {

	public float size = 10;
	public bool lookat_cam = false;
	void Start () {
		Vector3 position = transform.position;
		Text text;
		text = GetComponent<Text> ();
		int fontsize = 200;
		if (text != null) {
			size = text.fontSize / text.rectTransform.localScale.x;
			text.rectTransform.sizeDelta = Vector2.one *fontsize*size*2;
			text.fontSize = fontsize;
			text.rectTransform.localScale = Vector3.one * size / fontsize;
		} else {
			TextMesh tm = GetComponent<TextMesh> ();
			tm.fontSize = fontsize;
			tm.transform.localScale = Vector3.one * size / fontsize;
		}
		transform.position = position;
	}
	
	// Update is called once per frame
	void Update () {
		if (lookat_cam) {
			transform.LookAt (Camera.main.transform.position);
			transform.Rotate (transform.up, 180);
			Vector3 rot = transform.localRotation.eulerAngles;
			transform.localRotation = Quaternion.Euler(new Vector3 (rot.x, rot.y, 0));
		}
	}
}
