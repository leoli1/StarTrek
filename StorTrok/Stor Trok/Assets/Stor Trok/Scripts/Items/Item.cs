using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public enum ItemType{
	Schild,
	ImpulsAntrieb,
	WarpAntrieb,
	Weapon,
	Raumschiff,
	SpaceshipModule
}

public enum AntriebType{
	Impuls = 0,
	Warp = 1
}
		
public abstract class Item : ScriptableObject{ // superclass für alle items (waffen, antrieb, schild...)

	public static List<Item> AllItems = new List<Item> ();// = Item.create_items();//SchildItem.allSchildItems.ConvertAll(x=>(Item)x) + ImpulsAntrieb.impuls_antriebe.ConvertAll(x=>(Item)x) + Weapon.AllWeapons.ConvertAll(x=>(Item)x);


	public static Item get_item_at_position_in_list(int index){
		Item o = AllItems [index];
		return (Item)o.MemberwiseClone ();
	}

	/// <summary>
	/// Findet ein Item per ID und gibt eine Kopie dessen zurück.
	/// </summary>
	/// <returns>Kopie des gefundenen Items oder null, wenn kein Item mit dieser ID existier.</returns>
	/// <param name="id">Die ID des zu suchenden Items.</param>
	public static Item get_item_by_id(int id){
		int index = AllItems.FindIndex (item => item.ID == id);

		return index == -1 ? null : get_item_at_position_in_list (index);
	}

	/// <summary>
	/// Lädt die Item-Daten aus dem Resource-Folder
	/// </summary>
	public static void load_items(){
		AllItems = new List<Item>();
		List<Object> items = Utils.GetScriptAssetsOfType<Item>();
		foreach (Object o in items) {
			AllItems.Add ((Item)o);
		}


		Raumschiff.alle_raumschiffe = new List<Raumschiff> ();
		List<Object> raumschiffe = Utils.GetScriptAssetsOfType<Raumschiff> ();

		foreach (Object o in raumschiffe) {
			Raumschiff.alle_raumschiffe.Add ((Raumschiff)o);
		}
	}

	/// <summary>
	/// Wählt ein zufälliges Item aus
	/// </summary>
	public static Item random(){
		int l = AllItems.Count;
		int i = Random.Range (0, l);
		return AllItems[i];
	}

	public Item(){
		AllItems.Add (this);
	}

	/// <summary>
	/// Unique ID für das Item
	/// </summary>
	public int ID;

	/// <summary>
	/// Das Sprite, das im Inventar für das Item angezeigt wird
	/// </summary>
	public Sprite inventory_tex;

	/// <summary>
	/// Name des Items
	/// </summary>
	public new string name;

	/// <summary>
	/// Nummer des Items, z.B. Phaser 1 oder Torpedo 6
	/// </summary>
	public int mark_number = 0;

	/// <summary>
	/// Preis des Items
	/// </summary>
	public int cost; // TODO

	/// <summary>
	/// Typ des Items: Schild, Waffe, etc.
	/// </summary>
	public ItemType item_type;

	/// <summary>
	/// Erstellt einen Text, der im Inventar angezeigt wird, wenn der Mauszeiger auf dem Item ist
	/// </summary>
	/// <returns>The description text.</returns>
	public abstract string get_description_text ();

}
