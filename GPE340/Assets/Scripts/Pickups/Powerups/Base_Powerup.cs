using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Base Powerup class
public abstract class Base_Powerup
{
    [SerializeField] private float duration;  // Duration indicates how long a powerup will last after it is picked up
    [SerializeField] private bool isPermanent; // isPermanent indicates if the powerup lasts permanently (all powerups are currently permanent)

    // Getter method for duration
    public float GetDuration()
    {
        return duration;
    }
    // Setter method for duration
    public void SetDuration(float d)
    {
        duration = d;
    }
    // Getter method for isPermanent
    public bool GetIsPermanent()
    {
        return isPermanent;
    }
    // Setter method for isPermanent
    public void SetIsPermanent(bool ip)
    {
        isPermanent = ip;
    }
    // Abstract method: Activate the powerup (overload for PlayerPawn parameter)
    public abstract void ActivatePowerup(GameObject obj, PlayerPawn player);
    // Abstract method: Activate the powerup (overload for AIPAwn parameter)
    public abstract void ActivatePowerup(GameObject obj, AIPawn ai); 

    // TODO: Powerup timer method
}
