using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Base_Data class is a scriptable object that contains health properties and get/set methods
public class Base_Data : ScriptableObject
{
    [SerializeField] private float defaultMaxHealth; // Maximum health that the object can have

    /// <summary>
    /// Getter method for maxHealth
    /// </summary>
    public float GetDefaultMaxHealth()
    {
        return defaultMaxHealth;
    }
    /// <summary>
    /// Setter method for maxHealth
    /// </summary>
    private void SetDefaultMaxHealth(float dmh)
    {
        defaultMaxHealth = dmh;
    }
}

