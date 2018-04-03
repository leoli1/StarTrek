using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ToggleButton : MonoBehaviour {

	new public bool enabled = false;
	public Image enabled_image;

	public Color enabledColor = new Color (0, 0, 0, 0);
	public Color notEnabledColor = new Color(1,1,1,0.5f);
	void Start () {
		enabled_image.color = enabled ? enabledColor : notEnabledColor;
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void OnClick(){
		enabled = !enabled;
		enabled_image.color = enabled ? enabledColor : notEnabledColor;
	}
}
