using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;



public class UIWeaponImage : MonoBehaviour {

	public static List<UIWeaponImage> allUIWeaponImages;
	public Weapon weapon;
	//public int index;
	public PlayerWeaponPositions pos;
	public GameObject cooldown_obj;

	public GameObject weapon_range_prefab;
	private GameObject weapon_range_object;
	void Start () {
		allUIWeaponImages.Add (this);
		cooldown_obj.GetComponent<RectTransform> ().sizeDelta = this.GetComponent<RectTransform> ().sizeDelta;
		set_image ();
	}
	void OnDestroy(){
		allUIWeaponImages.Remove (this);
	}

	public void set_image(){
		//print (weapon.name);
		this.GetComponent<Image> ().sprite = weapon==null ? ItemButton.standard_tex : weapon.inventory_tex;
	}

	void Update () {
		if (weapon != null) {
			float t;
			if (weapon.GetType ().Name != "PulsPhaser") {
				t = Mathf.Clamp (1 - ((Time.time - weapon.last_time_shot) / weapon.reload_time), 0, 1);
			} else {
				t = 0;
				if (((PulsPhaser)weapon).missiles_shot == ((PulsPhaser)weapon).salve_size) {
					t = Mathf.Clamp (1 - (Time.time - weapon.last_time_shot) / ((PulsPhaser)weapon).salve_reload_time, 0, 1);
				}
			}
			cooldown_obj.transform.localScale = new Vector3 (1, t, 1);
		} else {
			cooldown_obj.transform.localScale = new Vector3 (1, 0, 1);
		}
	}
	public void pointer_enter(){
		/*weapon_range_object = GameObject.Instantiate (weapon_range_prefab, weapon.weapon_position.transform);
		float rat = Mathf.Tan (weapon.arc_range*Mathf.Deg2Rad);
		print (rat);
		float scale = 100;
		weapon_range_object.transform.localScale = new Vector3 (scale * rat, scale * rat, scale);
		weapon_range_object.transform.localPosition = Vector3.zero;
		Quaternion rotation = Quaternion.LookRotation (weapon.weapon_position.GetComponent<SpaceshipWeaponPosition> ().local_direction);
		rotation.eulerAngles = new Vector3 (rotation.eulerAngles.x, rotation.eulerAngles.y + 180, rotation.eulerAngles.z);
		weapon_range_object.transform.rotation = rotation;*/

	}
	public void pointer_exit(){
		//Destroy (weapon_range_object);
	}

	/*Weapon get_weapon(){
		switch (pos){
		case ItemButtonPositionType.BotWeapons:
			//return Player.player.get_bot_weapon_position().weap
		}
	}*/
}
