using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// PowerupObject component is attached to all powerups in the game, allows designers to choose the powerup type from a list
public class PowerupObject : MonoBehaviour
{
    public ActivePowerup activePowerup; // ActivePowerup 
    public enum ActivePowerup { Health, CritDamage, CritChance, Damage }; // Available powerups to choose from
    [HideInInspector] public Base_Powerup powerup; // Base_Powerup class variable
    [HideInInspector] public SphereCollider sphereCollider; // Sphere collider is toggled to allow the item to be picked up or not
    [HideInInspector] public MeshRenderer[] meshRenderer; // Mesh renderer is toggled to display/hide the object

    // TODO: Get powerup settings in inspector

    // Start is called before the first frame update
    void Start()
    {
        sphereCollider = GetComponent<SphereCollider>(); // Set the sphere collider variable
        meshRenderer = GetComponentsInChildren<MeshRenderer>(); // Set the mesh renderer variable

        if (activePowerup == ActivePowerup.Health) // If this is a health pickup...
        {
            powerup = new Health(); // Create a new Health class
            powerup.SetIsPermanent(true); // Health is a permanent powerup
        }
        if (activePowerup == ActivePowerup.CritDamage) // If this is a crit damage pickup...
        {
            powerup = new CritDamage(); // Create a new CritDamage class
            powerup.SetIsPermanent(true); // Health is a permanent powerup
        }
        if (activePowerup == ActivePowerup.CritChance) // If this is a crit chance pickup...
        { 
            powerup = new CritChance(); // Create a new CritChance class
            powerup.SetIsPermanent(true); // Health is a permanent powerup
            // TODO: Make crit chance temporary
        }
        if (activePowerup == ActivePowerup.Damage)// If this is a damage pickup...
        {
            powerup = new Damage(); // Create a new Damage class
            powerup.SetIsPermanent(true); // Damage is a permanent powerup
        }
    }
    // Trigger to determine when a powerup is picked up and by whom
    public void OnTriggerEnter(Collider col)
    {
        if (col.CompareTag("Player")) // If the player picks up the object
        {
            powerup.ActivatePowerup(col.gameObject, col.gameObject.GetComponent<PlayerPawn>()); // Activate powerup using the PlayerPawn overload method
            DeactivatePickup(); // Deactivate the pickup once it is picked up
        }
        else if (col.CompareTag("AI")) // If the AI picks up the object
        {
            powerup.ActivatePowerup(col.gameObject, col.gameObject.GetComponent<AIPawn>()); // Activate powerup using the AIPawn overload method
            DeactivatePickup(); // Deactivate the pickup once it is picked up
        }
    }
    // Coroutine to respawn the pickup after it is picked up
    private IEnumerator RespawnPickupEvent()
    {
        yield return new WaitForSeconds(GameManager.instance.pickupRespawnDelay); // Wait for pickupRespawnDelay to reactivate the powerup
        sphereCollider.enabled = true; // Activate the sphere collider so the object can be picked up again
        for (int i = 0; i < meshRenderer.Length; i++) // Reactivate all meshRenderer components to make the object visible again
        {
            meshRenderer[i].enabled = true; // Make meshRenderer visible
        }
    }
    // Deactivate the pickup after it is used
    private void DeactivatePickup()
    {
        sphereCollider.enabled = false; // Deactivate the sphere collider to prevent it from being picked up again
        for (int i = 0; i < meshRenderer.Length; i++) // Loop through all the MeshRenderer components
        {
            meshRenderer[i].enabled = false; // Disable all MeshRenderer components 
        }
        StartCoroutine("RespawnPickupEvent"); // Start pickup respawn coroutine to respawn pickup object after it is used
    }
}
