using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// This script requires a Transform and Animator Component as well as a PlayerData ScriptableObject
[RequireComponent(typeof(Transform))]
[RequireComponent(typeof(Animator))]
// PlayerPawn script has all the basic movement methods, besides horizontal and vertical movement.
public class PlayerPawn : Base_Pawn
{
    [Header("PlayerData ScriptableObject")] // Header displayed in the inspector
    // SerializedField displays the private variable in the inspector, tooltip adds a tooltip when the mouse hovers over the variable in the inspector
    [Tooltip("The PlayerData ScriptableObject contains all the player variables.")] public PlayerData pData;
    [SerializeField] private float critChance; // Crit chance is the percent chance a crit hit will land
    [SerializeField] private float critDamage; // Crit damage is the multiplier that will multiply base damage when a crit hit is successful

    /// <summary>
    /// Getter method for critChance. 
    /// </summary>
    public float GetCritChance()
    {
        return critChance;
    }
    /// <summary>
    /// Getter method for critDamage. 
    /// </summary>
    public float GetCritDamage()
    {
        return critDamage;
    }
    /// <summary>
    /// Setter method for critChance. 
    /// </summary>
    public void SetCritChance(float chance)
    {
        critChance = chance;
        GameManager.instance.playerHUD.UpdateCritChance(critChance); // Update the player HUD with the new crit chance value
    }
    /// <summary>
    /// Setter method for critDamage. 
    /// </summary>
    public void SetCritDamage(float crit)
    {
        critDamage = crit;
        GameManager.instance.playerHUD.UpdateCritDamage(critDamage); // Update the player HUD with the new crit damage value
    }

    // Override Awake method using only the base method
    public override void Awake()
    {
        base.Awake();
    }

    private void Start()
    {
        SetMaxHealth(pData.GetDefaultMaxHealth()); // Set max health to default max health
        SetCurrentHealth(GetMaxHealth()); // Set current health to max health
        SetCritChance(pData.GetDefaultCritChance()); // Set Crit chance to initial value
        SetCritDamage(pData.GetDefaultCritDamage()); // Set Crit damage to initial value

        GameManager.instance.playerHUD.UpdateHUD(GetCritChance(), GetCritDamage(), GetCurrentHealthPercentage()); // Update the player's health on HUD
    }
    /// <summary>
    /// This method will toggle the player's crouch. 
    /// </summary>
    public void Crouch(bool active) // Bool variable is passed to this method to determine if the player should crouch
    {
        if (active) // Check if crouch is active
        {
            anim.SetBool("isCrouching", true); // Set the animator "isCrouching" bool to true
        }
        else
        {
            anim.SetBool("isCrouching", false); // Set the animator "isCrouching" bool to false
        }
    }
    /// <summary>
    /// This method will toggle the player's run.
    /// </summary>
    public void Run(bool active) // Bool variable is passed to this method to determine if the player should run
    {
        if (active) // Check if run is active
        {
            anim.SetBool("isRunning", true); // Set the animator "isRunning" bool to true
        }
        else
        { 
            anim.SetBool("isRunning", false); // Set the animator "isRunning" bool to false
        }
    }
    /// <summary>
    /// This method will face the player in the direction of the mouse. The boolean variable isPlayerMouseControlled must be true and isCameraMouseControlled must be false.
    /// </summary>
    public void FaceMouseDir()
    {
        Plane plane = new Plane(Vector3.up, tf.position); // Create a new plane for the mouse position to use as a reference 
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition); // Create a ray from the the mouse position

        float enter; // Distance variable
        
        if (plane.Raycast(ray, out enter)) // If the ray from the mouse position intersects the plane...
        {
            Vector3 targetPoint = ray.GetPoint(enter); // Get the point the ray and plane intersect
            // Convert the Vector3 targetPoint to a Quaternion using the Quaternion.LookRotation method. This determines the rotation needed to turn the playerPawn in the direction of the targetPoint (mouse position)
            Quaternion targetRotation = Quaternion.LookRotation(targetPoint - tf.position); 
            // Change the rotation of the camera using the RotateTowards method. Parameters passed in are the origin, the direction and the rotation speed, respectively.                                                                                 
            tf.rotation = Quaternion.RotateTowards(tf.rotation, targetRotation, GameManager.instance.rotationSpeed);
        }
    }

    /// <summary>
    /// Override method of EquipWeapon, uses base method, but also indicates the pawn is the player and sets the weapon's PlayerPawn component.
    /// </summary>
    public override void EquipWeapon(GameObject weap)
    {
        base.EquipWeapon(weap); // Run the base EquipWeapon method
        equippedWeapon.GetComponent<Weapon>().SetIsPlayerWeapon(true); // Indicates the current pawn is the player
        weaponComponent.player = GetComponent<PlayerPawn>(); // Set player pawn component, needed to set hand and elbow IK positions/rotations

        // TODO: Allow weapon to be picked up by AI
    }

    /// <summary>
    /// Override method of UNequipWeapon, uses base method, but also indicates the weapon is no longer controlled by the player and sets the controlling player to null.
    /// </summary>
    public override void UnequipWeapon()
    {
        base.UnequipWeapon(); // Run the base UnequipWeapon method
        if (isWeaponEquipped) // Check if weapon is equipped
        {
            equippedWeapon.GetComponent<Weapon>().SetIsPlayerWeapon(false); // Indicate the pawn does not control the weapon
            weaponComponent.player = null; // Set the player pawn component back to null since pawn no longer controls weapon

            // TODO: Allow weapon to be dropped by AI
        }
    }
}
