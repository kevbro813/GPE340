using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// PlayerController accepts all player inputs
public class PlayerController : MonoBehaviour
{
    private PlayerPawn playerPawn; // PlayerPawn component
    private Animator anim; // Animator component
    private float inputVertical; // Float variable to hold the vertical inputs from the W/S keys
    private float inputHorizontal; // Float variable to hold the horizontal inputs from the A/D keys
    private Transform tf; // Transform component
    
    // Start is called before the first frame update
    void Start()
    {
        playerPawn = GetComponent<PlayerPawn>(); // Set the playerPawn variable
        anim = GetComponent<Animator>(); // Set the Animator component
        tf = GetComponent<Transform>(); // Set the transform component
    }

    // Update is called once per frame
    void Update()
    {
        if (playerPawn == null) // Check if the playerPawn is null
        {
            Debug.Log("Null Reference Exception: playerPawn not set to an instance."); // Send error message
        }
        else // If the playerPawn is not null...
        {
            inputVertical = Input.GetAxis("Vertical"); // Set the inputVertical to the Vertical axis value
            inputHorizontal = Input.GetAxis("Horizontal"); // Set the inputHorizontal to the Horizontal axis value

            // Create a vector3 with the horizontal and vertical inputs as the x and z axis, respectively. 
            // Note: The vertical input corresponds to the z axis (forward / back), but inputVertical is the y-axis with respect to controllers
            Vector3 moveDirection = new Vector3(inputHorizontal, 0.0f, inputVertical);

            // Clamp the move direction, which limits the player's movement speed
            moveDirection = Vector3.ClampMagnitude(moveDirection, GameManager.instance.maxSpeed);

            // Toggles world and self space (If true, the controls will be in world space, false is self)
            if (GameManager.instance.isMovementWorldSpace) 
            {
                moveDirection = tf.InverseTransformDirection(moveDirection); // InverseTransformDirection method converts the moveDirection Vector3 from self to world space
            }

            anim.SetFloat("Horizontal", moveDirection.x); // Set the horizontal float value from the animator to the moveDirection x-axis value
            anim.SetFloat("Vertical", moveDirection.z); // Set the vertical float value from the animator to the moveDirection z-axis value

            if (Input.GetButton("Crouch")) // If the crouch button is pressed (default: left ctrl)...
            {
                playerPawn.Crouch(true); // Crouch is active
            }
            else
            {
                playerPawn.Crouch(false); // Deactivate crouch
            }

            if (Input.GetButton("Run")) // If the run button is pressed (default: left shift)...
            {
                playerPawn.Run(true); // Run is active
            }
            else
            {
                playerPawn.Run(false); // Deactivate run
            }
            // If the camera is not controlled by the mouse, but the player is controlled by the mouse...
            if (!GameManager.instance.isCameraMouseControlled && GameManager.instance.isPlayerMouseControlled)
            {
                playerPawn.FaceMouseDir(); // Face the player in the direction of the mouse
            }
        }
    }

}
