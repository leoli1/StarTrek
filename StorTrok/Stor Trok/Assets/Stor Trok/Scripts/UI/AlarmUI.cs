using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AlarmUI : MonoBehaviour {

	public static AlarmUI alarmUI;

	public int size = 100;
	public float fade_duration = 1;

	public Image left;
	public Image bot;
	public Image right;
	public Image top;
	public List<Image> allImages;

	Color normal_color = new Color (1, 0, 0, 1);
	Color red_color = new Color (1, 0, 0, 0.2f);

	void OnEnable () {
		alarmUI = this;

		left.rectTransform.position = new Vector3 ();
		left.rectTransform.sizeDelta = new Vector2 (size, Screen.height-size);

		bot.rectTransform.position = new Vector3 (Screen.width, 0, 0);
		bot.rectTransform.sizeDelta = new Vector2 (Screen.width-size, size);

		right.rectTransform.position = new Vector3 (Screen.width, Screen.height, 0);
		right.rectTransform.sizeDelta = new Vector2 (size, Screen.height-size);

		top.rectTransform.position = new Vector3 (0, Screen.height, 0);
		top.rectTransform.sizeDelta = new Vector2 (Screen.width-size, size);

		allImages = new List<Image> () {
			left,
			bot,
			right,
			top
		};

		set_color (normal_color);
		set_alpha (0);
	}
		
	public void set_alpha(float alpha){
		foreach (Image i in allImages) {
			i.canvasRenderer.SetAlpha (alpha);
		}
	}
	public void set_color(Color c){
		foreach (Image i in allImages) {
			i.color = c;
		}
	}
	public void fade_color(Color end_color, float duration){
		foreach (Image i in allImages) {
			//i.CrossFadeAlpha(1, duration, false);
			i.CrossFadeColor(end_color, duration, false, true);
		}
	}

	void alarm1(){
		set_alpha (0);
		fade_color (red_color, fade_duration);
	}
	void alarm2(){
		set_alpha (red_color.a);
		Color c = normal_color;
		c.a = 0;
		fade_color (c, fade_duration);
	}
	
	public void alarm(){
		alarm1 ();
		Invoke ("alarm2", fade_duration);
	}
	void Update () {

	}
}
