using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Base_Data class is a scriptable object that contains health properties and get/set methods
public class Base_Data : ScriptableObject
{
    [SerializeField] private float currentHealth; // Variable to contain the object's current health
    [SerializeField] private float initialHealth; // Initial health value to reset current health to when game resets or object respawned
    [SerializeField] private float maxHealth; // Maximum health that the object can have

    /// <summary>
    /// Getter method for currentHealth
    /// </summary>
    public float GetCurrentHealth()
    {
        return currentHealth;
    }
    /// <summary>
    /// Sets currentHealth to a float value.
    /// </summary>
    public void SetCurrentHealth(float h)
    {
        currentHealth = h;
    }
    /// <summary>
    /// Getter method for maxHealth
    /// </summary>
    public float GetMaxHealth()
    {
        return maxHealth;
    }
    /// <summary>
    /// Setter method for maxHealth
    /// </summary>
    private void SetMaxHealth(float mh)
    {
        maxHealth = mh;
    }
    /// <summary>
    /// Getter method for initialHealth
    /// </summary>
    public float GetInitialHealth()
    {
        return initialHealth;
    }
    /// <summary>
    /// Setter method for initialHealth
    /// </summary>
    private void SetInitialHealth(float ih)
    {
        initialHealth = ih;
    }
    /// <summary>
    /// Method used to calculate the health percentage (used for health bar)
    /// </summary>
    public float GetCurrentHealthPercentage()
    {
        return currentHealth / maxHealth;
    }
}

