using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shotgun : ProjectileWeapon
{
    [SerializeField] private float spread; // Spread amount is how wide the bullets will spread
    [SerializeField] private int shrapnelCount; // Number of shrapnel produced by a shotgun shell

    // Getter method for spread
    public float GetSpread()
    {
        return spread;
    }

    // Setter method for spread
    public void SetSpread(float spr)
    {
        spread = spr;
    }

    // Getter method for shrapnelCount
    public int GetShrapnelCount()
    {
        return shrapnelCount;
    }

    // Setter method for shrapnelCount
    public void SetShrapnelCount(int sct)
    {
        shrapnelCount = sct;
    }

    // Use Weapon override method will adjust fire type to shotgun spread
    public override void UseWeapon()
    {
        base.UseWeapon(); // Base weapon method determines if weapon is loaded
        
        if (roundChambered == true) // If the weapon is ready to fire...
        {
            roundChambered = false; // Indicate weapon cannot be fired again

            for (int i = 0; i < GetShrapnelCount(); i++) // Loop that will create shrapnel up to the shrapnelCount value
            {
                // Create a projectile instance (rotation is altered to add some spread to each of the shrapnel projectiles)
                GameObject projectileClone = Instantiate(projectile, barrel.position, barrel.rotation * Quaternion.Euler(Random.onUnitSphere * GetSpread())) as GameObject;

                // Add force outward from the barrel
                projectileClone.GetComponent<Rigidbody>().AddRelativeForce(Vector3.forward * 10, ForceMode.VelocityChange);

                // Calculate damage (base or crit)
                projectileClone.GetComponent<Projectile>().damage = CalculateDamage();

                // Destroy the projectileClone after a set duration
                Destroy(projectileClone, projectileClone.GetComponent<Projectile>().duration);
            }
            // Reset lastShotFired
            lastShotFired = Time.time;
        }
    }
    // Override method to update the weapon's stats
    public override void UpdateWeaponStats()
    {
        base.UpdateWeaponStats();
    }
}
