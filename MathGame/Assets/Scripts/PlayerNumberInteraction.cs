using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Player))]
public class PlayerNumberInteraction : MonoBehaviour {

    // ============================== INSPECTOR VARIABLES ==============================

    public AudioClip sound;
    private AudioSource source {get{return GetComponent<AudioSource>();}}
    // The prefab of the number object to spawn
    public GameObject numberPrefab;

    // The parent transform of the aim indicator particle system, for correct rotation
    public Transform aimRotator;

    // The aim indicator particle system
    public ParticleSystem aimIndicator;

    // Maximum distance to pickup items at
    public float itemPickupRange = 2;

    // For smooth aiming
    public float aimAngularAcceleration = 15f;

    // ============================== INTERNAL VARIABLES ==============================

    // The player script on the current player object
    Player playerScript;

    // For input
    string interactAxisName;

    // The closest number (to pickup) an distance to it
    Number closestNumber = null;
    float closestNumberDistance = float.MaxValue;

    // The closest number slot (to pickup number from and place number in)
    NumberSlot closestNumberSlot = null;

    // Flags for controlling aiming when a number is in range
    bool needToAim = false;
    bool isAiming = false;

    // The particle system's main module requires having it as a variable to change anything on it
    ParticleSystem.MainModule aimIndicatorMainModule;

    // ============================== START ==============================

    void Start() {
        // Audio Setup
        gameObject.AddComponent<AudioSource>();
        source.clip = sound;
        source.playOnAwake = false;

        // Get the player script and setup the reference to this script so other scripts can access this one
        playerScript = GetComponent<Player>();
        playerScript.playerNumberInteraction = this;

        // Stop the aim particles
        aimIndicator.Stop();
        aimIndicatorMainModule = aimIndicator.main;
        aimIndicatorMainModule.prewarm = true;

        // Determine name of axis (key bindings) for interaction based on left/right player
        string inputAxisSuffix = playerScript.isLeftPlayer ? "Left" : "Right";
        interactAxisName = "Interact" + inputAxisSuffix;

    }

    // ============================== UPDATE ==============================

    void Update() {

        // Enable aim if close to a number
        needToAim = (closestNumber != null && closestNumber.canBePickedUp);
        if (needToAim && !isAiming) {
            aimIndicator.Play();
            isAiming = true;
        }
        // Disable aim if not close to a number
        else if (!needToAim && isAiming) {
            aimIndicator.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            isAiming = false;
        }

        // If currently close to a number and is aiming
        if (isAiming) {

            // Aim towards mouse direction
            Vector3 direction = (MainController.mouseController.mousePosition - transform.position).normalized;
            aimRotator.rotation = Quaternion.Slerp(aimRotator.rotation, Quaternion.LookRotation(direction), aimAngularAcceleration * Time.deltaTime);

            // Move number on RMB
            if (Input.GetMouseButtonDown(1)) {

                // Mouse is over a number slot, move number to it
                if (MainController.mouseController.closestNumberSlot != null) {
                    closestNumber.numberMovement.setNumberSlotTarget(MainController.mouseController.closestNumberSlot);
                    playSound();
                }
                // Mouse is not in region of equation, move number in direction of mouse
                else {
                    closestNumber.numberMovement.setDirectionTarget(direction);
                    playSound();
                }

            }

        }

    }

    // ============================== PLAY PROJECTILE SOUND ==============================
    public void playSound(){
        source.PlayOneShot(sound);
    }

    // ============================== NUMBERS PROXIMITY ==============================

    public void setCloseNumber(Number number, float distance) {

        // If the player is not in the region of one of the sides of the equation
        // and a new closest number is found
        if ( /*!playerScript.playerInventory.hasItem &&*/ closestNumberSlot == null && (distance < closestNumberDistance || number == closestNumber)) {

            // Deselect the previous closest number if need be
            setNonCloseNumber(closestNumber);

            // Select the new closest number and update the distance of the closest number
            closestNumber = number;
            closestNumberDistance = distance;
            closestNumber.setSelected(playerScript.isLeftPlayer);

        }

    }

    public void setNonCloseNumber(Number number) {

        // If there is a previous closest number that is different from the currently closest one
        if (closestNumberSlot == null && closestNumber != null && number == closestNumber) {

            // Make sure to only do this if selected by this player only
            bool canDeselectThisNumber = playerScript.isLeftPlayer && !number.isSelectedByRightPlayer || !playerScript.isLeftPlayer && !number.isSelectedByLeftPlayer;
            if (canDeselectThisNumber) {

                // Deselect the previous closest number and reset the closest distance
                closestNumberDistance = float.MaxValue;
                number.setDeselected();
                closestNumber = null;

            }
        }

    }

}