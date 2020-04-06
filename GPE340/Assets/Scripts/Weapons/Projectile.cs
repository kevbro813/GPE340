using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Component attached to a projectile that is fired
public class Projectile : MonoBehaviour
{
    public float damage; // Amount of damage to be dealt by the projectile
    public float duration; // Duration the projectile will be active
    private Rigidbody rb; // Rigidbody component of the projectile (used to add force)

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>(); // Set Rigidbody component when object is created
    }
    // Method that runs when the projectile runs into a collider
    private void OnTriggerEnter(Collider col)
    {
        if (col.GetComponent<HealthSystem>()) // If the object the projectile collides with has a HealthSystem component...
        {
            col.GetComponent<HealthSystem>().DoDamage(damage); // Deal damage via DoDamage method, passing in damage amount
        }
        Destroy(this.gameObject); // Destroy the gameObject if the projectile collides with any other collider
    }
}
