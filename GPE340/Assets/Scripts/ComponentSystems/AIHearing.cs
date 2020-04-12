using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIHearing : MonoBehaviour
{
    private Transform tf; // This object's transform component
    private Transform ttf; // Target transform component
    private SphereCollider col; // Sphere collider component (hearing radius)
    private AIPawn aiPawn; // AIPawn component

    // Start is called before the first frame update
    void Start()
    {
        // Set component variables
        tf = GetComponent<Transform>();
        aiPawn = GetComponentInParent<AIPawn>();
        col = GetComponent<SphereCollider>();
        //col.enabled = true; // Enable the sound collider // TODO: Fix Investigate State being active simultaneously with other states (this will allow the collider to be enabled)
        col.radius = aiPawn.aiData.hearingRadius; // Set the hearingRadius to value stored in aiData
    }
    // When an object enters the hearing collider
    public void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player")) // Check if the other collider belongs to the player
        {
            ttf = other.GetComponent<Transform>(); // Set the target transform component to the player's
            aiPawn.canHear = true; // Indicate the AI can hear the player
        }
    }
    // When an object exits the hearing collider
    public void OnTriggerExit(Collider other) 
    {
        if (other.gameObject.CompareTag("Player")) // Check if the other collider belongs to the player
        {
            aiPawn.canHear = false; // Indicate the AI can no longer hear the player
        }
    }

    // Update method runs every frame
    void Update()
    {
        if (aiPawn.canHear == true) // If the AI can hear the player
        {
            if (ttf) // Check if the target transform component is not null
            {
                Vector3 vectorToSound = ttf.position - tf.position; // Determine the vector from the AI to the sound source

                RaycastHit hit; // Raycast hit will store if the raycast hits an object

                if (Physics.Raycast(tf.position, vectorToSound, out hit, col.radius)) // Raycast to sound location with hearing radius as maxDistance
                {
                    if (hit.collider.CompareTag("Player")) // Check if the object hit is the player
                    {
                        aiPawn.lastSoundLocation = hit.point; // Store the last sound location locally
                        GameManager.instance.lastSoundLocation = aiPawn.lastSoundLocation; // Store the last sound location in GameManager so the AI's allies know the location (Used for alert state)
                    }
                }
            }
            else // If there is no target transform component
            {
                aiPawn.canHear = false; // The AI cannot hear the player
            }
        }
    }
}
