using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pistol : ProjectileWeapon
{
    [SerializeField] private bool isSilenced; // Indicates if the pistol is silenced or unsilenced version

    // TODO: Method to switch silenced/unsilenced models

    // Getter method for isSilenced
    public bool GetIsSilenced()
    {
        return isSilenced;
    }
    // Setter method for isSilenced
    public void SetIsSilenced(bool sil)
    {
        isSilenced = sil;
    }

    // Override UseWeapon method uses only the base method
    public override void UseWeapon()
    {
        base.UseWeapon(); // Base weapon method determines if weapon is loaded
        // If the weapon is ready to fire...
        if (roundChambered == true)
        {
            roundChambered = false; // Indicate the weapon cannot fire again

            // Create a projectile instance
            GameObject projectileClone = Instantiate(projectile, barrel.position, barrel.rotation) as GameObject;

            // AddRelativeForce will launch the projectile forward
            projectileClone.GetComponent<Rigidbody>().AddRelativeForce(Vector3.forward * 10, ForceMode.VelocityChange);

            // Calculate the damage of the projectile (base or crit)
            projectileClone.GetComponent<Projectile>().damage = CalculateDamage();

            // Destroy the projectileClone after a set duration
            Destroy(projectileClone, projectileClone.GetComponent<Projectile>().duration);

            // Reset lastShotFired
            lastShotFired = Time.time;
        }
    }
    // Override UpdateWeaponStats method uses only the base method
    public override void UpdateWeaponStats()
    {
        base.UpdateWeaponStats();
    }
}
