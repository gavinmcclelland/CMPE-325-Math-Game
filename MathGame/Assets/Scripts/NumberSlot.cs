using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NumberSlot : MonoBehaviour {

    // ============================== INSPECTOR VARIABLES ==============================

    // The colours that the number slot should be when idle and close to a player
    public Color normalColor = Color.grey;
    public Color activeColor = Color.green;

    // ============================== INTERNAL VARIABLES ==============================

    // The visible text (underline) of the number slot
    TextMesh testMesh;
    // The associated number placed in this number slot
    [HideInInspector]
    public Number number;
    // The number slot controller this number slot is part of
    [HideInInspector]
    public NumberSlotController numberSlotController;

    // State flags
    bool isSelected = false;
    [HideInInspector]
    public bool hasNumber = false;

    // The number that is currently in the process of being moved to this number slot
    [HideInInspector]
    public Number reservedNumber;

    // ============================== START ==============================

    void Start() {
        // Get the text from within the child text object
        // The number slot itself is an empty object for positioning
        testMesh = GetComponentInChildren<TextMesh>();
    }
    
    // ============================== UPDATE ==============================

    void Update() {
        
        // Debug draw (can be removed)
        if (hasNumber) {
            Debug.DrawLine(transform.position, number.transform.position, Color.black);
        }
        Debug.DrawRay(transform.position, new Vector3(1, 0, -1), (reservedNumber != null) ? Color.green : Color.red);
        Debug.DrawRay(transform.position, new Vector3(0, 0, -1), hasNumber ? Color.green : Color.red);

    }

    // ============================== FILL WITH NUMBER ==============================

    public void setIsFilled(Number numberToSet) {

        // Assign a number to this number slot
        number = numberToSet;
        hasNumber = (number != null);

        // If thiere is now a number in the number slot,
        // there is no more number in-process of being moved
        if (hasNumber) {
            reservedNumber = null;
        }

        // If there is now a number in this number slot,
        // unselect the number slot since the number is always selected
        // instead of the number slot if conditions are met
        if (hasNumber) {
            setDeselected();
        }

        // If the number contained in this number slot changed,
        // Tell the number slot controller this number slot is part of to recalculate its value
        if (numberSlotController != null) {
            numberSlotController.calculateValue();
        }

    }

    // ============================== SELECT / DESELECT ==============================

    public void setSelected(bool byLeftPlayer) {

        // Make sure to only do this if not already selected
        if (!isSelected) {

            // If there is no number in this number slot,
            // set the underline text to look highlighted
            if (hasNumber) {
                testMesh.fontStyle = FontStyle.Bold;
                testMesh.color = activeColor;
            }
            // Otherwise set the number to be selected
            else {
                number.setSelected(byLeftPlayer);
            }
            isSelected = true;
        }

    }

    public void setDeselected() {

        // Make sure to only do this if still selected
        if (isSelected) {

            // Set the text to look normal
            testMesh.fontStyle = FontStyle.Normal;
            testMesh.color = normalColor;

            // Deselect the number as well if need be
            if (hasNumber) {
                number.setDeselected();
            }
            isSelected = false;
        }

    }

}