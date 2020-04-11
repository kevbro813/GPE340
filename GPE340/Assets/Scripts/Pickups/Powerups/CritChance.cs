using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable] // TODO: Class properties will not show up in inspector since child classes are not 
public class CritChance : Base_Powerup // CritChance is a child of Base_Powerup
{
    [SerializeField] private float critChanceAmount = 0.1f; // Crit Chance upgrade amount

    // Override the ActivatePowerup method (PlayerPawn overload)
    public override void ActivatePowerup(GameObject obj, PlayerPawn player)
    {
        player.SetCritChance(player.GetCritChance() + critChanceAmount); // Set the CritChance to the upgraded amount
    }

    // Override the ActivatePowerup method (AIPawn overload)
    public override void ActivatePowerup(GameObject obj, AIPawn ai)
    {
        // TODO: Do nothing for now, critChance powerup does not affect AI
    }
}
