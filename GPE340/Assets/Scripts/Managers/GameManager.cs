using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// GameManager script is a singleton
public class GameManager : MonoBehaviour
{
    public static GameManager instance; // Static instance of the GameManager for Singleton Pattern
    public bool isCameraMouseControlled = false; // Bool to indicate if the camera is being controlled by the mouse, false means the camera will trail the player
    public bool isPlayerMouseControlled = true; // Bool to indicate if the player is controlled by the mouse, false means the player is not controlled by the mouse, but does not mean the mouse controls the camera
    public bool isMovementWorldSpace = true; // Toggles world and self space with regards to player controls
    public bool isMoveTowards = true; // Toggles between MoveTowards() method and Mathf.Lerp for the camera while trailing the player
    public float maxSpeed = 5; // Max movement speed
    public float rotationSpeed = 100; // Rotation speed
    public float camTrailSpeed = 0.5f; // Speed of camera while in trailing mode
    public float cameraTrailDistance = 10.0f; // Distance the camera will follow while in trailing mode
    public float camZoomDistance = 5.0f; // Camera zoom distance while mouse controls camera, this setting is controlled with the mouse wheel by default
    public float camZoomSpeed = 4.0f; // The speed that the camera zooms in and out while in mouse control mode
    public float minZoomDistance = 1.5f; // Lower bound clamp for the zoom distance to prevent clipping inside pawn
    public float maxZoomDistance = 10.0f; // Upper bound clamp for the zoom distance to prevent camera from spinning uncontrollably

    private void Awake()
    {
        // Singleton
        if (instance == null) // If the instance is null...
        {
            instance = this; // This instance is set to the static instance
            DontDestroyOnLoad(gameObject); // Do not destroy GameManager on load
        }
        else // If the instance is not null...
        {
            Destroy(gameObject); // Destroy the gameObject
        }
    }
}
