using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Player))]
public class PlayerInventory : MonoBehaviour {

    // ============================== INSPECTOR VARIABLES ==============================

	public Text numberSlot;
	public Text numberNameSlot;

    // ============================== INTERNAL VARIABLES ==============================

	// The player script on the current player object
	[HideInInspector]
	Player playerScript;

	// The actual inventory data
	Number.NumberData numberData = null;
	[HideInInspector]
	public bool hasItem = false;

    // ============================== START ==============================

	void Start() {
		
		// Get the player script and setup the reference to this script so other scripts can access this one
		playerScript = GetComponent<Player>();
		//playerScript.playerInventory = this;

		// Set number slot text to blank at start
		numberSlot.text = "";
		numberNameSlot.text = "None";

	}

    // ============================== CLEAR INVENTORY ==============================

	public void clearInventory() {
		// Reset the text to no selection
		hasItem = false;
		numberSlot.text = "";
		numberNameSlot.text = "None";
	}

    // ============================== GET / SET NUMBER ==============================

	public Number.NumberData getNumber() {

		// Nothing to return of no number stored in inventory
		if (!hasItem) {
			return null;
		}

		// Otherwise clear the inventory and return selected number
		clearInventory();
		return numberData;
	}

	// Returns true if number was picked up
	public bool addNumber(Number number) {

		// No space in inventory
		if (hasItem) {
			// TODO: Error message
			Debug.Log("Inventory is full!");

			// Return fasle to indicate a number could not be stored
			return false;
		}

		// Strore in inventory by copying current number attributes to NumberData inner class
		hasItem = true;
		numberSlot.text = number.getString();
		numberNameSlot.text = number.getNameString();
		numberData = new Number.NumberData(number.numericValue, number.isNumber, number.symbolicValue);

		// Return true to indicate a number was stored
		return true;

	}

}