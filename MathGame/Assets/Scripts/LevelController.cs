using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelController : MonoBehaviour {

    // ============================== INSPECTOR VARIABLES ==============================

    // The score and current level labels UI components
    public Text scoreLabel;
    public Text levelLabel;

    // The player parameters
    public int playerMaxLives = 4;
    public Vector3 playerSpawnPointPosition = new Vector3(0, 1, -3);
    public static Vector3 playerSpawnPoint;

    // ============================== INTERNAL VARIABLES ==============================

    // The last score that was set to the score label text
    int previousScore = -1;

    // ============================== START ==============================

    private void Start() {
        
        // Set the level text to the current level number and name
        levelLabel.text = ((int) MainController.selectedLevel + 1) + ": " + MainController.levelNames[(int) MainController.selectedLevel];
        
        // Set the static player position to the ose set in the inspector
        playerSpawnPoint = playerSpawnPointPosition;

        // Clear all data
        MainController.score = 0;
        MainController.stageNumber = 0;
        MainController.numbers = new List<Number>();

        // TODO: If in darkness mode, disable/dim global light

    }

    // ============================== UPDATE ==============================

    private void Update() {

        // Update the score text to current score when it changes
        if (previousScore != MainController.score) {
            previousScore = MainController.score;
            scoreLabel.text = "" + MainController.score;
        }

        // If the player lost all the lives
        if (MainController.leftPlayer.numLives == 0) {

            // Reset the equation
            MainController.equationController.needToGenerateNewEquation = true;

            // Move the player back to the spawn point and reset number of lives
            MainController.leftPlayer.transform.position = playerSpawnPoint;
            MainController.leftPlayer.numLives = playerMaxLives;

        }

    }
}