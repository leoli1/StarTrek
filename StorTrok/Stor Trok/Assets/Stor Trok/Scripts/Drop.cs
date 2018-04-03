using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Drop : MonoBehaviour {

	private Item _item;
	public Item item{
		get{ 
			return _item;
		}
		set{ 
			//print ("New Item for drop " + name + ": " + value.name);
			_item = value;
		}
	}
	const float lifetime = 120;

	public static void drop_item(Vector3 spawn_loc){
		drop_item (spawn_loc, Item.random ());
	}
	public static void drop_item(Vector3 spawn_loc, Item item){
		GameObject drop = GameObject.Instantiate (OtherPrefabObjects.otherPrefabObjects.drop_object, spawn_loc, Quaternion.identity);
		Drop d = drop.GetComponent<Drop> ();
		d.item = item;
	}

	void Start () {
		Destroy (gameObject, lifetime);
		float rad = GetComponent<SphereCollider> ().radius * transform.localScale.x+3;
		if (Vector3.Distance (PlayerScript.playerScript.transform.position, transform.position) < rad) {
			//GetComponent<ObjectAura> ().OnTriggerEnter (PlayerScript.playerScript.gameObject.GetComponentInChildren<Collider> ());
		}
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
