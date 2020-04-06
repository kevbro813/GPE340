using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileWeapon : Weapon
{
    [SerializeField] private float recoil; // Amount of recoil (bullet spread in this game)
    public GameObject projectile; // Contains the projectile prefab to use
    public Transform barrel; // Indicates the barrel where the projectile is instantiated
    public float rateOfFire; // Indicates the rate of fire of the weapon
    public float lastShotFired; // Variable that stores the last time the cannon was fired
    public bool roundChambered = true; // Bool is true when the timer expires, allowing the cannon can be fired again
    private void Start()
    {
        lastShotFired = Time.time; // Set the lastShotFired to Time.time (used for rate of fire timer)
    }
    // Getter method for recoil
    public float GetRecoil()
    {
        return recoil;
    }
    // Setter method for recoil
    public void SetRecoil(float rec)
    {
        recoil = rec;
    }

    // Method that overrides the UseWeapon method in Weapon parent class
    public override void UseWeapon()
    {
        if (Time.time >= lastShotFired + rateOfFire) // Check if the weapon is ready to fire based on Time elapsed since the last shot was fired
        {
            roundChambered = true; // Indicate the weapon is ready to fire
        }
    }

    public override void UpdateWeaponStats()
    {
        // TODO: Method that will allow weapon stats to be updated
    }

    // Calculate the damage done (either base damage if not player or base/crit damage if the player fired the weapon) 
    public float CalculateDamage()
    {
        if (isPlayerWeapon) // Check if the player fired the weapon
        { 
            float critChance = player.pData.GetCritChance(); // Get Crit Chance value
            float rand = Random.Range(1.0f, 100.0f) / 100; // Random value between 0.01 and 1

            if (critChance >= rand) // Deal crit damage if crit is successful
            {
                float critDamagePercentage = player.pData.GetCritDamage(); // Get CritDamage value
                float critDamageAmount = damage * critDamagePercentage; // Set the damage amount to the base damage * critDamagePercentage
                Debug.Log("Crit Damage"); // TODO: This will temporarily indicate a crit hit in the console (Add indicator to canvas screen)
                return critDamageAmount; // Return crit damage
            }
            else
            {
                Debug.Log("Normal Damage"); // TODO: This will temporarily indicate a normal hit in the console (Add indicator to canvas screen)
                return damage; // Return base damage if crit fails
            }
        }
        return damage; // Return base damage if not the player
    }
}
