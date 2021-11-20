using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {

    // ============================== INSPECTOR VARIABLES ==============================

    // Used by this and other scripts attached to the current player object
    public bool isLeftPlayer = true;

    // ============================== INTERNAL VARIABLES ==============================

    // References to the subscripts controlling the player object
    // This is done so all scripts can access each other
    [HideInInspector]
    public PlayerMovement playerMovement;
    [HideInInspector]
    public PlayerNumberInteraction playerNumberInteraction;

    public int numLives = 4;
    //[HideInInspector]
    //public PlayerInventory playerInventory;

    // ============================== START ==============================

    void Start() {

        // Set the reference of the corresponding player script in the main controller to this script
        // This is done so all other scripts can easily access the player scipts
        if (isLeftPlayer) {
            MainController.leftPlayer = this;
        }
        else {
            MainController.rightPlayer = this;
        }

        // TODO: If in darkness mode, enable flashlight

    }

}