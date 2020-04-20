using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

[RequireComponent(typeof(AIData))]
// AIPawn has methods run by AI enemies
public class AIPawn : Base_Pawn
{
    [Header("AI Settings")]
    [HideInInspector] public AIData aiData; // AIData (set in inspector)
    [HideInInspector] public GameObject[] weaponSelection; // Array that contains the weapons available to the AI (Set in inspector)
    private AIVision aiVision; // Vision component
    private NavMeshAgent nma; // NavMeshAgent component
    private Animator anim; // Animator component
    private Rigidbody rb; // Rigidbody component

    public Image enemyHealthBar;

    [Header("Patrol Settings")]
    public PatrolType patrolType; // Allows the patrol type to be changed in the Inspector
    public WaypointType waypointType; // Allows the type of waypoints used to be changed in the Inspector
    public enum WaypointType { Global, Local } // Selections for Waypoint Type (Global are waypoints saved in GameManager, local would be local waypoints that are used with prefabs in local space)
    public enum PatrolType { Random, Stop, Loop, PingPong }; // Selections for Patrol Type
    public List<Transform> enemyWaypoints; // Array of all the enemy waypoints being used by the AI
    private int currentWaypoint = 0; // Tracks the waypoint that is currently being used by an AI
    private bool isPatrolForward = true; // Used in "PingPong" patrol type to loop through the waypoints in reverse order

    [Header("FSM Settings")]
    [HideInInspector] public bool atWaypoint = false; // Indicates whether the AI is currently at a waypoint while patrolling
    [HideInInspector] public bool atSearchLocation = false; // Indicates whether the AI is currently at the last known player location while searching
    [HideInInspector] public bool atInvestigateLocation = false; // Indicates whether the AI is currently looking in the direction of a sound while investigating
    [HideInInspector] public bool atAlertLocation = false; // Indicates whether the AI is currently at the alert location
    [HideInInspector] public bool isInvestigating = false; // Switching from true to false allows the AI to transition from investigate state to patrol state
    [HideInInspector] public bool isSearching = false; // Switching from true to false allows the AI to transition from search state to patrol state
    [HideInInspector] public bool isAlertActive = false; // Switching from true to false allows the AI to transition from alert state to patrol state
    [HideInInspector] public bool isFleeing = false; // Indicates if the current AI is fleeing the player
    [HideInInspector] public bool canHear; // Used to activate AI Hearing
    private bool isTurned = false; // Is the AI turned around (Used in Flee function)
    private float waitTime; // Used to track how long to wait at a waypoint while patrolling
    private float investigateTime; // How long the AI will investigate before returning to patrol
    private float searchTime; // How long the AI will search before returning to patrol
    private float alertTime; // How long the AI will be alert before returning to patrol
    [HideInInspector] public float randomRotation; // Random value used to determine how long an AI will rotate while avoiding obstacles. This creates less predictable movement and patrol patterns
    [HideInInspector] public float weaponEffectiveRange; // Effective range of the currently equipped weapon (Set when the weapon is equipped and used to determine attackRange)
    [HideInInspector] public Vector3 lastSoundLocation; // lastSoundLocation vector set by raycast.point
    [HideInInspector] public Vector3 lastPlayerLocation; // lastPlayerLocation vector set by raycast.point
    [HideInInspector] public RaycastHit obstacleHit; // Raycast hit for obstacles

    [Header("Item Drop Settings")]
    public ItemDrops[] itemDrops;
    public double itemDropChance = 0.5f;
    public double[] cdfArray;

    // Override and use the Awake method
    public override void Awake() 
    {
        base.Awake(); // Run base Awake method

        // Set component variables
        nma = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();
        aiVision = GetComponentInChildren<AIVision>();

        SetMaxHealth(aiData.GetDefaultMaxHealth()); // Set max health to the default max health
        SetCurrentHealth(GetMaxHealth()); // Set current health to max health

        // If global waypoints are used, then get them from the GameManager
        if (waypointType == WaypointType.Global)
        {
            enemyWaypoints = GameManager.instance.enemyWaypoints;
        }
        // If random patrol is used, get an initial random waypoint to use
        if (patrolType == PatrolType.Random)
        {
            currentWaypoint = Random.Range(0, enemyWaypoints.Count);
        }
    }

