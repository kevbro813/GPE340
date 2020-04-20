using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Menus : MonoBehaviour
{
    public Toggle toggle_CursorLocked; // Toggle switch for cursor lock mode
    public Toggle toggle_CameraMouseControlled; // Toggle switch for camera controlled by mouse
    public Toggle toggle_PlayerMouseControlled; // Toggle switch for player controlled by mouse
    public Toggle toggle_WorldSpace; // Toggle switch for world/self space movement controls

    public void StartGame() // Start a new game when button is pressed
    {
        GameManager.instance.Init_Game(); // Run Init_Game method from GameManager
    }

    public void ResumeGame() // Resume game when button is pressed
    {
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
    public void UpdateToggles() // Update the toggle switches to match the values stored in game manager
    {
        toggle_CursorLocked.isOn = GameManager.instance.isCursorLocked;
        toggle_CameraMouseControlled.isOn = GameManager.instance.isCameraMouseControlled;
        toggle_PlayerMouseControlled.isOn = GameManager.instance.isPlayerMouseControlled;
        toggle_WorldSpace.isOn = GameManager.instance.isMovementWorldSpace;
    }

    public void ToggleCursor() // Set isCursorLocked in GameManager to toggle bool
    {
        GameManager.instance.isCursorLocked = toggle_CursorLocked.isOn;
    }

    public void ToggleCamMouse() // Set isCameraMouseControlled in GameManager to toggle bool
    {
        GameManager.instance.isCameraMouseControlled = toggle_CameraMouseControlled.isOn;
    }

    public void TogglePlayerMouse() // Set isPlayerMouseControlled in GameManager to toggle bool
    {
        GameManager.instance.isPlayerMouseControlled = toggle_PlayerMouseControlled.isOn;
    }

    public void ToggleWorldSpace() // Set isMovementWorldSpace in GameManager to toggle bool
    {
        GameManager.instance.isMovementWorldSpace = toggle_WorldSpace.isOn;
    }
}
