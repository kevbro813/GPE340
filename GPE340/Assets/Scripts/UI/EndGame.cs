using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EndGame : MonoBehaviour
{
    public void NewGame() // Start a new game by switching to main scene
    {
        SceneManager.LoadScene(0);
    }

    public void QuitGame() // Quit game
    {
        Application.Quit();

        // The following will only run in the Unity Editor and will be ignored in build
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false; // Quit out of editor play mode
#endif
    }
}
