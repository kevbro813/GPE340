using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(Animator))]
public class AIController : Base_Controller
{
    private AIPawn aiPawn; // AIPawn component
    private AIVision aiVision; // AIVision component
    public AIState aiState; // AIState enum will indicate the current state
    public AIState tempState; // TempState will save the previous state when transitioning to obstacle avoidance. Allows the AI to return to the previous state once the obstacle is avoided
    public enum AIState { Idle, Patrol, Attack, Search, Pursue, Flee, Investigate, Avoidance, Alerted } // AIState selections

    void Awake()
    {
        aiPawn = GetComponent<AIPawn>(); // Set AIPawn component
        aiVision = GetComponentInChildren<AIVision>(); // Set AIVision component
    }
    // Update is called once per frame
    private void Update()
    {
        // IDLE STATE
        if (aiState == AIState.Idle)
        {
            DoIdle(); // Run Idle state (Do nothing)
            ChangeState(AIState.Patrol); // Transition immediately to Patrol (For testing)
        }

        // PATROL STATE
        if (aiState == AIState.Patrol)
        {
            DoPatrol(); // Run patrol state (Patrols between waypoints, has multiple patrol types)

            // Check if conditions are met to transition to another state
            TransitionPursue(); // If the target is seen but not in attack range
            TransitionInvestigate(); // If the target is heard
            TransitionAttack(); // If the target is seen and in firing range
        }
        // ATTACK STATE
        if (aiState == AIState.Attack)
        {
            DoAttack(); // Run attack state (Attacks when target is seen and within range of the equipped weapon)

            // Check if conditions are met to transition to another state
            TransitionPursue(); // If the target is no longer in firing range
            TransitionSearch(); // If the target is no longer seen, go to last known location
        }
        // SEARCH STATE
        if (aiState == AIState.Search)
        {
            DoSearch(); // Run Search State (Search the last known visible target location)

            // Check if conditions are met to transition to another state
            if (!aiPawn.isSearching) // If isSearching is false (this will become false when the search duration expires)
            {
                TransitionPatrol(); // Return to patrol state
            }
            TransitionPursue(); // If the target is currently seen 
            TransitionInvestigate(); // If the target is heard
            TransitionAttack(); // If the target is seen and within range
        }
        // PURSUE STATE
        if (aiState == AIState.Pursue)
        {
            DoPursue(); // Run Pursue State (Chase after the target if seen but not in attack range)

            // Check if conditions are met to transition to another state
            TransitionAttack(); // If the target is within range and seen
            TransitionSearch(); // If the target is no longer seen, go to last known visible location
        }
        // INVESTIGATE STATE
        if (aiState == AIState.Investigate)
        {
            DoInvestigate(); // Run Investigate State (Turns towards the location of a sound from target - AKA where the target intersects the hearing collider)

            // Check if conditions are met to transition to another state
            if (!aiPawn.isInvestigating) // If isInvestigating state is false (this will become false when the investigate duration timer expires)
            {
                TransitionPatrol(); // Return to patrol state
            }
            TransitionPursue(); // If the target is seen but not within attack range
            TransitionAttack(); // if the target is seen and within attack range
        }

        // FLEE STATE
        if (aiState == AIState.Flee)
        {
            DoFlee(); // Flee, run away from target
        }
        // OBSTACLE AVOIDANCE STATE
        if (aiState == AIState.Avoidance)
        {
            DoObstacleAvoidance(); // Avoid obstacle by rotating a random amount

            /* The following will rotate the AI a random amount while avoiding obstacles, this is done by having a random duration during which the AI will rotate.
            This makes the AI have less predictable movement and creates unique patrol routes*/
            if (aiPawn.randomRotation <= 0)
            {
                aiState = tempState; // Return AI to the previous state
                // Create a new random rotation for the next time the AI enters the Avoidance State
                aiPawn.randomRotation = Random.Range(aiPawn.aiData.rotationLow, aiPawn.aiData.rotationHigh);
            }
            else
            {
                aiPawn.randomRotation -= Time.deltaTime; // Decrement time
            }
        }
        if (aiState != AIState.Avoidance) // Transition to avoidance state can occur in any state but the avoidance state
        {
            TransitionAvoidance(); // Obstacle avoidance state
        }
        // ALERTED STATE
        if (aiState == AIState.Alerted)
        {
            DoAlerted(); // Run alerted state, go towards location of target known globally

            // Check if conditions are met to transition to another state
            TransitionPatrol(); // If the target is no longer seen or heard
            TransitionPursue(); // If the target is seen but not within attack range
            TransitionInvestigate(); // If the target is heard
            TransitionAttack(); // If the target is seen and within attack range
        }

        // Alerted and Flee states can be transitioned to from any state
        TransitionAlerted(); // Alerted state will mark the target location globally, and send all eligible AI to that location
        TransitionFlee(); // Flee state will activate when the AI's current health is below critical health. Forces the AI to run away
    }


