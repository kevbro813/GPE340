using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Damage : Base_Powerup // Damage is a child of Base_Powerup
{
    [SerializeField] private float damageAmount = 50.0f; // Amount of damage to be dealt by damage powerup

    // Override the ActivatePowerup method (PlayerPawn overload)
    public override void ActivatePowerup(GameObject obj, PlayerPawn player)
    {
        // Deal damage to the pawn
        obj.GetComponent<HealthSystem>().DoDamage(damageAmount);
    }
    // Override the ActivatePowerup method (AIPawn overload)
    public override void ActivatePowerup(GameObject obj, AIPawn ai)
    {
        // Do nothing for now, damage powerup does not affect AI
    }
}
