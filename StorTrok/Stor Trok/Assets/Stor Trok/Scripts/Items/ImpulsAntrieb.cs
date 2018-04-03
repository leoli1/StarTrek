using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(menuName = "Game-Items/ImpulsAntrieb")]
public class ImpulsAntrieb : Item { //ID = 5000


	public float max_speed;


	public ImpulsAntrieb(){
		this.item_type = ItemType.ImpulsAntrieb;
	}

	public override string get_description_text ()
	{
		string s = this.name+"\nGeschwindigkeit: "+this.max_speed.ToString();
		return s;
	}
}
