using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ItemDrops
{
    [SerializeField] private Object value; // Value represents the object that will be dropped
    [SerializeField] private double chance = 1; // The weight this object will drop

    // Get the chance the item will drop
    public double GetChance()
    {
        return chance;
    }
    // Get the object
    public Object GetValue()
    {
        return value;
    }
}