    // Update the enemy's health bar
    public void UpdateEnemyHealthBar()
    {
        enemyHealthBar.fillAmount = GetCurrentHealthPercentage(); // Change the health bar to match the enemy's health percentage
    }
    private void Start()
    {
        SetCDFArray(); // Set CDF array for weighted drops
        DisableRagdoll(); // Ensure ragdoll is disabled when the AI is spawned
        SetDefaultWeapon(); // Set the AI's default weapon (random assignment)

        // Set all state duration variables
        investigateTime = aiData.investigateDuration; // How long the AI will investigate a sound
        searchTime = aiData.searchDuration; // How long an AI will search for the player
        waitTime = aiData.waitDuration; // How long the AI will wait at a waypoint
        alertTime = aiData.alertDuration; // How long the AI will be alerted before returning to patrol
        randomRotation = Random.Range(aiData.rotationLow, aiData.rotationHigh); // Create a randomRotation at start
        SetCDFArray();
    }
    // Select an item to drop using Array.BinarySearch
    public Object SelectItemToDrop()
    {
        System.Random rnd = new System.Random(); // Get a random double
        double rand = rnd.NextDouble(); // Get the next random double in sequence
        int index = System.Array.BinarySearch(cdfArray, rand * cdfArray.Last()); // Get a random value for index
        if (index < 0) // Check if index is negative
            index = ~index; // If negative then use bitwise 
        return itemDrops[index].GetValue(); // Return object
    }
    // Create the cumulative distribution function array
    public void SetCDFArray()
    {
        cdfArray = new double[itemDrops.Length]; // Create a new double array of itemDrops length
        cdfArray[0] = itemDrops[0].GetChance(); // Set the first CDF to the first itemDrops value

        for (int i = 1; i < cdfArray.Length; i++) // Loop through the rest of the CDF array
        {
            cdfArray[i] = cdfArray[i - 1] + itemDrops[i].GetChance(); // Set the CDF array to cumulative value of all previous indices
        }
    }
    // Drop an item on when killed
    public void DropItem()
    {
        if (Random.Range(0f, 1f) < itemDropChance) // Chance an item will drop
        {
            Instantiate(SelectItemToDrop(), tf.position + Vector3.up, Quaternion.identity); // Instantiate the selected item
        }
    }
    // Override the UnequipWeapon method
    public override void UnequipWeapon()
    { 
        base.UnequipWeapon(); // Use the base method
    }
    // Override the EquipWeapon method
    public override void EquipWeapon(GameObject weap) // Pass in the weapon that is being equipped
    {
        base.EquipWeapon(weap); // Use the base method
        Weapon weaponComponent = equippedWeapon.GetComponent<Weapon>(); // Set the weaponComponent of the currently equipped weapon
        weaponComponent.SetIsPlayerWeapon(false); // Also set isPlayerWeapon to false (since this is an AI)
        weaponEffectiveRange = weaponComponent.GetEffectiveRange(); // Set the effectiveRange stored in the AIPawn component to the weapon's effectiveRange
    }
    // Set the AI's default weapon randomly
    public void SetDefaultWeapon()
    {
        // Instantiate a random weapon from the weaponSelection list
        GameObject weaponClone = Instantiate(weaponSelection[Random.Range(0, weaponSelection.Length)]) as GameObject;
        EquipWeapon(weaponClone); // Equip the weapon
    }
    // Match the NavMeshAgents velocity to the velocity of the animator which prevents the AI from moving erratically 
    private void OnAnimatorMove()
    {
        nma.velocity = anim.velocity;
    }
    // Check if the tank is facing the target and return a bool
    public bool FacingTarget(Vector3 target)
    {
        Vector3 vectorToTarget = target - tf.position; // Calculate the vector to the target

        float dotProd = Vector3.Dot(vectorToTarget, tf.forward); // Find the dot product of the vector to target and forward vector of the AI
        if (dotProd > 0.9f) // If the dotProduct is close to 1 (default 0.9), then the AI is close enough to facing the target
        {
            return true; // Indicate the AI is facing the target
        }
        return false; // Indicate the AI is not facing the target
    }
    // Check if the tank is facing away from the target and return a bool, used for flee method
    public bool BackToTarget(Vector3 target)
    {
        Vector3 vectorAwayFromTarget = (target - tf.position) * -1; // Set vector in the inverse of the vector to target

        float dotProd = Vector3.Dot(vectorAwayFromTarget, tf.forward); // Find the dot product of the vector to target and forward vector of the AI
        if (dotProd > 0.9) // If the dotProduct is close to 1 (default 0.9), then the AI is close enough to facing directly away from the target
        {
            return true; // Indicate the AI has its back to the target
        }
        return false; // Indicate the AI does not have its back to the target
    }
    // Method to move the AI towards a target
    public void MoveTowards(Vector3 target)
    {
        nma.SetDestination(target); // Set the destination of the NavMeshAgent to the Vector3 target passed to this method
        // Calculate the velocity to move the AI towards the target
        Vector3 inputMovement = Vector3.MoveTowards(rb.velocity, nma.desiredVelocity, nma.acceleration * Time.deltaTime);
        inputMovement = transform.InverseTransformDirection(inputMovement); // Convert to world space

        anim.SetFloat("Horizontal", inputMovement.x); // Set the animator float variable responsible for horizontal movement
        anim.SetFloat("Vertical", inputMovement.z); // Set the animator float variable responsible for vertical movement
    }
    // Rotate the AI towards a target
    public void RotateTowards(Vector3 target, float rotationSpeed) // Target and rotation speed passed in
    {
        Vector3 vectorToTarget = target - tf.position; // Calculate the vector to target
        Quaternion targetRotation = Quaternion.LookRotation(vectorToTarget); // Calculate quaternion to rotate the AI towards the target

        // Set the rotation using RotateTowards for smooth rotation to target
        tf.rotation = Quaternion.RotateTowards(tf.rotation, targetRotation, rotationSpeed * Time.deltaTime);
    }
    // Rotate the AI away from a target
    public void RotateAway(Vector3 target, float rotationSpeed) // Target and rotation speed passed in
    { 
        Vector3 vectorAwayFromTarget = (target - tf.position) * -1; // Calculate the inverse to the vector to target
        Quaternion targetRotation = Quaternion.LookRotation(vectorAwayFromTarget); // Calculate quaternion to rotate the AI away from the target

        // Set the rotation using RotateTowards for smooth rotation away from the target
        tf.rotation = Quaternion.RotateTowards(tf.rotation, targetRotation, rotationSpeed * Time.deltaTime);
    }

