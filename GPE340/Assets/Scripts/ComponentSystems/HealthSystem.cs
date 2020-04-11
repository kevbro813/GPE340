﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

// Health System is a component based system that gives objects health and the ability to take damage, heal and die
[System.Serializable]
public class HealthSystem : MonoBehaviour
{
    [Header("Events")] // Events header
    [SerializeField] private UnityEvent onHeal; // Event that will run when the object heals
    [SerializeField] private UnityEvent onDamage; // Event that will run when the object is damaged
    [SerializeField] private UnityEvent onDeath; // Event that will run when the object dies
    private Base_Pawn bp; // This is the parent data class that has the health properties and methods
    private bool isPlayer; // Indicates if the current object is the player (required to update the player HUD)
    private bool isDead = false;
    // Start is called before the first frame update
    void Start()
    {
        if (GetComponent<PlayerPawn>()) // If there is a PlayerPawn component, then the object is a player
        {
            bp = GetComponent<PlayerPawn>(); // Set the Base_Data component to the PlayerData
            isPlayer = true; // Indicate this is the player
        }
        else if (GetComponent<AIPawn>()) // If there is an AIPawn component, then the object is an AI (Enemy)
        {
            bp = GetComponent<AIPawn>(); // Set the Base_Data component to AIData
            isPlayer = false; // This is not the player
        }
    }

    // Method that deals damage to the object
    public void DoDamage(float dmgAmt) // Takes in a float value that represents the damage amount
    {
        if ((bp.GetCurrentHealth() - dmgAmt) > 0) // If the player still has health (health above 0) after taking damage
        {
            bp.SetCurrentHealth(bp.GetCurrentHealth() - dmgAmt); // Set current health by subtracting damage amount

            CallUpdateHealth(); // Update the player HUD health bar

            onDamage.Invoke(); // TODO: Animation "Taking Damage"
        }
        else // If the player will run out of health when damaged...
        {
            bp.SetCurrentHealth(0.0f); // Set health to 0
            DoDeath(); // Run the DoDeath method
            isDead = true;
        }
    }
    // Method that heals an object
    public void DoHealing(float healAmt) // Healing amount passed as a parameter
    {
        if ((bp.GetCurrentHealth() + healAmt) <= bp.GetMaxHealth()) // Check if healing amount is less than or equal to max health amount
        {
            bp.SetCurrentHealth(bp.GetCurrentHealth() + healAmt); // Heal the object for the heal amount

            CallUpdateHealth(); // Update the player's HUD

            onHeal.Invoke(); // TODO: Animation "Healing"
        }
        else if ((bp.GetCurrentHealth() + healAmt) > bp.GetMaxHealth()) // If the healing amount is greater than max health
        {
            bp.SetCurrentHealth(bp.GetMaxHealth()); // Set current health to max health

            CallUpdateHealth(); // Update the player HUD

            onHeal.Invoke(); // TODO: Animation "Healing"
        }
    }
    // Method that runs when an object dies (has 0 health)
    public void DoDeath()
    {
        if (!isDead)
        {
            if (isPlayer) // If the object is the player
            {
                bp.UnequipWeapon();
                GameManager.instance.activePlayers.Remove(this.gameObject);
                GameManager.instance.InstantiatePlayer(); // Instantiate a new player
                Destroy(this.gameObject); // Destroy the current gameObject
            }
            else // If not the player (then the object is the enemy)
            {
                bp.UnequipWeapon();
                GameManager.instance.currentEnemies--; // Update enemy count
                GameManager.instance.activeEnemies.Remove(this.gameObject);
                GameManager.instance.InstantiateEnemies(); // Instantiate enemies
                Destroy(this.gameObject, 3.0f); // Destroy the current gameObject
                bp.EnableRagdoll();
            }
            onDeath.Invoke(); // TODO: Animation "Death"


        }
    }
    // Method that will update the player's HUD with currentHealthPercentage
    public void CallUpdateHealth()
    {
        if (isPlayer) // Check that the object is the player
        {
            // Update the HUD according to the currentHealthPercentage (Percentage is required for the health bar)
            GameManager.instance.playerHUD.UpdateHealth(bp.GetCurrentHealthPercentage());
        }
    }
}
