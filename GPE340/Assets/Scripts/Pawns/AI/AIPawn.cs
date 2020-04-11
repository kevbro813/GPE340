using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// AIPawn has methods run by AI enemies
public class AIPawn : Base_Pawn
{
    public AIData aiData; // AIData set in inspector
    public GameObject[] weaponSelection;

    // Override and use the Awake method
    public override void Awake() 
    {
        base.Awake();
        SetMaxHealth(aiData.GetDefaultMaxHealth()); // Set current health to max health
        SetCurrentHealth(GetMaxHealth());
    }

    private void Start()
    {
        DisableRagdoll();
        SetDefaultWeapon();
    }
    // Override the UnequipWeapon method
    public override void UnequipWeapon()
    { 
        base.UnequipWeapon(); // Use the base method
    }
    // Override the EquipWeapon method
    public override void EquipWeapon(GameObject weap) // Pass in the weapon that is being equipped
    {
        base.EquipWeapon(weap); // Use the base method
        equippedWeapon.GetComponent<Weapon>().SetIsPlayerWeapon(false); // Also set isPlayerWeapon to false (since this is an AI)
    }

    public void SetDefaultWeapon()
    {
        GameObject weaponClone = Instantiate(weaponSelection[Random.Range(0, weaponSelection.Length)]) as GameObject;
        EquipWeapon(weaponClone);
    }
}
