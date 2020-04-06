using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu (menuName = "Data/PlayerData")] // Allows a PlayerData scriptable object instance to be created from within the inspector menu
public class PlayerData : Base_Data
{
    [Header("Crit Statistics")]
    [SerializeField] private float critChance; // Crit chance is the percent chance a crit hit will land
    [SerializeField] private float critDamage; // Crit damage is the multiplier that will multiply base damage when a crit hit is successful
    [SerializeField] private float initialCritChance = 0.1f; // Initial value to reset critChance variable
    [SerializeField] private float initialCritDamage = 1.5f; // Initial value to reset critDamage variable

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
    /// <summary>
    /// Getter method for initialCritChance. 
    /// </summary>
    public float GetInitialCritChance()
    {
        return initialCritChance;
    }
    /// <summary>
    /// Getter method for initialCritDamage. 
    /// </summary>
    public float GetInitialCritDamage()
    {
        return initialCritDamage;
    }
}
