using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Weapon : MonoBehaviour
{
    public float damage; // Damage value
    public bool isPlayerWeapon; // Indicates if the player controls the weapon
    [HideInInspector] public PlayerPawn player; // Component variable for the PlayerPawn

    [Header("Weapon IK Positions")]
    public Transform rightHandPosition; // Position on the gun where the right hand should be placed
    public Transform leftHandPosition; // Position on the gun where the left hand should be placed
    public Transform rightElbowPosition; // Position where the right elbow should be placed
    public Transform leftElbowPosition; // Position where the left elbow should be placed


    // Getter method for damage
    public float GetDamage()
    {
        return damage;
    }
    // Setter method for damage
    public void SetDamage(float dam)
    {
        damage = dam;
    }

    public abstract void UpdateWeaponStats(); // Update weapon stats abstract method
    public abstract void UseWeapon(); // Use weapon Abstract method
}
