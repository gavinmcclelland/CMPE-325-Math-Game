using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseController : MonoBehaviour {

    // ============================== INSPECTOR VARIABLES ==============================

    // Maximum distance to number slot for it to be considered close
    public float numberSlotRange = 2;

    // ============================== INTERNAL VARIABLES ==============================

    // The main camera, so it does not need to be found each frame
    Camera mainCamera;

    // The number slot controller (left/right side of equaiton) the mouse is currently over
    // Level is made so that the it is not possible to intersect two of these at any one time
    //NumberSlotController intersectingNumberSlotController = null;

    // The closest number slot (null when no number slots in range)
    [HideInInspector]
    public NumberSlot closestNumberSlot = null;

    // The world position of the mouse
    [HideInInspector]
    public Vector3 mousePosition;

    // The left and right number slot controllers
    // In an array to allow for accessing left and right parts by index
    NumberSlotController[] numberSlotControllers = new NumberSlotController[2];

    // If the mouse is over each part of the equation
    // Only one or zero elements will be true
    bool[] mouseOverEquationSide = new bool[2];

    // ============================== START ==============================

    void Start() {

        // Find the main camera
        mainCamera = Camera.main;
        // Register this script in the main controller
        MainController.mouseController = this;

        // Populate number slot controllers array
        numberSlotControllers[0] = MainController.leftNumberSlotController;
        numberSlotControllers[1] = MainController.rightNumberSlotController;

    }

    // ============================== UPDATE ==============================

    void Update() {

        // Get the direction of the mouse pointer, discarding the y-coordinate (no vertical movement)
        Vector3 rawMousePosition = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        mousePosition = new Vector3(rawMousePosition.x, 1, rawMousePosition.z);

        // For both left and right sides of equation
        for (int i = 0; i < 2; i++) {

            // Check if mouse over each side of equation
            mouseOverEquationSide[i] = numberSlotControllers[i].border.bounds.Contains(mousePosition);

            // If it is over the current side
            if (mouseOverEquationSide[i]) {

                // Get the currently closest number slot
                float closestDistance;
                NumberSlot possibleCloseNumberSlot = HelperFunctions.getClosestNumberSlot(mousePosition, numberSlotControllers[i], out closestDistance);

                // If it is valid (does not and will not contain a number yet) and close enough
                if (possibleCloseNumberSlot != null && possibleCloseNumberSlot.reservedNumber == null && closestDistance <= numberSlotRange) {
                    closestNumberSlot = possibleCloseNumberSlot;
                }
                // Otherwise, if the mouse is not close enough to any number slots
                // Count is as not being over the equation
                else {
                    mouseOverEquationSide[i] = false;
                }

            }

        }

        // If the mouse is not over any number slots, forget about the closest number slot
        if (!mouseOverEquationSide[0] && !mouseOverEquationSide[1]) {
            closestNumberSlot = null;
        }

        // Debug draw
        if (closestNumberSlot != null) {
            Debug.DrawLine(closestNumberSlot.transform.position, mousePosition, Color.green);
        }

    }

}