    // Rotate and move away from obstacles
    public void ObstacleAvoidance(RaycastHit hit)
    {
        RotateAway(obstacleHit.point, aiData.rotationSpeed);
        //MoveTowards(); // Might need to move the AI forward some amount while in avoidance state
    }

    // Check if there is an obstacle in the way and return a bool
    public bool ObstacleCheck()
    {
        // Raycast forward from AI
        if (Physics.Raycast(tf.position, tf.forward, out obstacleHit, aiData.avoidanceDistance))
        {
            // If the collider hit is the arena or another enemy...
            if (obstacleHit.collider.CompareTag("Arena") || obstacleHit.collider.CompareTag("Enemy"))
            {
                return true; // Indicate the AI needs to enter obstacle avoidance state
            }
        }
        return false; // Indicates the AI's path is clear
    }
    // Flee when health is low
    public void Flee()
    {
        // Rotate away from the target
        if (BackToTarget(lastPlayerLocation) == false && isTurned == false)
        {
            RotateAway(lastPlayerLocation, aiData.rotationSpeed);
        }
        // Once facing away from target, set isTurned to true
        if (BackToTarget(lastPlayerLocation) == true)
        {
            isTurned = true; // Indicates the AI has completed turning phase
        }
        // When the AI is done rotating, move the AI away from the target
        if (isTurned == true)
        {
            // TODO: MoveAway method
        }
    }
    // Patrol method with various patrol types
    public void Patrol()
    {
        // If not facing target and not at waypoint
        if (FacingTarget(enemyWaypoints[currentWaypoint].position) == false && atWaypoint == false)
        {
            // Rotate towards the waypoint
            RotateTowards(enemyWaypoints[currentWaypoint].position, aiData.rotationSpeed);
        }
        // If facing waypoint, but not at waypoint
        if (FacingTarget(enemyWaypoints[currentWaypoint].position) == true && atWaypoint == false)
        {
            // Move towards the waypoint
            MoveTowards(enemyWaypoints[currentWaypoint].position);
        }
        // Stop at waypoint
        if (Vector3.SqrMagnitude(enemyWaypoints[currentWaypoint].position - tf.position) < (aiData.waypointRange * aiData.waypointRange))
        {
            atWaypoint = true; // AI stops moving
            // After waitTime delay
            if (waitTime <= 0)
            {
                // Next waypoint is random
                if (patrolType == PatrolType.Random)
                {
                    RandomPatrol();
                }
                // Loop back to first waypoint
                if (patrolType == PatrolType.Loop)
                {
                    LoopPatrol();
                }
                // Stop at the last waypoint
                if (patrolType == PatrolType.Stop)
                {
                    StopPatrol();
                }
                // Reverse through the order of waypoints
                if (patrolType == PatrolType.PingPong)
                {
                    PingPongPatrol();
                }
                atWaypoint = false; // No longer at waypoint
                waitTime = aiData.waitDuration; // Reset waitTime
            }
            else
            {
                waitTime -= Time.deltaTime; // Decrement time (timer)
            }
        }
    }
    // Alerted state will move to last known player location (static location)
    public void Alerted()
    {
        if (isAlertActive == true) // If there is an active alert
        {
            // Go to the last known player location (global - saved in GameManager)
            // If not facing the target or at the target
            if (FacingTarget(GameManager.instance.lastPlayerLocation) == false && atAlertLocation == false)
            {
                // Rotate towards target
                RotateTowards(GameManager.instance.lastPlayerLocation, aiData.rotationSpeed);
            }
            // Stop when facing target
            if (FacingTarget(GameManager.instance.lastPlayerLocation) == true && atAlertLocation == false)
            {
                // Move towards the last player location
                MoveTowards(GameManager.instance.lastPlayerLocation);
            }
            // If the AI is within range of the last known location of the player...
            if (Vector3.SqrMagnitude(tf.position - GameManager.instance.lastPlayerLocation) < (aiData.waypointRange * aiData.waypointRange))
            {
                atAlertLocation = true; // AI is now at the location
                if (atAlertLocation == true)
                {
                    if (alertTime < 0) // After a delay...
                    {
                        alertTime = aiData.alertDuration; // Reset timer
                        isAlertActive = false; // AI is no longer alert, allows AI to return to patrol state
                        GameManager.instance.isAlerted = false; // Global alert is no longer active

                        /* Resets boolean that indicates whether AI is at the search location
                        Prevents bug when changing states before AI finished looking at location */
                        atAlertLocation = false;
                    }
                    else
                    {
                        alertTime -= Time.deltaTime; // Decrement time
                    }
                }
            }
        }
    }
    // Attack the player
    public void Attack()
    {
        // If AI is not facing the player
        if (FacingTarget(lastPlayerLocation) == false)
        {
            // Rotate towards player
            RotateTowards(lastPlayerLocation, aiData.rotationSpeed);
        }
        // Stop when aimed at player
        if (FacingTarget(lastPlayerLocation) == true)
        {
            TriggerAttack(); // Fire weapon
        }
    }

