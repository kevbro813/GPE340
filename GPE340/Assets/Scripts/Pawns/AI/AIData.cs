using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Data/AIData")]
public class AIData : Base_Data
{
    [Header("AI Sense Variables")]
    public float fieldOfView = 55.0f; // AI's field of view, used with AIVision raycast
    public float maxViewDistance = 5.0f; // Max view distance of the AI
    public float hearingRadius = 5.0f; // Max hearing radius
    [Space(10)]
    [Header("AI State Variables")]
    public float waitDuration = 3.0f; // Duration the AI will wait at a waypoint while patrolling
    public float waypointRange = 1.0f; // Distance from the waypoint the AI must be for it to register as "reaching" the waypoint
    public float searchDuration = 3.0f; // Duration the AI will stay in the Search State before returning to patrol state
    public float investigateDuration = 3.0f; // Duration the AI will stay in the Investigate State before returning to patrol state
    public float alertDuration = 3.0f; // Duration the AI will stay in alert state before returning to patrol state
    public float avoidanceDistance = 2; // Length of the raycast used for obstacle avoidance
    [Space(10)]
    [Header("AI Obstacle Avoidance Variables")]
    public float rotationHigh = 3; // High value used in Random.Range function to create a random length of time the AI will rotate
    public float rotationLow = 1; // Low value used in Random.Range function to create a random length of time the AI will rotate
    public float rotationSpeed = 5; // Speed the AI will rotate when avoiding an obstacle
    [Space(10)]
    [Header("AI Health Variables")]
    public float criticalHealth = 100; // When the AI's current health is less than or equal to critical health, the AI will enter the Flee state and run away from the player
}
