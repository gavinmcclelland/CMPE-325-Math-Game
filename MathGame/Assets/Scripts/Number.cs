using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Number : MonoBehaviour {

    // ============================== INSPECTOR VARIABLES ==============================

    // Parameters of a number (actually a number or operator)
    public bool canBePickedUp = true;
    public int numericValue = 1;
    public bool isNumber = true;

    // Types of operators supported
    public enum Symbol {
        PLUS = 0,
        MINUS = 1,
        MULTIPLY = 2,
        DIVIDE = 3
    }
    public Symbol symbolicValue = Symbol.PLUS;

    // The colours that the number should be when idle, close to a player, and fixed
    public Color normalColor = Color.black;
    public Color activeColor = Color.green;
    public Color fixedColor = Color.yellow;

    // ============================== INTERNAL VARIABLES ==============================

    // The number movement script on this number object
    [HideInInspector]
    public NumberMovement numberMovement;

    // The visible text of the number
    TextMesh testMesh;
    // The associated number slot this number is placed in
    [HideInInspector]
    public NumberSlot numberSlot = null;

    // For storing data about numbers
    // This is required since in Unity the Number class (subclass of MonoBehaviour) 
    // cannot be created without creating a Game Object
    public class NumberData {
        public int numericValue;
        public bool isNumber;
        public Number.Symbol symbolicValue;
        public bool canBePickedUp;
        public NumberData(int _numericValue, bool _isNumber, Symbol _symbolicValue, bool _canBePickedUp = true) {
            numericValue = _numericValue;
            isNumber = _isNumber;
            symbolicValue = _symbolicValue;
            canBePickedUp = _canBePickedUp;
        }
    }

    // Flags to control behaviour
    [HideInInspector]
    public bool isSelectedByLeftPlayer = false;
    [HideInInspector]
    public bool isSelectedByRightPlayer = false;

    // State flags
    bool didInitialize = false;
    bool isSelected = false;

    // The string representation of the possible values a number or operator can have
    static string[] numericNames = { "Zero", "One", "Two", "Three", "Four", "Five", "Six", "Seven", "Eight", "Nine" };
    static string[] symbolicNames = { "Plus", "Minus", "Multiply", "Divide" };
    static string[] symbolicSymbols = { "+", "-", "×", "÷" };
    static string[] evaluationSymbols = { "+", "-", "*", "/" };

    // ============================== START ==============================

    void initialize() {

        // Make sure to only do this once
        if (!didInitialize) {

            // Set the value of the text to what it should be
            testMesh = GetComponent<TextMesh>();
            testMesh.text = getString();

            // Get the number movement script and set reference of number script to this script
            numberMovement = GetComponent<NumberMovement>();
            numberMovement.numberScript = this;

            // Set the text color to fixed if not selectable
            if (!canBePickedUp) {
                testMesh.color = fixedColor;
            }

            // If already parented to a number slot, assign this number to it
            if (transform.parent != null && transform.parent.gameObject.tag == "Number Slot") {
                numberSlot = transform.parent.gameObject.GetComponent<NumberSlot>();
            }
            if (numberSlot != null) {
                transform.SetParent(numberSlot.transform);
                numberSlot.setIsFilled(this);
            }

            // Add this number to the global list of numbers to be selectable by the player
            MainController.numbers.Add(this);
            didInitialize = true;
        }

    }

    void Start() {

        // Initialize if need be
        initialize();

        // Start out small and scale up to normal size
        transform.localScale = Vector3.zero;
        numberMovement.setScaleTarget(1.0f);

        // Fix movement if not selectable
        if (!canBePickedUp) {
            numberMovement.setKinematic();
        }

    }

    // ============================== UPDATE ==============================

    void Update() {

        // Highlighting based on player proximity if not scaling down before removal
        if (numberMovement.scaleTarget != 0) {

            if (MainController.leftPlayer != null) {
                float distanceToPlayer = Vector3.Distance(transform.position, MainController.leftPlayer.transform.position);

                // If within selecting distance indicate that this number is potentially the closest
                if (distanceToPlayer <= MainController.leftPlayer.playerNumberInteraction.itemPickupRange) {
                    // Debug.DrawLine(transform.position, MainController.leftPlayer.transform.position, Color.green, 0);
                    MainController.leftPlayer.playerNumberInteraction.setCloseNumber(this, distanceToPlayer);
                }
                // Otherwise indicate that this number is not within range of being selected
                else {
                    // Debug.DrawLine(transform.position, MainController.leftPlayer.transform.position, Color.red, 0);
                    MainController.leftPlayer.playerNumberInteraction.setNonCloseNumber(this);
                }
            }

            // Same thing for player 2
            // TODO: maybe refactor so this code is not duplicated twice (not sure if woth the trade-off)
            if (MainController.rightPlayer != null) {
                float distanceToPlayer = Vector3.Distance(transform.position, MainController.rightPlayer.transform.position);
                if (distanceToPlayer <= MainController.leftPlayer.playerNumberInteraction.itemPickupRange) {
                    MainController.rightPlayer.playerNumberInteraction.setCloseNumber(this, distanceToPlayer);
                }
                else {
                    MainController.rightPlayer.playerNumberInteraction.setNonCloseNumber(this);
                }
            }

        }

        // Otherwise, this number is being picked and is the currently selected number
        else {
            setSelected(true);
        }

    }

    // ============================== SET FIXED ==============================

    public void setFixed() {

        // Set the text to fixed color and not bold
        testMesh.color = fixedColor;
        testMesh.fontStyle = FontStyle.Normal;
        canBePickedUp = false;

    }

    // ============================== SELECT / DESELECT ==============================

    public void setSelected(bool byLeftPlayer) {

        // Make sure to only do this if not already selected
        if (!isSelected) {
            initialize();

            // Keep track of which player has this number selected
            isSelectedByLeftPlayer = byLeftPlayer;
            isSelectedByRightPlayer = !byLeftPlayer;

            // Set the text to look highlighted if this number can be picked up
            if (canBePickedUp) {
                testMesh.fontStyle = FontStyle.Bold;
                testMesh.color = activeColor;
            }
            isSelected = true;
        }

    }

    public void setDeselected() {

        // Make sure to only do this if still selected
        if (isSelected) {
            initialize();

            // Now no player has this number selected
            isSelectedByLeftPlayer = false;
            isSelectedByRightPlayer = false;

            // Set the text to look normal if need be
            if (canBePickedUp) {
                testMesh.fontStyle = FontStyle.Normal;
                testMesh.color = normalColor;
            }
            isSelected = false;
        }

    }

    // ============================== PICKUP / PLACE DOWN ==============================

    public void assignToNumberSlot(NumberSlot numberSlotToSet) {

        // Initialize if need be
        initialize();

        // Clear the previous number slot if need be
        if (numberSlot != null) {
            numberSlot.setIsFilled(null);
        }

        // Set this number as the one belonging to the number slot it is in
        numberSlot = numberSlotToSet;
        numberSlot.setIsFilled(this);

        // Indicate that the number now needs to move to the center position of the number slot
        numberMovement.setMovementTarget(numberSlot.transform);

    }

    public void unAssignFromNumberSlot() {

        // Initialize if need be
        initialize();

        // Clear the number slot and make it not contain any number
        if (numberSlot != null) {
            numberSlot.setIsFilled(null);
            numberSlot = null;
            // Deselect the number as well since it is being moved away from the player
            setDeselected();
        }

    }

    public void unregister() {

        // Update number slot to contain no number if need be
        if (numberSlot != null) {
            numberSlot.setIsFilled(null);
        }

        // Remove this number from the global list
        MainController.numbers.Remove(this);

    }

    public void getPickedUp(bool byLeftPlayer) {

        // Remove logic from this number
        unregister();

        // If there is a left player
        if (MainController.leftPlayer != null) {

            // Indicate that this number is no longer the closest to the player
            MainController.leftPlayer.playerNumberInteraction.setNonCloseNumber(this);

            // Set target to move towards player
            if (byLeftPlayer) {
                numberMovement.setMovementTarget(MainController.leftPlayer.transform);
            }

        }

        // Same thing for player 2
        // TODO: maybe refactor so this code is not duplicated twice (not sure if woth the trade-off)
        if (MainController.rightPlayer != null) {
            MainController.rightPlayer.playerNumberInteraction.setNonCloseNumber(this);
            if (!byLeftPlayer) {
                numberMovement.setMovementTarget(MainController.rightPlayer.transform);
            }
        }

        // Set target to shrink down
        numberMovement.setScaleTarget(0);

    }

    // ============================== INSTANTIATE NUMBER ==============================

    public static Number InstantiateNumber(NumberData numberData, NumberSlot parentNumberSlot, GameObject numberPrefab, Vector3 position, Quaternion rotation) {

        // Create the number game object and set its attributes to the required ones based on number data
        GameObject numberObject = Instantiate(numberPrefab, position, rotation);
        Number number = numberObject.GetComponent<Number>();
        number.numericValue = numberData.numericValue;
        number.isNumber = numberData.isNumber;
        number.symbolicValue = numberData.symbolicValue;
        number.canBePickedUp = numberData.canBePickedUp;

        // Assign to number slot if required
        if (parentNumberSlot != null) {
            number.transform.SetParent(parentNumberSlot.transform);
        }

        // Set the name of the number game object to reflect what it is
        numberObject.name = (number.isNumber ? "Number " : "Symbol ") + number.getNameString();

        return number;

    }

    // ============================== CHAR TO NUMBER DATA ==============================

    public static NumberData getNumberDataFromChar(char token) {

        // Check if this token is a symbol
        int symbolIndex = Array.IndexOf(evaluationSymbols, token.ToString());
        if (symbolIndex != -1) {
            // Return the number data for this symbol
            return new NumberData(0, false, (Symbol) symbolIndex);
        }

        // Otherwise, this token should be a number
        else {

            // Try to convert to numeric value
            int numericValue = 0;
            if (Int32.TryParse(token.ToString(), out numericValue)) {
                // Return the number data for this number
                return new NumberData(numericValue, true, Symbol.PLUS);
            }

            // If the convertion failed, this is an invalid token
            else {
                return null;
            }

        }

    }

    // ============================== TO STRING ==============================

    public string getString() {
        // By default, return the human-readable string representaion
        return getString(false);
    }

    public string getString(bool forEvaluation) {

        // If number, return number string representation
        if (isNumber) {
            return numericValue.ToString();
        }

        // Otherwise return symbol string representation
        // If for evaluation of value, use * and / for multiply and divide
        switch (symbolicValue) {
            case Symbol.PLUS:
                return forEvaluation ? evaluationSymbols[0] : symbolicSymbols[0];
            case Symbol.MINUS:
                return forEvaluation ? evaluationSymbols[1] : symbolicSymbols[1];
            case Symbol.MULTIPLY:
                return forEvaluation ? evaluationSymbols[2] : symbolicSymbols[2];
            case Symbol.DIVIDE:
                return forEvaluation ? evaluationSymbols[3] : symbolicSymbols[3];
        }

        // Should never get here
        return "?";
    }

    public string getNameString() {

        // If number, return name of number
        if (isNumber) {
            return numericNames[numericValue];
        }

        // Otherwise return name of symbol
        switch (symbolicValue) {
            case Symbol.PLUS:
                return symbolicNames[0];
            case Symbol.MINUS:
                return symbolicNames[1];
            case Symbol.MULTIPLY:
                return symbolicNames[2];
            case Symbol.DIVIDE:
                return symbolicNames[3];
        }

        // Should never get here
        return "?";
    }

}