    // Pursue the player
    public void Pursue()
    {
        // If the AI is not facing the player
        if (FacingTarget(lastPlayerLocation) == false)
        {
            // Rotate towards player
            RotateTowards(lastPlayerLocation, aiData.rotationSpeed);
        }
        // Stop when facing player
        if (FacingTarget(lastPlayerLocation) == true)
        {
            // Move towards player location
            MoveTowards(lastPlayerLocation);
        }
    }

    // Search when player goes from in view to out of view. AI will navigate to the last location the player was seen
    public void Search()
    {
        // Check if the AI is still searching
        if (isSearching == true)
        {
            // If the AI is not facing the last known player location and is not currently at that location
            if (FacingTarget(lastPlayerLocation) == false && atSearchLocation == false)
            {
                // Rotate towards location
                RotateTowards(lastPlayerLocation, aiData.rotationSpeed);
            }
            // Stop when facing player and still not at the search location
            if (FacingTarget(lastPlayerLocation) == true && atSearchLocation == false)
            {
                // Move towards last player location
                MoveTowards(lastPlayerLocation);
            }
            // If the AI is within range of the last known location of the player...
            if (Vector3.SqrMagnitude(tf.position - lastPlayerLocation) < (aiData.waypointRange * aiData.waypointRange))
            {
                atSearchLocation = true; // Indicates the AI is now at the location
                if (atSearchLocation == true) // If the AI is at the location
                {
                    if (searchTime < 0) // After a delay...
                    {
                        searchTime = aiData.searchDuration; // Reset timer
                        isSearching = false; // AI is no longer searching, allows AI to return to patrol

                        /* Resets boolean that indicates whether AI is at the search location
                        Prevents bug when changing states before AI finished looking at location */
                        atSearchLocation = false;
                    }
                    else
                    {
                        searchTime -= Time.deltaTime; // Decrement time (timer)
                    }
                }
            }
        }
    }

