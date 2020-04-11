using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Data/AIData")]
public class AIData : Base_Data
{
    [Header("AI Sense Variables")]
    public float fieldOfView = 55.0f; // Set Field of View in Unity, default 55 degrees
    public float maxViewDistance = 5.0f; // Max view distance of the object, can be set in Unity
    //public float maxAttackRange = 5.0f; // Max distance to be in attack range
    public float hearingRadius = 5.0f; // Max hearing radius
    [Space(10)]
    [Header("AI State Variables")]
    public float waitDuration; // Duration the AI will wait at a waypoint while patrolling
    public float waypointRange = 1.0f; // Distance from the waypoint the AI must be for it to register as reaching the waypoint
    public float searchDuration = 3; // Duration the AI will stay in the Search State before returning to patrol
    public float investigateDuration = 3; // Duration the AI will stay in the Investigate State before returning to patrol
    public float alertDuration = 3; // Duration the AI will stay in alert state before returning to patrol
    public float avoidanceDistance = 2; // Length of the raycast used for obstacle avoidance
    [Space(10)]
    [Header("AI Obstacle Avoidance Variables")]
    public float rotationHigh = 3; // High value used in Random.Range function to create a random length of time the AI will rotate
    public float rotationLow = 1; // Low value used in Random.Range function to create a random length of time the AI will rotate
    public float randomRotation; // Random value used to determine how long an AI will rotate while avoiding obstacles. This creates less predictable movement and patrol patterns
    [HideInInspector] public Vector3 lastSoundLocation; // lastSoundLocation vector set by raycast.point
    [HideInInspector] public bool canHear; // Used to activate the sound raycast
    [HideInInspector] public Vector3 lastPlayerLocation; // lastPlayerLocation vector set by raycast.point
}
