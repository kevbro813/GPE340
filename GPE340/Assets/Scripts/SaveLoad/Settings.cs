using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable] // Important to serialize the data so it can be saved
public class Settings
{
    public bool isCursorVisible; // Is cursor locked and invisible
    public bool isCameraMouseControlled; // Bool to indicate if the camera is being controlled by the mouse, false means the camera will trail the player
    public bool isPlayerMouseControlled; // Bool to indicate if the player is controlled by the mouse, false means the player is not controlled by the mouse, but does not mean the mouse controls the camera
    public bool isMovementWorldSpace; // Toggles world and self space with regards to player controls
    public float masterVolume; // Master volume level
    public float effectsVolume; // Effects volume level
    public float musicVolume; // Music volume level
    public int resolution; // Index used to select resolution
    public int screenMode; // Index used to select screen mode
    public int videoQuality; // Index used to select video quality
}
