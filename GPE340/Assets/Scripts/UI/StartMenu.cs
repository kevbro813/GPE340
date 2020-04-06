using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Component that handles the start menu
public class StartMenu : MonoBehaviour
{
    public void StartGame() // Start a new game when buttin is pressed
    {
        GameManager.instance.Init_Game(); // Run Init_Game method from GameManager
    }
}
