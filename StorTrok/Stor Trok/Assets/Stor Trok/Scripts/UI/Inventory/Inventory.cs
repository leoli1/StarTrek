using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Inventory : MonoBehaviour {

	public static Inventory inventory;

	public int rows = 6;
	public int columns = 4;
	int offset = 4;
	int offset_zum_rand = 5;

	public GameObject item_button;

	public List<GameObject> item_buttons;

	public Inventory(){
		Inventory.inventory = this;
	}

	void Start () {
		item_buttons = new List<GameObject> ();
		Image img = item_button.GetComponent<Image> ();

		RectTransform rt = this.GetComponent<RectTransform> ();
		rt.sizeDelta = new Vector2(rows * (img.rectTransform.rect.width + offset)+offset_zum_rand,columns*(img.rectTransform.rect.height + offset)+offset_zum_rand);
		rt.position = new Vector3 (Screen.width * 0.7f, Screen.height*0.5f, 0);

		for (int y = columns-1; y >= 0; y--) {
			for (int x = 0; x < rows; x++) {
				GameObject button = GameObject.Instantiate (item_button);
				button.transform.SetParent(transform);

				Image b_img = button.GetComponent<Image> ();
				b_img.rectTransform.localPosition = new Vector3 (x * (img.rectTransform.rect.width + offset)+offset_zum_rand, y * (img.rectTransform.rect.height + offset)+offset_zum_rand,0);

				item_buttons.Add (button);
			}
		}
		set_buttons ();
	}

	public void set_buttons(){
		Item[] items = Player.player.player_inventar.items;
		for (int i = 0; i < items.Length; i++) {
			ItemButton ib = item_buttons [i].GetComponent<ItemButton> ();//item_buttons [i].GetComponentInChildren<ItemButton> ();
			ib.item_button_position_type = ItemButtonPositionType.Inventar;
			ib.index = i;
			//if (Utils.item_is_null(items[i])) {continue;}
			ib.set_item(items[i]);
		}
	}

	void Update () {
	}
}
