using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Inventar {
	public Item[] items;
	public int capacity = 16; // maximale anzahl an items im inventar

	public Inventar(){
		capacity = Inventory.inventory.rows*Inventory.inventory.columns;
		items = new Item[capacity];
	}

	public bool add_item(Item item){
		int c = 0;
		foreach (Item i in items) {
			if (i == null || i.ID == 0) {
				items [c] = item;
				LevelManager.save_player_data ();
				Inventory.inventory.set_buttons ();
				return true;
			}
			c++;
		}
		StatusTexts.status_texts.new_text ("Inventar ist voll");
		return false;
	}
	public bool add_item(Item item, int index){
		if (items [index] == null) {
			items [index] = item;
			return true;
		} else {
			return false;
		}
	}
}
