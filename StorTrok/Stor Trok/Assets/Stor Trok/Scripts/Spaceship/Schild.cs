using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum SchildPartTypes{
	Top,
	Right,
	Left,
	Bot,
	None
}

[System.Serializable]
public class SchildPart{
	public SchildPartTypes part;
	public float shield_intensity;
	public Range angle_range;
}
[System.Serializable]
public class Schild { // informationen über den schild des raumschiffs

	public static Range top_range = new Range(-45, 45);
	public static Range right_range = new Range(45,135);
	public static Range bot_range = new Range (135, 225);
	public static Range left_range = new Range (225, 315);


	public SchildPart[] shield_parts = new SchildPart[]{
		new SchildPart{
			part = SchildPartTypes.Left,
			shield_intensity = 0,
			angle_range = Schild.left_range
		},
		new SchildPart{
			part = SchildPartTypes.Right,
			shield_intensity = 0,
			angle_range = Schild.right_range
		},
		new SchildPart{
			part = SchildPartTypes.Top,
			shield_intensity = 0,
			angle_range = Schild.top_range
		},
		new SchildPart{
			part = SchildPartTypes.Bot,
			shield_intensity = 0,
			angle_range = Schild.bot_range
		}
	};

	public float max_shield_power;


	public GameObject gameObject;

	public static string getPartName(SchildPartTypes part){
		switch (part) {
		case SchildPartTypes.Bot:
			return "Hintere";
		case SchildPartTypes.Left:
			return "Backbord";
		case SchildPartTypes.Right:
			return "Steuerbord";
		case SchildPartTypes.Top:
			return "Vordere";
		}
		return "";
	}
	public Schild(){
		max_shield_power = 100;
		setup_shield_parts ();
	}
	public Schild(float pmax_shield_power, GameObject go){
		max_shield_power = pmax_shield_power;
		setup_shield_parts ();
		gameObject = go;
	}

	public SchildPart get_best_shield_part(){
		SchildPart best_part = null;
		foreach (SchildPart p in shield_parts) {
			if (best_part == null || p.shield_intensity > best_part.shield_intensity)
				best_part = p;
		}
		return best_part;
	}

	public void setup_shield_parts(){
		float q = max_shield_power /4;
		foreach (SchildPart p in shield_parts) {
			p.shield_intensity = q;
		}
	}

	/*public void new_power(float pow){
		this.max_shield_power = pow;
		setup_shield_parts ();
	}*/

	public void change_max_power(float pow){
		float n_q = pow / 4;
		foreach (SchildPart p in shield_parts) {
			p.shield_intensity = n_q * (p.shield_intensity / max_shield_power_per_quarter);
		}
		this.max_shield_power = pow;
	}
		

	public float max_shield_power_per_quarter{
		get { return max_shield_power / 4; }
	}

	public SchildPart get_shield_part(SchildPartTypes part){
		foreach (SchildPart p in shield_parts) {
			if (p.part == part)
				return p;
		}
		return null;
	}

	public float get_shield_intensity_at_part(SchildPartTypes schildpart){
		SchildPart p = get_shield_part (schildpart);
		return p == null ? -1 : p.shield_intensity;
	}
	public SchildPartTypes get_schildpart_at_angle(float angle){
		//angle += angle < 0 ? (angle<-45 ? 360 : 0) : 0;
		angle += angle<-45 ? 360 : 0;
		if (Schild.top_range.contains_number (angle)) {
			return SchildPartTypes.Top;
		}
		if (Schild.right_range.contains_number (angle)) {
			return SchildPartTypes.Right;
		}
		if (Schild.bot_range.contains_number (angle)) {
			return SchildPartTypes.Bot;
		}
		if (Schild.left_range.contains_number (angle)) {
			return SchildPartTypes.Left;
		}
		return SchildPartTypes.None;
	}

	public void apply_dmg_at_shield_part(SchildPartTypes schildpart, float dmg){
		SchildPart p = get_shield_part (schildpart);
		if (p != null) {
			p.shield_intensity -= dmg;
			p.shield_intensity = p.shield_intensity < 0 ? 0 : p.shield_intensity;
		}
	}

	public void regenerate_shield(float regen_amount){
		foreach (SchildPart p in shield_parts) {
			p.shield_intensity += regen_amount;
			p.shield_intensity = Mathf.Min (p.shield_intensity, max_shield_power_per_quarter);
		}
	}
	public void transfer_shield_energy(SchildPartTypes p1, SchildPartTypes p2, float amt){ //TODO
		throw new System.NotImplementedException ();
	}
}
