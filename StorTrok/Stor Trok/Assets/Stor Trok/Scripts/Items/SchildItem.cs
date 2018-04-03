using System.Collections;
using System.Collections.Generic;
using UnityEngine;



[CreateAssetMenu(menuName = "Game-Items/Schild")]
public class SchildItem : Item {//ID = 1000

	[Tooltip("Maximale Schildintensität")]
	public float max_shield_intenstiy;

	[Tooltip("Schildintensität, die das Schild pro Sekunde regeneriert")]
	public float shield_regeneration_rate; //TODO

	public SchildItem(){
		item_type = ItemType.Schild;
	}

	public override string get_description_text ()
	{
		string s = this.name+"\nSchildintensität: "+this.max_shield_intenstiy.ToString();
		return s;
	}
}