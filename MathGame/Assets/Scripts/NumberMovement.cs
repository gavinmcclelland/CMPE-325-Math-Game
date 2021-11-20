using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NumberMovement : MonoBehaviour {

    // ============================== INSPECTOR VARIABLES ==============================

    // The speed at which to move to the assigned number slot
    public float moveToNumberSlotSpeed = 5.0f;

    // The speed at which to move to some world position, for smoothly hovering into place
    public float moveToPositionSpeed = 5.0f;

    // The speed at which to move in some direction, for moving across the world
    public float moveInDirectionSpeed = 5.0f;

    // Maximum distance to number slot for moving to it
    public float numberSlotRange = 1.7f;

    // ============================== INTERNAL VARIABLES ==============================

    // The number script on the current number object
    [HideInInspector]
    public Number numberScript;

    // The number slot controller (left/right side of equaiton) the number is currently in
    // Level is made so that the number cannot intersect two of these at any one time
    NumberSlotController intersectingNumberSlotController = null;

    // For movement
    Rigidbody rigidbodyComponent;

    // Flags to control behaviour
    [HideInInspector]
    public bool needToMoveToTarget = false;
    [HideInInspector]
    public bool needToChangeScale = true;
    [HideInInspector]
    public bool needToMoveInDirection = false;

    // The current scale to shrink/grow to
    [HideInInspector]
    public float scaleTarget = 1.0f;

    // The current transform's position to follow (higher precedence)
    // Or just position to move to
    [HideInInspector]
    public Vector3 movementTargetPosition;

    // The current direction to apply physics-based movement in
    [HideInInspector]
    public Vector3 movementTargetDirection;

    // ============================== START ==============================

    void initialize() {
        if (rigidbodyComponent == null) {
            // Get rigidbody component for controlling movement
            rigidbodyComponent = GetComponent<Rigidbody>();
        }
    }

    void Start() {
        // Initialize if need be
        initialize();
    }

    // ============================== UPDATE ==============================

    void Update() {

        // Only move if number is not fixed
        if (numberScript.canBePickedUp) {

            // Smooth movement to target position if need be
            if (needToMoveToTarget && !needToMoveInDirection) {
                //transform.position = Vector3.Lerp(transform.position, movementTargetPosition, currentSpeed);

                // The speed and direction move in
                float currentSpeed = moveToPositionSpeed * Time.deltaTime;
                Vector3 movementOffset = movementTargetPosition - transform.position;
                Vector3 movementOffsetNormalized = movementOffset.normalized;

                // When close to a number slot, slow down to smoothly "float" into place
                Vector3 currentMovementDirection;
                if (movementOffset.sqrMagnitude > movementOffsetNormalized.sqrMagnitude) {
                    currentMovementDirection = movementOffsetNormalized;
                }
                else {
                    currentMovementDirection = movementOffset;
                }

                // Move towards the number slot at the required speed
                transform.Translate(currentMovementDirection * currentSpeed, Space.World);

                // If reached close enough to target
                if (Vector3.Distance(transform.position, movementTargetPosition) < 0.01) {

                    // Assign to the closest number slot to become part of equation
                    assignToClosestNumberSlot();

                    // Stop trying to move any closer
                    needToMoveToTarget = false;

                }
            }

            // Add force in direction if need be
            if (needToMoveInDirection) {
                needToMoveInDirection = false;
                rigidbodyComponent.AddForce(movementTargetDirection * moveInDirectionSpeed, ForceMode.VelocityChange);
            }

            // If in region of equation
            if (intersectingNumberSlotController != null) {

                // If not already moving to a number slot
                // Assign to the closest one
                if (!needToMoveToTarget) {
                    assignToClosestNumberSlot();
                }

            }

        }

        // Smooth scaling to set scale if need be (for spawn/despawn)
        if (needToChangeScale) {
            transform.localScale = Vector3.Lerp(transform.localScale, new Vector3(scaleTarget, scaleTarget, scaleTarget), Time.deltaTime * moveToNumberSlotSpeed);

            // If reached close enough to scale, stop trying to change scale any move
            if (Mathf.Abs(transform.localScale.x - scaleTarget) < 0.01) {
                needToChangeScale = false;

                // If target was to scale down to become invisible,
                // the number is being picked up, so need to remove it from the world
                if (scaleTarget == 0) {
                    Destroy(gameObject);
                }
            }
        }

    }

    //void FixedUpdate() {
    // TODO: If in high speed mode, move number around
    //}

    // ============================== ASSIGN TO CLOSEST NUMBER SLOT ==============================

    public void assignToClosestNumberSlot() {

        // Get the currently closest number slot
        float closestDistance;
        NumberSlot closestNumberSlot = HelperFunctions.getClosestNumberSlot(transform.position, intersectingNumberSlotController, out closestDistance);

        // If it is valid (does not contain a number yet) and close enough
        if (closestNumberSlot != null && closestDistance <= numberSlotRange) {

            // If not already moving into place, first smoothly move the the center of the number slot
            // This is done after the number is moved in a direction instead of directly to a number slot
            if (!needToMoveToTarget) {
                setNumberSlotTarget(closestNumberSlot);
            }
            // Otherwise, the number is already in the right position so become part of the equation
            else {
                // First clear the number slot this number was in (make it not contain any number)
                numberScript.unAssignFromNumberSlot();
                // Then assign this number to the closest number slot if it is empty (move into the equation)
                numberScript.assignToNumberSlot(closestNumberSlot);
            }

        }

    }

    // ============================== NUMBER SLOTS PROXIMITY ==============================

    void OnTriggerEnter(Collider col) {

        // If the number just entered into the region of one of the sides of the equation
        if (col.gameObject.tag == "Number Slot Controller") {
            // Get the number slots in that region
            intersectingNumberSlotController = col.gameObject.GetComponent<NumberSlotController>();
        }

    }

    void OnTriggerExit(Collider col) {

        // If the number just left the region of one of the sides of the equation
        if (col.gameObject.tag == "Number Slot Controller") {

            // Forget about the number slots in that region
            intersectingNumberSlotController = null;

            // Clear the number slot this number was in (make it not contain any number)
            numberScript.unAssignFromNumberSlot();

        }

    }

    // ============================== SCALE TARGET ==============================

    public void setScaleTarget(float scaleTargetToSet) {
        // Set scale flag and target
        needToChangeScale = true;
        scaleTarget = scaleTargetToSet;
    }

    // ============================== NUMBER SLOT TARGET ==============================

    public void setNumberSlotTarget(NumberSlot numberSlotTarget) {

        // If the target number lsot already has some other number being noved to it
        // Don't move this number to it to avoid duplicated numbers in number slot
        if(numberSlotTarget.reservedNumber != null && numberSlotTarget.reservedNumber != numberScript) {
            return;
        }

        // Set movement flag and target as number slots's position
        needToMoveToTarget = true;
        movementTargetPosition = numberSlotTarget.transform.position;

        // Indicate that no other numbers can be moved into the number slot
        numberSlotTarget.reservedNumber = numberScript;

        // Stop moving in the current direction to properly move further
        rigidbodyComponent.velocity = Vector3.zero;

    }

    // ============================== POSITION TARGET ==============================

    public void setMovementTarget(Transform movementTargetTransformToSet) {

        // Set movement flag and target as transform's position
        needToMoveToTarget = true;
        movementTargetPosition = movementTargetTransformToSet.position;

        // Stop moving in the current direction to properly move further
        rigidbodyComponent.velocity = Vector3.zero;

    }

    public void setMovementTarget(Vector3 movementTargetPositionToSet) {

        // Set movement flag and target as position
        needToMoveToTarget = true;
        movementTargetPosition = movementTargetPositionToSet;

        // Stop moving in the current direction to properly move further
        rigidbodyComponent.velocity = Vector3.zero;

    }

    // ============================== DIRECTION TARGET ==============================

    public void setDirectionTarget(Vector3 movementTargetDirectionToSet) {

        // If still moving to number slot, don't move in a direction
        if (needToMoveToTarget) { return; }

        // Set movement in direction flag and target as direction
        needToMoveInDirection = true;
        movementTargetDirection = movementTargetDirectionToSet;

        // Stop moving in the current direction to properly move further
        rigidbodyComponent.velocity = Vector3.zero;

    }

    // ============================== SET KINEMATIC ==============================

    public void setKinematic() {

        // Initialize if need be
        initialize();

        // Set the rigitbody to never move
        // This is called when the number is fixed in the equation
        rigidbodyComponent.isKinematic = true;

    }

}