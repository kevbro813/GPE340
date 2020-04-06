using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Base_Pawn : MonoBehaviour
{
    [Header("Required Components")] // Required components header displayed in inspector
    [HideInInspector] public Transform tf; // Transform component for the player gameObject
    [HideInInspector] public Animator anim; // Animator component for the player gameObject

    [Header("Weapon Settings")]
    public bool isWeaponEquipped; // Indicates if a weapon is equipped
    // TODO: Need to check if players starts with a weapon
    public GameObject equippedWeapon; // GameObject that holds the currently equipped weapon
    public Weapon weaponComponent; // Component of the currently equipped weapon
    private Transform weap_tf; // Transform component of the weapon
    public Transform weaponContainer; // Container on the pawn that will hold the weapon
    public Transform rightHandIK; // Right Hand IK position
    public Transform leftHandIK; // Left hand IK position
    public Transform rightElbowIK; // Right elbow IK Hint position
    public Transform leftElbowIK; // Left elbow IK Hint Position

    public virtual void Awake() // Virtual Awake method, will be overridden by children
    {
        tf = GetComponent<Transform>(); // Set the transform component
        anim = GetComponent<Animator>(); // Set the animator component
    }

    // Virtual method for EquipWeapon
    public virtual void EquipWeapon(GameObject weap) // Pass in the weapon gameobject that will be used
    {
        // The following two settings will prevent the weapon collider from interfering with the player/AI that is holding the weapon
        weap.GetComponent<BoxCollider>().enabled = false; // Disable the box collider
        weap.GetComponent<Rigidbody>().isKinematic = true; // Set to kinematic

        weap.layer = 8; // Set weapon layer to "Player"

        // If the weapon is a rifle or shotgun
        if (weap.GetComponent<Rifle>() || weap.GetComponent<Shotgun>())
        {
            anim.SetInteger("weaponType", 1); // Set the animation style to Rifle (also works for shotgun)
        }
        // If the weapon is a pistol
        else if (weap.GetComponent<Pistol>())
        {
            anim.SetInteger("weaponType", 2); // Set the animation style to pistol
        }
        // Set the weapon's transform component
        weap_tf = weap.GetComponent<Transform>();

        // Set the weapon's parent to the weaponContainer
        weap_tf.SetParent(weaponContainer);

        // Set the weapon's position and rotation to match the weaponContainer
        weap_tf.position = weaponContainer.position;
        weap_tf.rotation = weaponContainer.rotation;

        equippedWeapon = weap; // Set the equippedWeapon as the weapon that is passed to this method

        weaponComponent = weap.GetComponent<Weapon>(); // Set the weapon component

        // Identify left and right hand IK
        rightHandIK = weaponComponent.rightHandPosition;
        leftHandIK = weaponComponent.leftHandPosition;

        // Left/right elbow IK
        rightElbowIK = weaponComponent.rightElbowPosition;
        leftElbowIK = weaponComponent.leftElbowPosition;

        isWeaponEquipped = true; // Indicate the weapon is equipped
    }

    // Virtual method to unequip a weapon
    public virtual void UnequipWeapon()
    {
        if (isWeaponEquipped) // Check if a weapon is currently equipped
        {
            // Remove weapon from parent
            weaponContainer.DetachChildren();
            
            anim.SetInteger("weaponType", 0); // Set the weaponType to "None"

            // Set all of the IKGoal and IKHint positions to null
            rightHandIK = null;
            leftHandIK = null;
            rightElbowIK = null;
            leftElbowIK = null;

            isWeaponEquipped = false; // Indicate the weapon is not equipped
            equippedWeapon.GetComponent<BoxCollider>().enabled = true; // Reactivate the box collider
            equippedWeapon.GetComponent<Rigidbody>().isKinematic = false; // Set kinematic to false

            // The following code will toss the weapon ahead of the parent object to prevent it from immediately being picked up again
            equippedWeapon.GetComponent<Transform>().Translate(Vector3.forward, Space.Self);
            equippedWeapon.layer = 0; // Reset the weapon's layer to default
            equippedWeapon = null; // Make equippedWeapon null, prevents gun from being fired by player after it is unequipped
        }
    }

    // If the pawn collides with a weapon (using OnCollisionEnter since the weapon has a collider, not a trigger)
    public void OnCollisionEnter(Collision col)
    {
        if (col.gameObject.CompareTag("Weapon")) // Check if the pawn collides with a weapon
        {
            if (!isWeaponEquipped) // If a weapon is not equipped (Do not want multiple weapons equipped at same time)
            {
                GameObject weapon = col.gameObject; // Equip the weapon
                EquipWeapon(weapon); // Equip weapon, passing in the weapon
            }
        }
    }

    // OnAnimatorIK method will maintain the hand and elbow positions of the pawn while it holds a weapon
    public void OnAnimatorIK()
    {
        if (rightHandIK && leftHandIK) // If left and right hand IK transform variables exist
        {
            // Set right and left hand IK positions
            anim.SetIKPosition(AvatarIKGoal.RightHand, rightHandIK.position);
            anim.SetIKPosition(AvatarIKGoal.LeftHand, leftHandIK.position);

            // Set right and left hand position weights to 1 (activates them)
            anim.SetIKPositionWeight(AvatarIKGoal.RightHand, 1f);
            anim.SetIKPositionWeight(AvatarIKGoal.LeftHand, 1f);

            // Set right and left hand IK rotations
            anim.SetIKRotation(AvatarIKGoal.RightHand, rightHandIK.rotation);
            anim.SetIKRotation(AvatarIKGoal.LeftHand, leftHandIK.rotation);

            // Set right and left hand rotation weights to 1 (activates them)
            anim.SetIKRotationWeight(AvatarIKGoal.RightHand, 1f);
            anim.SetIKRotationWeight(AvatarIKGoal.LeftHand, 1f);

            // Set the right and left elbow positions
            anim.SetIKHintPosition(AvatarIKHint.RightElbow, rightElbowIK.position);
            anim.SetIKHintPosition(AvatarIKHint.LeftElbow, leftElbowIK.position);

            // Set right and left elbow position weights to 1 (activates them)
            anim.SetIKHintPositionWeight(AvatarIKHint.RightElbow, 1f);
            anim.SetIKHintPositionWeight(AvatarIKHint.LeftElbow, 1f);
        }
        else
        {
            // Set all hand and elbow weights to 0 which deactivates them and allows the default animation to be active
            anim.SetIKPositionWeight(AvatarIKGoal.RightHand, 0f); // Right hand position
            anim.SetIKRotationWeight(AvatarIKGoal.RightHand, 0f); // Right hand rotation

            anim.SetIKPositionWeight(AvatarIKGoal.LeftHand, 0f); // Left hand position
            anim.SetIKRotationWeight(AvatarIKGoal.LeftHand, 0f); // Left hand rotation

            anim.SetIKHintPositionWeight(AvatarIKHint.RightElbow, 0f); // Right elbow position
            anim.SetIKHintPositionWeight(AvatarIKHint.LeftElbow, 0f); // Left elbow position
        }
    }
    // Attack method will simply run the UseWeapon method from the ProjectileWeapon script attached to the currently equippedWeapon
    public void Attack()
    {
        if (isWeaponEquipped) // Check if a weapon is equipped
        {
            equippedWeapon.GetComponent<ProjectileWeapon>().UseWeapon(); // Call the UseWeapon method
        }
        else
        {
            // TODO: No weapon equipped/Possibly add melee weapons
        }
    }

}
