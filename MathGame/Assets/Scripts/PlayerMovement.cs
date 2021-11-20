using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Player))]
public class PlayerMovement : MonoBehaviour {

    // ============================== INSPECTOR VARIABLES ==============================

    // Settings
    public bool isControlledByMouse = true;
    public float minimumMouseDistance = 0.5f;
    public float speed = 100f;
    public float acceleration = 10f;

    // ============================== INTERNAL VARIABLES ==============================

    // The player script on the current player object
    Player playerScript;

    // The sprite sheet animator script on the current player object
    SpriteSheetAnimator animator;

    // For input
    string horizontalAxisName;
    string verticalAxisName;

    // For movement
    Rigidbody rigidbodyComponent;
    float horizontal;
    float vertical;

    // ============================== START ==============================

    void Start() {

        // Get the player script and setup the reference to this script so other scripts can access this one
        playerScript = GetComponent<Player>();
        playerScript.playerMovement = this;

        // Get the sprite sheet animator script
        animator = GetComponent<SpriteSheetAnimator>();

        // Get rigidbodyComponent for controlling movement
        rigidbodyComponent = GetComponent<Rigidbody>();

        // Determine names of axis (key bindings) for movement based on left/right player
        string inputAxisSuffix = playerScript.isLeftPlayer ? "Left" : "Right";
        horizontalAxisName = "Horizontal" + inputAxisSuffix;
        verticalAxisName = "Vertical" + inputAxisSuffix;

    }

    // ============================== UPDATE ==============================

    void Update() {

        // Read the movement input keys if keyboard input is used
        if (!isControlledByMouse) {
            horizontal = Input.GetAxisRaw(horizontalAxisName);
            vertical = Input.GetAxisRaw(verticalAxisName);
        }
        // Otherwise get mouse input
        else {

            // Get the direction of the mouse pointer, discarding the y-coordinate (no vertical movement)
            Vector3 direction = (MainController.mouseController.mousePosition - transform.position).normalized;

            // Calculate the cardinal direction
            // Taken from: https://gamedev.stackexchange.com/a/154765
            float angle = Mathf.Atan2(direction.z, direction.x);
            int cardinalDirection = Mathf.RoundToInt((angle * 2 / Mathf.PI) + 4) % 4;

            // Set the texture to face the correct direction
            animator.setTexture(cardinalDirection);

            // If LMB is held, check to move
            if (Input.GetMouseButton(0)) {

                // If the pointer is not already very close to the player
                if (Vector3.Distance(MainController.mouseController.mousePosition, transform.position) > minimumMouseDistance) {

                    // Set the direction to move in
                    horizontal = direction.x;
                    vertical = direction.z;

                    // Play the animation
                    animator.start();

                }
                // Otherwise, stop
                else {
                    horizontal = 0;
                    vertical = 0;
                    animator.stop();
                }

            }
            // Otherwise, stop
            else {
                horizontal = 0;
                vertical = 0;
                animator.stop();
            }
        }

    }

    void FixedUpdate() {

        // Apply physics-based movement in fixed update to keep consistent with the Unity timestep system
        float velocityMagnitude = speed * Time.deltaTime;

        // Make sure that diagonal movement is same speed as just up/down/left/right movement
        // If not controlled by mouse
        if (!isControlledByMouse && horizontal != 0 && vertical != 0) {
            velocityMagnitude *= 0.70710678f;
        }

        // Apply the final velocity value in the right direction
        rigidbodyComponent.velocity = Vector3.Lerp(rigidbodyComponent.velocity, new Vector3(horizontal * velocityMagnitude, 0, vertical * velocityMagnitude), acceleration * Time.deltaTime);

        // Make the player face the movement direction
        // If not controlled by mouse
        if (!isControlledByMouse && rigidbodyComponent.velocity != Vector3.zero) {
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(rigidbodyComponent.velocity), acceleration * Time.deltaTime);
        }

    }

}