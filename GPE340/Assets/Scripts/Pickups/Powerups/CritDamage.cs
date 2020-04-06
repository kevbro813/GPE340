using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CritDamage : Base_Powerup // CritDamage is a child of Base_Powerup
{
    [SerializeField] private float critDamageAmount = 0.2f; // Crit Damage upgrade amount

    // Override the ActivatePowerup method (PlayerPawn overload)
    public override void ActivatePowerup(GameObject obj, PlayerPawn player)
    {
        player.pData.SetCritDamage(player.pData.GetCritDamage() + critDamageAmount); // Set the CritDamage to the upgraded amount
    }
    // Override the ActivatePowerup method (AIPawn overload)
    public override void ActivatePowerup(GameObject obj, AIPawn ai)
    {
        // Do nothing for now, critDamage powerup does not affect AI
    }
}
