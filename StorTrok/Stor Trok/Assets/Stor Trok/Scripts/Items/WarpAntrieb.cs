using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Game-Items/WarpAntrieb")]
public class WarpAntrieb : Item { //TODO warpantrieb im raumschiff inventar anzeigen

	public float max_warpfactor;

	public WarpAntrieb(){
		this.item_type = ItemType.WarpAntrieb;
	}

	public float max_speed{
		get{ 
			float s = (float)(0.995f / Mathf.Sqrt (1 - max_warpfactor * max_warpfactor / 1000) * Mathf.Pow (max_warpfactor, 2.85f));
			return s;
		}
	}

	public override string get_description_text ()
	{
		string s = this.name+"\nMax. Warpfaktor "+this.max_warpfactor;
		return s;
	}
}
