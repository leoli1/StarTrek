using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum AlleRaumschiffe{
	None,
	EnterpriseM1,
	EnterpriseE,
	Defiant,
	DeltaFlyer,
	KlingWarbirdM1,
	BorgCube
}
public enum RawRaumschiffType{
	None = 1,
	BigShip = 2,
	Shuttle = 4,
	All = BigShip|Shuttle
}

[CreateAssetMenu(menuName = "Game-Items/Raumschiff")]
public class Raumschiff : Item { //0000
	
	public static List<Raumschiff> alle_raumschiffe = new List<Raumschiff> ();

	public static Raumschiff get_raumschiff_at_position_in_list(int index){
		Raumschiff r = alle_raumschiffe [index];
		return (Raumschiff)r.MemberwiseClone ();
	}
	public static Raumschiff get_raumschiff_by_type(AlleRaumschiffe type){
		foreach (Raumschiff r in alle_raumschiffe) {
			if (r.raumschiff_type == type) {
				return r;
			}
		}
		return null;
	}
	public static Raumschiff get_raumschiff_by_id(int id){
		foreach (Raumschiff r in alle_raumschiffe) {
			if (r.ID == id) {
				return r;
			}
		}
		return null;
	}


	public GameObject spaceship_object;

	public AlleRaumschiffe raumschiff_type;
	public RawRaumschiffType raw_raumschiff_type;

	public float rotation_speed;
	public float acceleration;

	public float structural_integrity;
	public float structural_integrity_regeneration_rate;

	public int spaceship_module_capacity;

	[Header("Standard Items")]

	public SchildItem standard_schild;
	public ImpulsAntrieb standard_impuls_antrieb;
	public WarpAntrieb standard_warp_antrieb;

	public List<Weapon> standard_top_weapons;
	public List<Weapon> standard_bot_weapons;

	public List<SpaceshipModule> standard_spaceship_modules;
		
	public Raumschiff(){
		item_type = ItemType.Raumschiff;
	}

	public override string get_description_text ()
	{
		return "";
	}

}