    // Investigate when the player is heard. AI will rotate towards the sound origin (location where player intersects hearing collider)
    public void Investigate()
    {
        // Check if the AI is still investigating
        if (isInvestigating == true)
        {
            // If not facing the direction of the sound origin
            if (FacingTarget(lastSoundLocation) == false)
            {
                // Rotate towards sound origin
                RotateTowards(lastSoundLocation, aiData.rotationSpeed);
            }
            // If the AI is facing the sound origin...
            if (FacingTarget(lastSoundLocation) == true)
            {
                atInvestigateLocation = true; // Indicates the AI is currently facing the sound location
                if (atInvestigateLocation == true) // If the AI is looking at the Investigate location
                {
                    if (investigateTime < 0) // After a delay...
                    {
                        investigateTime = aiData.investigateDuration; // Reset timer
                        isInvestigating = false; // AI is no longer investigating, required for AI to return to patrol
                        /* Resets boolean that indicates the AI is at the investigation location 
                         Prevents bug when changing states before AI finished looking at location */
                        atInvestigateLocation = false;
                    }
                    else
                    {
                        investigateTime -= Time.deltaTime; // Decrement time (timer)
                    }
                }
            }
        }
    }

    // Idle State
    public void Idle()
    {
        // Do nothing.
    }

    // PATROL METHODS

    // Forward then reverse order waypoint movement
    public void PingPongPatrol()
    {
        if (currentWaypoint == enemyWaypoints.Count - 1)
        {
            isPatrolForward = false; // If at the end of the array, flip patrol
        }
        if (currentWaypoint == 0)
        {
            isPatrolForward = true; // If at the beginning of the array, patrol in order
        }
        if (isPatrolForward == true)
        {
            currentWaypoint++; // If the patrol is moving forward through array, increment
        }
        if (isPatrolForward == false)
        {
            currentWaypoint--; // If the patrol is moving backward through array, decrement
        }
    }
    // Move to a random waypoint
    public void RandomPatrol()
    {
        currentWaypoint = Random.Range(0, enemyWaypoints.Count); // Get a random waypoint (can be same as the last one)
    }
    // Loop through waypoints
    public void LoopPatrol()
    {
        if (currentWaypoint < enemyWaypoints.Count - 1) // Loop through waypoints in order
        {
            currentWaypoint++;
        }
        else // When at the end of the array, restart from index 0
        {
            currentWaypoint = 0;
        }
    }
    // Stop at last waypoint
    public void StopPatrol()
    {
        if (currentWaypoint < enemyWaypoints.Count - 1)
        {
            currentWaypoint++;
        }
    }
}
