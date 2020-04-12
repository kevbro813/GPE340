using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rifle : ProjectileWeapon
{
    [SerializeField] private bool isFullAuto; // Indicates if the weapon is full auto

    // TODO: Full auto, single, burst modes

    // Getter method for isFullAuto
    public bool GetIsFullAuto()
    {
        return isFullAuto;
    }

    // Setter method for isFullAuto
    public void SetIsFullAuto(bool full)
    {
        isFullAuto = full;
    }

    // UseWeapon override method will add some recoil (bullet spread) to the rifle shots
    public override void UseWeapon()
    {
        base.UseWeapon(); // Base weapon method determines if weapon is loaded
        
        if (roundChambered == true) // If the weapon is ready to fire...
        {
            roundChambered = false; // Indicate the weapon is no longer ready to fire

            // Create a projectile instance (adding recoil by adjusting the rotation of the projectiles slightly using "Quaternion.Euler(Random.onUnitSphere * GetRecoil())"
            GameObject projectileClone = Instantiate(projectile, barrel.position, barrel.rotation * Quaternion.Euler(Random.onUnitSphere * GetRecoil())) as GameObject;

            // Add force outward from the barrel
            projectileClone.GetComponent<Rigidbody>().AddRelativeForce(Vector3.forward * 10, ForceMode.VelocityChange);

            // Calculate damage (base or crit)
            projectileClone.GetComponent<Projectile>().damage = CalculateDamage();

            // Destroy the projectileClone after a set duration
            Destroy(projectileClone, projectileClone.GetComponent<Projectile>().duration);

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
