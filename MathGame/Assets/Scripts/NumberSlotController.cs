using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using UnityEngine;

public class NumberSlotController : MonoBehaviour {

    // ============================== INSPECTOR VARIABLES ==============================

    // The side for these number slots
    public bool isLeftSide = true;

    // If the value should be shown using debug text output (should always be false)
    public bool showDebugValue = false;

    // ============================== INTERNAL VARIABLES ==============================

    // The number slot scripts (on number slot objects) assigned to this side
    // This stores all the number slots but some may be disabled depending on how many characters the current equation has
    public List<NumberSlot> allNumberSlots;

    // The currently active number slot scripts (on number slot objects) assigned to this side
    // This depends on how many characters the current equation has
    public List<NumberSlot> numberSlots;

    // The trigger box collider on the game object surrounding all the number slots
    [HideInInspector]
    public Collider border;

    // The equation part stored in the contained number slots, its value
    // and if it's currently valid (invalid when equation is not complete)
    [HideInInspector]
    string currentEquation = "";
    [HideInInspector]
    public double currentValue = 0;
    [HideInInspector]
    public bool currentValueIsValid = false;

    // The indicies of the number slot selected by each player
    [HideInInspector]
    public int currentSelectedSlotIndexByLeftPlayer = -1;
    [HideInInspector]
    public int currentSelectedSlotIndexByRightPlayer = -1;

    // ============================== START ==============================

    void Start() {

        // Set the reference of the corresponding number slot controller in the main controller to this script
        // This is done so all other scripts can easily access the number slot controller scipts
        if (isLeftSide) {
            MainController.leftNumberSlotController = this;
        }
        else {
            MainController.rightNumberSlotController = this;
        }

        // Get the collider component
        border = GetComponent<Collider>();

        // Get all the number slots
        allNumberSlots = new List<NumberSlot>(GetComponentsInChildren<NumberSlot>());

        // Assign the reference of the number slot controller to this script for each contained number slot
        foreach (NumberSlot numberSlot in allNumberSlots) {
            numberSlot.numberSlotController = this;
        }

    }

    // ============================== NUMBER SLOTS ==============================

    public void setNumberOfActiveNumberSlots(int numberOfActiveNumberSlots) {

        // Disable/Enable the needed number of number slots
        for (int i = 0; i < allNumberSlots.Count; i++) {

            // Left side
            if (isLeftSide) {
                // Number slot is active if its index is more or equal to the needed number of number slots
                allNumberSlots[i].gameObject.SetActive(i >= (allNumberSlots.Count - numberOfActiveNumberSlots));
            }
            // Right side 
            else {
                // Number slot is active if its index is less than the needed number of number slots
                allNumberSlots[i].gameObject.SetActive(i < numberOfActiveNumberSlots);
            }
        }

        // Finally, store the active number slots into the list
        numberSlots = new List<NumberSlot>(GetComponentsInChildren<NumberSlot>());

    }

    // ============================== (RE)CALCULATE VALUE ==============================

    public void calculateValue() {

        // Construct equation string using each existing number
        currentEquation = "";
        foreach (NumberSlot numberSlot in numberSlots) {
            currentEquation += numberSlot.number != null ? numberSlot.number.getString(true) : " ";
        }

        // Attempt to calculate value of equation
        try {
            currentValue = Convert.ToDouble(new DataTable().Compute(currentEquation, null));
            currentValueIsValid = true;
        }
        // If equation is invalid, clear the valid flag
        catch {
            currentValueIsValid = false;
        }

    }

    void Update() {
        // Debug: draw the value on screen
        if (showDebugValue) {
            HelperFunctions.DebugText(transform.position + new Vector3(0, 1, 2.75f), currentValueIsValid ? ("" + currentValue) : "N/A", Color.magenta);
        }
    }

    // ============================== SELECT / DESELECT ==============================

    public void setSelectedNumberSlot(bool byLeftPlayer, int numberSlotIndex) {

        // Make sure to only do this if not already selected
        bool canSetSelected = (byLeftPlayer && numberSlotIndex != currentSelectedSlotIndexByLeftPlayer) || (!byLeftPlayer && numberSlotIndex != currentSelectedSlotIndexByRightPlayer);
        if (canSetSelected) {

            // For the left player
            if (byLeftPlayer) {
                // Deselect the old number slot if need be
                deselectNumberSlot(true);
                // Select the new number slot
                currentSelectedSlotIndexByLeftPlayer = numberSlotIndex;
                numberSlots[numberSlotIndex].setSelected(byLeftPlayer);
            }

            // Same thing for player 2
            // TODO: maybe refactor so this code is not duplicated twice (not sure if woth the trade-off)
            else {
                deselectNumberSlot(false);
                currentSelectedSlotIndexByRightPlayer = numberSlotIndex;
                numberSlots[numberSlotIndex].setSelected(byLeftPlayer);
            }
        }
    }

    public void deselectNumberSlot(bool byLeftPlayer) {

        // Make sure to only deselect if selected by this player only
        bool canDeselectNumberSlot = currentSelectedSlotIndexByLeftPlayer != currentSelectedSlotIndexByRightPlayer;

        if (byLeftPlayer) {

            // Make sure to only reset selection this if still selected
            if (currentSelectedSlotIndexByLeftPlayer != -1) {
                if (canDeselectNumberSlot) {
                    numberSlots[currentSelectedSlotIndexByLeftPlayer].setDeselected();
                }
                currentSelectedSlotIndexByLeftPlayer = -1;
            }
        }

        // Same thing for player 2
        // TODO: maybe refactor so this code is not duplicated twice (not sure if woth the trade-off)
        else {
            if (currentSelectedSlotIndexByRightPlayer != -1) {
                if (canDeselectNumberSlot) {
                    numberSlots[currentSelectedSlotIndexByRightPlayer].setDeselected();
                }
                currentSelectedSlotIndexByRightPlayer = -1;
            }
        }

    }

}