    // CALLS ENEMY PAWN FUNCTIONS
    public void DoIdle()
    {
        //Debug.Log("Idle");
        aiPawn.Idle(); // Run idle method
    }
    public void DoPursue()
    {
        //Debug.Log("Pursue");
        aiPawn.Pursue(); // Run Pursue method
    }
    public void DoAttack()
    {
        //Debug.Log("Attack");
        aiPawn.Attack(); // Run attack method
    }
    public void DoPatrol()
    {
        //Debug.Log("Patrol");
        aiPawn.Patrol(); // Run patrol method
    }
    public void DoFlee()
    {
        //Debug.Log("Flee");
        aiPawn.Flee(); // Run Flee method
    }
    public void DoSearch()
    {
        //Debug.Log("Search");
        aiPawn.Search(); // Run Search method
    }
    public void DoInvestigate()
    {
        //Debug.Log("Investigate");
        aiPawn.Investigate(); // Run Investigate method
    }
    public void DoObstacleAvoidance()
    {
        //Debug.Log("Obstacle Avoidance");
        aiPawn.ObstacleAvoidance(aiPawn.obstacleHit); // Run Obstacle Avoidance
    }
    public void DoAlerted()
    {
        //Debug.Log("Alerted");
        aiPawn.Alerted(); // Run Alerted method
    }

    // TRANSITION FUNCTIONS (Transitions AI to the various states)

