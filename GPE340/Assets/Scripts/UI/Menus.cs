using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class Menus : MonoBehaviour
{
    public Toggle cursorVisible_Toggle; // Toggle switch for cursor lock mode
    public Toggle cameraMouseControlled_Toggle; // Toggle switch for camera controlled by mouse
    public Toggle playerMouseControlled_Toggle; // Toggle switch for player controlled by mouse
    public Toggle worldSpace_Toggle; // Toggle switch for world/self space movement controls
    public Slider masterVolume_Slider;
    public Slider musicVolume_Slider;
    public Slider effectsVolume_Slider;
    public Dropdown resolution_Dropdown;
    public Dropdown screenMode_Dropdown;
    public Dropdown videoQuality_Dropdown;
    private List<string> resolutionList;
    private Resolution[] resolutions;
    public Button applyButton;
    private List<string> screenModes = new List<string> {"Fullscreen", "Windowed", "Fullscreen Borderless"};

    private void Awake()
    {
        resolutions = Screen.resolutions; // Get list of screen resolutions and save to array
        resolution_Dropdown.ClearOptions(); // Clear out existing dropdown options
        resolutionList = new List<string>(); // Create a list to hold resolutions
        for (int i = 0; i < Screen.resolutions.Length; i++) // Loop through each resolution
        {
            resolutionList.Add ($"{Screen.resolutions[i].width} x {Screen.resolutions[i].height}"); // Add the resolution to the resolution list
            resolution_Dropdown.value = i; // Set the dropdown value to the resolution index
        }
        resolution_Dropdown.AddOptions(resolutionList); // Add the list of resolutions to the dropdown options

        screenMode_Dropdown.ClearOptions(); // Clear the screen mode options
        screenMode_Dropdown.AddOptions(screenModes); // Populate the screen mode dropdown

        videoQuality_Dropdown.ClearOptions(); // Clear the video quality options
        videoQuality_Dropdown.AddOptions(QualitySettings.names.ToList()); // Populate the video quality dropdown
    }
    // Update the settings displayed in the pause menu
    public void UpdateSettingsDisplay()
    {
        // Update Camera Settings
        cursorVisible_Toggle.isOn = GameManager.instance.settings.isCursorVisible;
        cameraMouseControlled_Toggle.isOn = GameManager.instance.settings.isCameraMouseControlled;
        playerMouseControlled_Toggle.isOn = GameManager.instance.settings.isPlayerMouseControlled;
        worldSpace_Toggle.isOn = GameManager.instance.settings.isMovementWorldSpace;
        // Update Sound Settings
        masterVolume_Slider.value = GameManager.instance.settings.masterVolume;
        musicVolume_Slider.value = GameManager.instance.settings.musicVolume;
        effectsVolume_Slider.value = GameManager.instance.settings.effectsVolume;
        // Update Video Settings
        resolution_Dropdown.value = GameManager.instance.settings.resolution;
        videoQuality_Dropdown.value = GameManager.instance.settings.videoQuality;
        screenMode_Dropdown.value = GameManager.instance.settings.screenMode;
    }
    // Setup the screen mode and resolution
    private void SetupScreen()
    {
        if (screenMode_Dropdown.value == 0) // If full screen mode
        {
            // Full Screen mode with selected resolution
            Screen.SetResolution(resolutions[resolution_Dropdown.value].width, resolutions[resolution_Dropdown.value].height, true); 
            Cursor.lockState = CursorLockMode.Locked; // Lock the cursor (It will stay centered)
        }
        if (screenMode_Dropdown.value == 1) // If window mode
        {
            // Window with selected resolution
            Screen.SetResolution(resolutions[resolution_Dropdown.value].width, resolutions[resolution_Dropdown.value].height, false);
        }
        if (screenMode_Dropdown.value == 2) // If Borderless fullscreen mode
        {
            // Fullscreen Borderless with selected resolution
            Screen.SetResolution(resolutions[resolution_Dropdown.value].width, resolutions[resolution_Dropdown.value].height, true);

            Cursor.lockState = CursorLockMode.Confined; // Unlocks the cursor, allowing it to be moved anywhere
        }
    }
    // Activate/Deactivate the apply button to apply video settings
    public void SetApplyButtonInteractable()
    {
        applyButton.interactable = true;
    }
    // Set the video quality based on the drop down selection
    private void SetVideoQuality()
    {
        QualitySettings.SetQualityLevel(videoQuality_Dropdown.value);
    }
    // Apply video settings when the apply button is pressed
    public void ApplySettings()
    {
        // These methods will set the settings containted in settings class
        DropdownResolution();
        DropdownScreenMode();
        DropdownVideoQuality();

        // Save the settings
        SaveLoad.SaveSettings("PlayerSettings");

        // Adjust the screen (resolution and screen mode) and video quality
        SetupScreen();
        SetVideoQuality();
    }

    public void StartGame() // Start a new game when button is pressed
    {
        GameManager.instance.Init_Game(); // Run Init_Game method from GameManager
    }

    public void ResumeGame() // Resume game when button is pressed
    {
        SaveLoad.SaveSettings("PlayerSettings");
        GameManager.instance.isPaused = false; // Set this bool to false to switch game state to pause
    }

    public void QuitGame() // Quit game by switching to Quit state using enum
    {
        GameManager.instance.SetGameState(GameManager.GameState.Quit);
    }
    public void RestartGame() // Restart game by switching to Pregame state using enum
    {
        GameManager.instance.SetGameState(GameManager.GameState.Pregame);
    }
    public void ToggleCursor() // Set isCursorLocked in GameManager to toggle bool
    {
        GameManager.instance.settings.isCursorVisible = cursorVisible_Toggle.isOn;
    }

    public void ToggleCamMouse() // Set isCameraMouseControlled in GameManager to toggle bool
    {
        GameManager.instance.settings.isCameraMouseControlled = cameraMouseControlled_Toggle.isOn;
    }

    public void TogglePlayerMouse() // Set isPlayerMouseControlled in GameManager to toggle bool
    {
        GameManager.instance.settings.isPlayerMouseControlled = playerMouseControlled_Toggle.isOn;
    }

    public void ToggleWorldSpace() // Set isMovementWorldSpace in GameManager to toggle bool
    {
        GameManager.instance.settings.isMovementWorldSpace = worldSpace_Toggle.isOn;
    }
    // Set the master volume
    public void SliderMasterVolume()
    {
        GameManager.instance.settings.masterVolume = masterVolume_Slider.value; // Sync with master volume in game manager
        GameManager.instance.audioMixer.SetFloat("masterVolumeParam", masterVolume_Slider.value); // Set the audioMixer float value for masterVolumeParam
    }
    // Set the music volume
    public void SliderMusicVolume()
    {
        GameManager.instance.settings.musicVolume = musicVolume_Slider.value; // Sync with music volume in game manager
        GameManager.instance.audioMixer.SetFloat("musicVolumeParam", musicVolume_Slider.value); // Set the audioMixer float value for musicVolumeParam
    }
    // Set the effects volume
    public void SliderEffectsVolume()
    {
        GameManager.instance.settings.effectsVolume = effectsVolume_Slider.value; // Sync with effects volume in game manager
        GameManager.instance.audioMixer.SetFloat("effectsVolumeParam", effectsVolume_Slider.value); // Set the audioMixer float value for effectsVolumeParam
    }
    // Sync the resolution settings in game manager with the menu settings
    public void DropdownResolution()
    {
        GameManager.instance.settings.resolution = resolution_Dropdown.value;
    }
    // Sync the video quality settings in game manager with the menu settings
    public void DropdownVideoQuality()
    {
        GameManager.instance.settings.videoQuality = videoQuality_Dropdown.value;
    }
    // Sync the screen mode settings in game manager with the menu settings
    public void DropdownScreenMode()
    {
        GameManager.instance.settings.screenMode = screenMode_Dropdown.value;
    }
}
