using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class EquationController : MonoBehaviour {

    // ============================== INSPECTOR VARIABLES ==============================

    // The prefab of the number object to spawn
    public GameObject numberPrefab;

    public AudioClip sound;
    private AudioSource source {get{return GetComponent<AudioSource>();}}

    // The list of equations to create, one for each stage (this must use '/' for divide and '*' for multiply)
    // Numbers and symbols in [square brackets] are placed in equation and are not moveable
    // Other numbers are placed randomly in level
    public string[] equationsList = new string[] {
        "[3]+5=2*[4]",
        "[7]+5=3*[4]",
        "[12]-5=3*[2]+1",
        "[9]+1=2*[5]",
        "[9]+5=4*[4]-2",
        "[4]*3-1=2*[5]+1",
        "[1]*5=3*[2]-1",
        "[16]+5=7+[14]",
        "[12]/2=2*[4]-2",
        "[8]+5=3*[4]+1"
    };

    // How many numbers to randomly place which are not part of the equation
    public int numberOfRandomNumbersToAdd = 3;

    // How long to pause after the current equation is solved before creating the next one (in seconds)
    public float nextStageDelay = 3.0f;

    // Set to true when a new equation needs to be generated
    public bool needToGenerateNewEquation = true;

    // For advancing levels via the editor, for debugging
    [Range(1, 10)] public int stageToSkipTo = 5;
    public bool skipToNewStageNow = false;

    // ============================== INTERNAL VARIABLES ==============================

    // Locations where to possibly place all non-fixed numbers
    GameObject[] numberLocations;

    // The current equation to generate 
    string currentEquation;

    // Set to true once the current equation is solved
    bool currentEquationIsSolved = true;

    // The left and right number slot controllers
    // In an array to allow for accessing left and right parts by index
    NumberSlotController[] numberSlotControllers = new NumberSlotController[2];

    // ============================== START ==============================

    void Start() {
        gameObject.AddComponent<AudioSource>();
        source.clip = sound;
        source.playOnAwake = false;

        // Get the location game objects by tag
        numberLocations = GameObject.FindGameObjectsWithTag("Number Location");

        // Populate number slot controllers array
        numberSlotControllers[0] = MainController.leftNumberSlotController;
        numberSlotControllers[1] = MainController.rightNumberSlotController;
        MainController.equationController = this;

    }

    // ============================== UPDATE ==============================

    void Update() {

        if(skipToNewStageNow) {
            skipToNewStageNow = false;
            if(stageToSkipTo > 0 && stageToSkipTo <= MainController.levelNumberOfStages[(int) MainController.selectedLevel]) {
                
                int numberOfSkippedLevels = (stageToSkipTo-1 - MainController.stageNumber);
                if(numberOfSkippedLevels > 0) {
                    MainController.score += MainController.levelScorePerStage[(int) MainController.selectedLevel] * numberOfSkippedLevels;
                }
                MainController.stageNumber = stageToSkipTo-1;
                playSound();
                needToGenerateNewEquation = true;

            }
        }

        // If a new equation needs to be created
        if (needToGenerateNewEquation) {
            needToGenerateNewEquation = false;

            // Clear all the current numbers and player inventory
            clearCurrentEquation();
            //clearPlayerInventories();

            // Generate an equation and populate the number slots
            currentEquation = generateNewEquation();
            populateEquation();
            currentEquationIsSolved = false;


        }

        // Check if the equation could be solved
        if (!currentEquationIsSolved && MainController.leftNumberSlotController.currentValueIsValid && MainController.rightNumberSlotController.currentValueIsValid) {

            // Check if the equation is solved
            if (MainController.leftNumberSlotController.currentValue == MainController.rightNumberSlotController.currentValue) {
                currentEquationIsSolved = true;

                // Make all number fixed to indicate the equation is now solved
                markEquationAsSolved();

                // If so, wait a bit and then create a new one
                Invoke("moveToNextStage", nextStageDelay);
            }

        }

    }

    // ============================== NEXT STAGE ==============================

    public void moveToNextStage() {

        // Increment the score and (try to) move on to the next stage
        MainController.score += MainController.levelScorePerStage[(int) MainController.selectedLevel];
        MainController.stageNumber++;
	
        // If the last stage has now been solved
        if (MainController.stageNumber >= MainController.levelNumberOfStages[(int) MainController.selectedLevel]) {

            // Load congratulations screen
            SceneManager.LoadScene("Congratulations Screen");

        }

        // Otherwise if it is not the last stage yet
        else {

            // Generate the new equation on the next frame
            needToGenerateNewEquation = true;

        }
        playSound();
    }

    // ============================== MARK EQUATION AS SOLVED ==============================

    public void markEquationAsSolved() {

        // For each number slot in each equation
        foreach (NumberSlotController numberSlotController in numberSlotControllers) {
            foreach (NumberSlot numberSlot in numberSlotController.numberSlots) {

                // If this number slot has a number, make it fixed.
                // This make it a different colour and not selectable by the player anymore
                if (numberSlot.hasNumber) {
                    numberSlot.number.setFixed();
                }

            }
        }

    }

    // ============================== PLAY NEXT STAGE SOUND ==============================
    public void playSound(){
        source.PlayOneShot(sound);
    }

    // ============================== CLEAR EQUATION & INVENTORIES ==============================
    public void clearCurrentEquation() {
        // For each number slot in each equation
        foreach (NumberSlotController numberSlotController in numberSlotControllers) {
            foreach (NumberSlot numberSlot in numberSlotController.numberSlots) {

                // If this number slot has a number, remove it
                if (numberSlot.hasNumber) {
                    Number number = numberSlot.number;
                    number.unregister();
                    Destroy(number.gameObject);
                }

            }
        }

        // Remove all numbers not in the equation
        List<Number> numbers = new List<Number>(MainController.numbers.Count);
        foreach (Number number in MainController.numbers) {
            numbers.Add(number);
        }
        foreach (Number number in numbers) {
            number.unregister();
            Destroy(number.gameObject);
        }

    }

    /*
    public void clearPlayerInventories() {

        // If the left player exits, clear its inventory
        if (MainController.leftPlayer != null) {
            MainController.leftPlayer.playerInventory.clearInventory();
        }
        // If the right player exits, clear its inventory
        if (MainController.rightPlayer != null) {
            MainController.rightPlayer.playerInventory.clearInventory();
        }

    }
    */

    // ============================== POPULATE EQUATION ==============================

    public void populateEquation() {

        // Split the equation into left and right parts
        string[] equationParts = currentEquation.Split('=');
        
        // Extract the preselected "random" numbers
        string[] equationSubParts = equationParts[1].Split('|');
        equationParts[1] = equationSubParts[0];
        string extraNumbers = equationSubParts[1];

        // List to keep track of numbers to place outside of the equation
        List<Number.NumberData> nonFixedNumberData = new List<Number.NumberData>();

        // For each token (character) in both the left and right equation parts
        for (int equationPartIndex = 0; equationPartIndex < equationParts.Length; equationPartIndex++) {

            // If the current number/symbol can be picked up by the player or not
            bool currentPositionIsFixed = false;

            // The index of the number slot to place the current number/symbol at
            int numberSlotIndex = -1;

            // Set the active number of number slots to the number of tokens in this part of the equation
            numberSlotControllers[equationPartIndex].setNumberOfActiveNumberSlots(equationParts[equationPartIndex].Replace("[", "").Replace("]", "").Length);

            for (int charIndex = 0; charIndex < equationParts[equationPartIndex].Length; charIndex++) {

                // Get current token
                char currentToken = equationParts[equationPartIndex][charIndex];

                // If the current token is a control token
                if (currentToken == '[' || currentToken == ']') {

                    // Instead, determine if the contained numbers between the [square brackets]
                    // are to be fixed (indicated by starting '[']) or not (indicated by ending ']')
                    if (currentToken == '[') {
                        currentPositionIsFixed = true;
                    }
                    else if (currentToken == ']') {
                        currentPositionIsFixed = false;
                    }

                    // Don't place a number/symbol, move to the next token
                    continue;
                }

                // Otherwise, place at the next number slot
                else {
                    numberSlotIndex++;
                }

                // Get the number data coresponding to this token
                Number.NumberData numberData = Number.getNumberDataFromChar(currentToken);

                // If this was a valid token (a number or supported symbol)
                if (numberData != null) {

                    // If the current number is fixed
                    if (currentPositionIsFixed) {

                        // Get the number slot to assign the current number to 
                        NumberSlot currentNumberSlot = numberSlotControllers[equationPartIndex].numberSlots[numberSlotIndex];

                        // Create the number game object and set its attributes to the required ones based on number data and number slot
                        numberData.canBePickedUp = false;
                        Number number = Number.InstantiateNumber(numberData, currentNumberSlot, numberPrefab, currentNumberSlot.transform.position, Quaternion.AngleAxis(90, Vector3.right));

                    }

                    // Otherwise, add it to the ones to place outside of the equation
                    else {
                        nonFixedNumberData.Add(numberData);
                    }

                }

            }
        }

        // Add random random numbers
        // for (int i = 0; i < numberOfRandomNumbersToAdd; i++) {
        //     // TODO: Add random symbols as well
        //     nonFixedNumberData.Add(new Number.NumberData(Random.Range(0, 10), true, Number.Symbol.PLUS));
        // }

        // Add extra numbers
        foreach (char extraNumber in extraNumbers) {
            Number.NumberData numberData = Number.getNumberDataFromChar(extraNumber);
            nonFixedNumberData.Add(numberData);
        }

        // Array indicating if each location has been selected to place a number at or not
        bool[] didSelectNumberLocation = new bool[numberLocations.Length];

        // Place each non-fixed number
        foreach (Number.NumberData currentNonFixedNumberData in nonFixedNumberData) {

            // Pick location to spawn number at number which has not been picked yet
            int currentNumberLocationIndex = -1;
            do {
                currentNumberLocationIndex = Random.Range(0, numberLocations.Length);
            } while (didSelectNumberLocation[currentNumberLocationIndex]);
            didSelectNumberLocation[currentNumberLocationIndex] = true;

            // Create the number game object and set its attributes to the required ones based on number data chosen and position to place at
            Number number = Number.InstantiateNumber(currentNonFixedNumberData, null, numberPrefab, numberLocations[currentNumberLocationIndex].transform.position, Quaternion.AngleAxis(90, Vector3.right));

        }

    }

    // ============================== GENERATE EQUATION ==============================

    public string generateNewEquation() {
        return equationsList[MainController.stageNumber];
    }


}