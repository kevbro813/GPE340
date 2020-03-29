using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// PlayerPawn script has all the basic movement methods, besides horizontal and vertical movement.
public class PlayerPawn : MonoBehaviour
{
    private Transform tf; // Transform component for the player gameObject
    private Animator anim; // Animator component for the player gameObject

    private void Awake()
    {
        tf = GetComponent<Transform>(); // Set the transform component
        anim = GetComponent<Animator>(); // Set the animator component
    }
    // Method for crouch animation
    public void Crouch(bool active) // Bool variable is passed to this method to determine if the player should crouch
    {
        if (active) // Check if crouch is active
        {
            anim.SetBool("isCrouching", true); // Set the animator "isCrouching" bool to true
        }
        else
        {
            anim.SetBool("isCrouching", false); // Set the animator "isCrouching" bool to false
        }
    }
    // Method for running
    public void Run(bool active) // Bool variable is passed to this method to determine if the player should run
    {
        if (active) // Check if run is active
        {
            anim.SetBool("isRunning", true); // Set the animator "isRunning" bool to true
        }
        else
        { 
            anim.SetBool("isRunning", false); // Set the animator "isRunning" bool to false
        }
    }
    // Method that will face the player in the direction of the mouse
    public void FaceMouseDir()
    {
        Plane plane = new Plane(Vector3.up, tf.position); // Create a new plane for the mouse position to use as a reference 
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition); // Create a ray from the the mouse position

        float enter; // Distance variable

        if (plane.Raycast(ray, out enter)) // If the ray from the mouse position intersects the plane...
        {
            Vector3 targetPoint = ray.GetPoint(enter); // Get the point the ray and plane intersect
            // Convert the Vector3 targetPoint to a Quaternion using the Quaternion.LookRotation method. This determines the rotation needed to turn the playerPawn in the direction of the targetPoint (mouse position)
            Quaternion targetRotation = Quaternion.LookRotation(targetPoint - tf.position); 
            // Change the rotation of the camera using the RotateTowards method. Parameters passed in are the origin, the direction and the rotation speed, respectively.                                                                                 
            tf.rotation = Quaternion.RotateTowards(tf.rotation, targetRotation, GameManager.instance.rotationSpeed);
        }
    }
}
