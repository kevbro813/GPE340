using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

[RequireComponent(typeof(AudioSource))]
public class SoundFXManager: MonoBehaviour
{
    private AudioSource audioSource;
    public SelectedFX soundFX; // Enum that allows for easy switching between sound effects. This enum must match the order of audio clips stored in the SoundFX array (in GameManager)
    public enum SelectedFX { bodyHitOne, bodyHitTwo, bodyHitThree, bodyHitFour, maleDeath, pickup, pistolShot, rifleShot, shotgunShot, ricochet, footsteps };

    private AudioClip defaultSoundFX; // Default Audio Clip
    private Transform tf; // Transform component

    private void Awake()
    {
        SetDefaultFXClip(soundFX); // Set the default audio clip
        audioSource = GetComponent<AudioSource>(); // Set the audio source component
        tf = GetComponent<Transform>(); // Set the transform component
    }
    // Method sets the default FX clip
    public void SetDefaultFXClip(SelectedFX selection)
    {
        soundFX = selection; // Set the soundFX enum to the selection
        defaultSoundFX = GameManager.instance.soundFX.soundFXSources[(int)soundFX]; // Set the defaultSoundFX to the corresponding sound file
    }
    // Play the default audio clip (Useful when there is only one clip)
    public void PlayDefaultFXClip()
    {
        audioSource.PlayOneShot(defaultSoundFX);
    }
    // Play a selected FX Clip One Shot
    public void PlayFXClipOneShot(SelectedFX selection)
    {
        audioSource.PlayOneShot(GameManager.instance.soundFX.soundFXSources[(int)selection]); // PlayOneShot the selected soundFX
    }
    // Play a selected FX Clip At Point (Useful when destroying game object that emits sound, it creates a temporary audio source)
    public void PlayFXClipAtPoint(SelectedFX selection)
    {
        AudioSource.PlayClipAtPoint(GameManager.instance.soundFX.soundFXSources[(int)selection], tf.position); // PlayClipAtPoint the selected SoundFX
    }
}
