using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Possible values for the currently selected level
public enum Level {
    LEVEL1 = 0,
    LEVEL2 = 1,
    LEVEL3 = 2
}

public class MainController : MonoBehaviour {
    
    // Level names and number of stages (equations to solve) for each level
    public static string[] levelNames = {"Journey's Start", "Darkness", "High Speed"};
    public static int[] levelNumberOfStages = {10, 10, 10};
    public static int[] levelScorePerStage = {1000, 2000, 3000};

    // The currently selected mode and level
    public static bool onePlayerMode = true; // True = one player mode, False = two player mode
    public static Level selectedLevel = Level.LEVEL1;

    // The current stage (equation within level) that is being solved
    public static int stageNumber = 0;

    // The current score
    public static int score = 0;

    // Reference to the mouse controller that holds mouse position in the world and selected number slot
    public static MouseController mouseController;

    public static EquationController equationController;
    // References to the player scripts (on the player objects)
    // One of them will be null in one player mode
    public static Player leftPlayer;
    public static Player rightPlayer;

    // A list of all number scripts (on each number object) on the screen
    // Used for selecting the closest number to each player
    public static List<Number> numbers = new List<Number>();

    // The left and right number slot controllers
    // These contain references to each number slot
    // Those contain references to each number in the current equation
    public static NumberSlotController leftNumberSlotController = null;
    public static NumberSlotController rightNumberSlotController = null;

}