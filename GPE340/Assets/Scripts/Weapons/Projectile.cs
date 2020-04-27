using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Component attached to a projectile that is fired
public class Projectile : MonoBehaviour
{
    public float damage; // Amount of damage to be dealt by the projectile
    public float duration; // Duration the projectile will be active
    private Transform tf; // Transform component
    public ParticleSystem hitModel; // Particle system for when a projectile hits a model (player or enemies)
    public ParticleSystem hitEnv; // Particle system for when a projectile hits the environment

    // Start is called before the first frame update
    void Start()
    {
        tf = GetComponent<Transform>(); // Get the transform component
    }
    // Method that runs when the projectile runs into a collider
    private void OnTriggerEnter(Collider col)
    {
        if (col.GetComponent<HealthSystem>()) // If the object the projectile collides with has a HealthSystem component...
        {
            col.GetComponent<HealthSystem>().DoDamage(damage); // Deal damage via DoDamage method, passing in damage amount
            ParticleSystem modelHit = Instantiate(hitModel, tf.position, tf.rotation) as ParticleSystem; // Instantiate the particle effect for hit model
            modelHit.Emit(1); // Emit one particle effect
            Destroy(modelHit.gameObject, 0.1f); // Destroy the bullet hit after it is used
        }
        else
        {
            GameObject ricochetSound = Instantiate(GameManager.instance.ricochetSound_Prefab, tf.position, tf.rotation) as GameObject; // Instantiate a ricochet sound prefab (needed to use effects output)
            ricochetSound.GetComponent<SoundFXManager>().PlayFXClipAtPoint(SoundFXManager.SelectedFX.ricochet); // Play ricochet sound effects using clip at point to play even after projectile is destroyed
            Destroy(ricochetSound); // Destroy the ricochetSound after it is used

            ParticleSystem environmentHit = Instantiate(hitEnv, tf.position, tf.rotation) as ParticleSystem; // Instantiate the particle effect for hit environment
            environmentHit.Emit(1); // Emit one particle effect
            Destroy(environmentHit.gameObject, 0.1f); // Destroy the bullet hit particle effect after it is used
        }
        Destroy(this.gameObject); // Destroy the gameObject if the projectile collides with any other collider
    }
}