    // Transition to Obstacle Avoidance State
    public void TransitionAvoidance()
    {
        // If an obstacle is detected...
        if (aiPawn.ObstacleCheck() == true) // If an obstacle is detected...
        {
            aiPawn.atWaypoint = false; // Not at waypoint while running obstacle avoidance
            tempState = aiState; // Save the current state
            ChangeState(AIState.Avoidance); // Change to Avoidance state
        }
    }
    // Transition to Patrol State
    public void TransitionPatrol()
    {
        // If the player is not seen or heard...
        if (aiVision.CanSee(GameManager.instance.activePlayers) == false && aiPawn.canHear == false)
        {
            GameManager.instance.isAlerted = false; // Reset Static isAlerted variable to false
            // Reset boolean variables in case they are still true after a state change
            aiPawn.isInvestigating = false;
            aiPawn.isSearching = false;
            aiPawn.atWaypoint = false;
            aiPawn.atAlertLocation = false;
            aiPawn.atSearchLocation = false;
            ChangeState(AIState.Patrol); // Change to Patrol state
        }
    }
    // Transition to Attack State
    public void TransitionAttack()
    {
        // If the player is seen...
        if (aiVision.CanSee(GameManager.instance.activePlayers) == true)
        {
            // Set alert variables to make AI stop when they see the player, rather than continuing to run into the player
            aiPawn.atAlertLocation = true;
            aiPawn.isAlertActive = false; // Not in active state anymore since the player is in sight
            // If the player is in attack range...
            if (aiVision.AttackRange(aiVision.targetDistance) == true)
            {
                // Reset boolean variables in case they are still true after a state change
                aiPawn.isInvestigating = false;
                aiPawn.isSearching = false;
                aiPawn.atWaypoint = false;
                aiPawn.atAlertLocation = false;
                aiPawn.atSearchLocation = false;
                ChangeState(AIState.Attack); // Change to Attack State
            }
        }
    }
    // Transition to Flee State
    public void TransitionFlee()
    {
        // Flee if health is below critical level
        if (aiPawn.GetCurrentHealth() <= aiPawn.aiData.criticalHealth && aiPawn.isFleeing == false)
        {
            aiPawn.isFleeing = true; // Indicates the AI is currently fleeing
            // Reset boolean variables in case they are still true after a state change
            aiPawn.isInvestigating = false;
            aiPawn.isSearching = false;
            aiPawn.atWaypoint = false;
            aiPawn.isAlertActive = false;
            aiPawn.atAlertLocation = false;
            aiPawn.atSearchLocation = false;
            ChangeState(AIState.Flee); // Change to Flee State
        }
    }
    // Transition to Search State
    public void TransitionSearch()
    {
        // If the player is no longer seen...
        if (aiVision.CanSee(GameManager.instance.activePlayers) == false)
        {
            // isSearching must be true to run Search method, set once during transition. Set to false when the AI search duration elapses.
            aiPawn.isSearching = true;
            // Reset boolean variables in case they are still true after a state change
            aiPawn.isInvestigating = false;
            aiPawn.isAlertActive = false;
            aiPawn.atAlertLocation = false;
            aiPawn.atWaypoint = false;
            aiPawn.atSearchLocation = false;
            ChangeState(AIState.Search); // Change to Search State
        }
    }
    // Transition to Investigate State
    public void TransitionInvestigate()
    {
        // If the player is not seen, but heard...
        if (aiVision.CanSee(GameManager.instance.activePlayers) == false && aiPawn.canHear == true)
        {
            // isInvestigating must be true to run Investigate function, set once during transition. Set to false when the AI Investigate duration elapses.
            aiPawn.isInvestigating = true;
            // Reset boolean variables in case they are still true after a state change
            aiPawn.isSearching = false;
            aiPawn.atWaypoint = false;
            aiPawn.atInvestigateLocation = false;
            aiPawn.isAlertActive = false;
            aiPawn.atAlertLocation = false;
            aiPawn.atSearchLocation = false;
            ChangeState(AIState.Investigate); // Change to Investigate State
        }
    }
    // Transition to Pursue State
    public void TransitionPursue()
    {
        // If the player is seen but not in attack range...
        if (aiVision.CanSee(GameManager.instance.activePlayers) == true && aiVision.AttackRange(aiVision.targetDistance) == false)
        {
            // Reset boolean variables in case they are still true after a state change
            aiPawn.isInvestigating = false;
            aiPawn.isSearching = false;
            aiPawn.atWaypoint = false;
            aiPawn.isAlertActive = false;
            aiPawn.atAlertLocation = false;
            aiPawn.atSearchLocation = false;
            ChangeState(AIState.Pursue); // Change to Pursue State
        }
    }
    //Transition to Idle State
    public void TransitionIdle()
    {
        // Do nothing for now
        aiPawn.Idle();
        // Reset boolean variables in case they are still true after a state change
        aiPawn.isInvestigating = false;
        aiPawn.isSearching = false;
        aiPawn.atWaypoint = false;
        aiPawn.isAlertActive = false;
        aiPawn.atAlertLocation = false;
        aiPawn.atSearchLocation = false;
    }
    public void TransitionAlerted()
    {
        // If the player is alerted...
        if (GameManager.instance.isAlerted == true)
        {
            // isAlertActive must be true to run Alerted function, set once during transition. Set to false once at atAlertLocation is true.
            aiPawn.isAlertActive = true;
            aiPawn.atAlertLocation = false;

            tempState = aiState; // Save the current state to tempState, used to transition back to previous state after alert is over

            // Reset boolean variables in case they are still true after a state change
            aiPawn.isInvestigating = false;
            aiPawn.isSearching = false;
            aiPawn.atWaypoint = false;
            aiPawn.atSearchLocation = false;
            ChangeState(AIState.Alerted); // Change to Alerted State
        }
        if (GameManager.instance.isAlerted == false && aiState == AIState.Alerted) // If no longer alerted while in alerted state
        {
            aiPawn.isAlertActive = false;
            aiPawn.isInvestigating = false;
            aiPawn.isSearching = false;
            aiPawn.atWaypoint = false;
            aiPawn.atSearchLocation = false;
            ChangeState(AIState.Patrol); // Return AI to patrol state
        }
    }
    // Changes the AIState to a new state
    public void ChangeState(AIState newState)
    {
        aiState = newState; // Set aiState to the new state
    }
}
