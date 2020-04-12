using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIVision : MonoBehaviour
{
    private Transform ttf; // Target transform component
    private Transform tf; // Current object's transform component
    private AIPawn aiPawn; // Current object's AIPawn component
    [HideInInspector] public float targetDistance; // Distance from the AI to the target

    void Start()
    {
        aiPawn = GetComponentInParent<AIPawn>(); // Set AIPawn component variable
        tf = GetComponent<Transform>(); // Set Transform component variable
    }
    // CanSee method will return true if the player is seen or false if the player is not seen
    public bool CanSee(List<GameObject> target) // A list of all active players is passed in
    {
        foreach (GameObject player in target) // Loop through each player in the list
        {
            ttf = player.GetComponent<Transform>(); // Get the player's transform component

            Vector3 vectorToTarget = ttf.position - tf.position; // Get the vector from the AI to the player transform

            targetDistance = Vector3.Distance(ttf.position, tf.position); // Calculate the distance between the AI and player

            float angleToTarget = Vector3.Angle(vectorToTarget, tf.forward); // Calculate the angle to the target from the AI's forward vector

            // If the player is within the AI's field of view and within the maxViewDistance...
            if (angleToTarget < aiPawn.aiData.fieldOfView && targetDistance < aiPawn.aiData.maxViewDistance)
            {
                int environmentLayer = LayerMask.NameToLayer("Environment"); // Set environment layer mask
                int playerLayer = LayerMask.NameToLayer("Player"); // Set player layer mask
                int layerMask = (1 << playerLayer) | (1 << environmentLayer); // Create a layer mask that limits what the vision component can see

                RaycastHit hit; // Store Raycast hit variable to indicate player location

                // Check if an object intersects the raycast (must be part of the layermask and within the maxViewDistance)
                if (Physics.Raycast(tf.position, vectorToTarget, out hit, aiPawn.aiData.maxViewDistance, layerMask))
                {
                    if (hit.collider.CompareTag("Player")) // If the object hit by the raycast is a player
                    {
                        aiPawn.lastPlayerLocation = hit.point; // Set the last player location locally
                        GameManager.instance.lastPlayerLocation = aiPawn.lastPlayerLocation; // Set the last player location in GameManager so allies are aware of player location
                        return true; // Return true to indicate the player has been spotted
                    }
                }
            }
        }
        return false; // Return false to indicate the player has not been seen
    }
    // This method determines if the target is within attack range
    public bool AttackRange(float distanceToTarget)
    {
        // If the target is within attack range...
        if (distanceToTarget <= aiPawn.weaponEffectiveRange)
        {
            return true; // Return true to indicate the target is in range
        }
        return false; // Return false to indicate the target is out of range
    }
}
