using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// AIPawn has methods run by AI enemies
public class AIPawn : Base_Pawn
{
    public AIData aiData; // AIData set in inspector

    // Override and use the Awake method
    public override void Awake() 
    {
        base.Awake();
    }
    // Override the UnequipWeapon method
    public override void UnequipWeapon()
    {
        base.UnequipWeapon(); // Use the base method
        equippedWeapon.GetComponent<Weapon>().isPlayerWeapon = false; // Also set isPlayerWeapon to false (since this is an AI)
    }
    // Override the EquipWeapon method
    public override void EquipWeapon(GameObject weap) // Pass in the weapon that is being equipped
    {
        base.EquipWeapon(weap); // Use the base method
        if (isWeaponEquipped) // Check if there is a weapon equipped
        {
            equippedWeapon.GetComponent<Weapon>().isPlayerWeapon = false; // Also set isPlayerWeapon to false (since this is an AI)
        }
    }
}
