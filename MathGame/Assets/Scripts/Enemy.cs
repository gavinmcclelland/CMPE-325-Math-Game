using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class Enemy : MonoBehaviour {

    // ============================== INSPECTOR VARIABLES ==============================
    
    // Audio setup
    public AudioClip sound;
    private AudioSource source {get{return GetComponent<AudioSource>();}}
    // The speed to move at
    public float movementSpeed = 4f;

    // How close to the target position to get before switching to the next one
    public float targetDistanceThreshold = 0.01f;

    // If currently moving to the left or right
    public float currentTargetIndex = 0;

    // The target positions
    public Transform leftTarget;
    public Transform rightTarget;

    // ============================== INTERNAL VARIABLES ==============================

    // The current position to move to
    Vector3 currentTargetPosition;

    // ============================== START ==============================

    void Start() {
        // Set the current position to move to
        currentTargetPosition = currentTargetIndex == 0 ? leftTarget.position : rightTarget.position;

        // Audio Setup
        gameObject.AddComponent<AudioSource>();
        source.clip = sound;
        source.playOnAwake = false;
    }

    // ============================== UPDATE ==============================

    void Update() {

        // The speed and direction move in
        float currentSpeed = movementSpeed * Time.deltaTime;
        Vector3 movementOffset = (currentTargetPosition - transform.position).normalized;

        // Move towards the current target
        transform.Translate(movementOffset * currentSpeed, Space.World);

        // If the target is reached
        if (Vector3.Distance(transform.position, currentTargetPosition) < targetDistanceThreshold) {

            // Flip the x-orientation to make the texture face in the proper direction
            transform.localScale = new Vector3(transform.localScale.x * -1, transform.localScale.y, transform.localScale.z);

            // Set the current position to move to
            currentTargetIndex = currentTargetIndex == 0 ? 1 : 0;
            currentTargetPosition = currentTargetIndex == 0 ? leftTarget.position : rightTarget.position;

        }


    }

    // ============================== COLLISION ==============================

    void OnTriggerEnter(Collider col) {

        // If the player hit the enemy
        if (col.gameObject.tag == "Player") {

            // Get he player scipt
            Player playerScript = col.gameObject.GetComponent<Player>();

            // Decrement the number of lives
            playerScript.numLives--;

            // Decrement score
            if (MainController.score >= 100) {
                MainController.score -= 100;
            }

            // Play sound when hit
            playSound();

            // Move the player back to the spawn point
            MainController.leftPlayer.transform.position = LevelController.playerSpawnPoint;

        }
    }

    public void playSound(){
        source.PlayOneShot(sound);
    }
}