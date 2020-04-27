using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
public class SaveLoad
{
    // Method used to save settings
    public static void SaveSettings(string fileName)
    {
        Settings s = GameManager.instance.settings; // Get the settings from the game manager
        BinaryFormatter bf = new BinaryFormatter(); // Create a new BinaryFormatter
        FileStream file = File.Create(Path.Combine(Application.persistentDataPath, fileName)); // Create a file to save to
        bf.Serialize(file, s); // Serialize the binaryFormatter and save it to file
        file.Close(); // Close the file
        Debug.Log("Save Settings"); // Indicate the file was saved in console
    }
    // Method used to load settings
    public static void LoadSettings(string fileName)
    {
        // Check if the filename exists
        if (File.Exists(Path.Combine(Application.persistentDataPath, fileName)))
        {
            BinaryFormatter bf = new BinaryFormatter(); // Create a new BinaryFormatter
            FileStream file = File.Open(Path.Combine(Application.persistentDataPath, fileName), FileMode.Open); // Open the file
            GameManager.instance.settings = (Settings)bf.Deserialize(file); // Set the settings in GameManager to the settings loaded from file
            file.Close(); // Close the file
            Debug.Log("Load Settings"); // Indicate the file was loaded in console
        }
        else // If the file name does not exist 
        {
            SaveSettings(fileName); // Save a new file with that name
        }
    }
}
