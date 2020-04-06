using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Health : Base_Powerup // Health is a child of Base_Powerup
{
    [SerializeField] private float healingAmount = 50.0f; // Amount of health to be healed

    // Override the ActivatePowerup method (PlayerPawn overload)
    public override void ActivatePowerup(GameObject obj, PlayerPawn player)
    {
        // Increase pawn's health by the healingAmount
        obj.GetComponent<HealthSystem>().DoHealing(healingAmount);
    }
    // Override the ActivatePowerup method (AIPawn overload)
    public override void ActivatePowerup(GameObject obj, AIPawn ai)
    {
        // Do nothing for now, health powerup does not affect AI
    }
}
