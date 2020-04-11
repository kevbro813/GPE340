using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu (menuName = "Data/PlayerData")] // Allows a PlayerData scriptable object instance to be created from within the inspector menu
public class PlayerData : Base_Data
{
    [Header("Crit Statistics")]
    [SerializeField] private float defaultCritChance; // Default critChance variable
    [SerializeField] private float defaultCritDamage; // Default critDamage variable

    /// <summary>
    /// Get PlayerData defaultCritChance property
    /// </summary>
    public float GetDefaultCritChance()
    {
        return defaultCritChance;
    }
    /// <summary>
    /// Get PlayerData defaultCritDamage property
    /// </summary>
    public float GetDefaultCritDamage()
    {
        return defaultCritDamage;
    }
}
