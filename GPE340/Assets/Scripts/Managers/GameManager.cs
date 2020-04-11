using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// GameManager script is a singleton
public class GameManager : MonoBehaviour
{
    [HideInInspector] public static GameManager instance; // Static instance of the GameManager for Singleton Pattern
    [Header ("Camera Settings")]
    public Camera mainCam; // Main camera component
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

    [Header("Player Settings")]
    public List<GameObject> activePlayers; // List of currently active players
    public Transform playerSpawn; // List of player spawns
    public GameObject playerPrototype; // Player prototype used to instantiate players
    
    [Header("UI Settings")]
    public GameObject hudObject; // HUD gameObject that can be set active/inactive
    public GameObject startMenu; // Start menu gameObject
    public PlayerHUD playerHUD; // Player HUD Component, has all methods used to update the HUD

    [Header("Pickup Settings")]
    public List<GameObject> pickupPrototypes; // List of pickup prefabs
    public List<Transform> pickupSpawnLocations; // spawnlocations list is used to remove old spawn locations (prevents double spawning at same location)
    public List<GameObject> availablePickups; // List of active pickups
    public float pickupRespawnDelay = 10f; // Delay before the pickups are spawned/respawned
    public int maxPickups = 3; // Maximum number of pickups spawned
    private int currentPickupQty = 0; // Keeps track of the current number of active pickups

    [Header("Weapon Settings")]
    public List<GameObject> weaponPrototypes; // List of weapon prefabs
    public List<Transform> weaponSpawnLocations; // List of weapon spawn locations
    public List<GameObject> availableWeapons; // List of currently available weapons
    public int maxWeapons = 3; // Max number of weapons to spawn
    private int currentWeaponQty = 0; // Tracks the current number of weapons available

    [Header("Enemy Settings")]
    public List<GameObject> enemyPrototypes; // List of enemy prefabs
    public List<Transform> enemySpawnLocations; // List of enemy spawn locations
    public List<GameObject> activeEnemies; // List of active enemies
    public int maxEnemies = 1; // Maximum number of enemies spawned
    public int currentEnemies = 0; // Tracks current number of enemies
    public float enemyRespawnDelay = 3.0f; // Duration to wait before spawning/respawning enemies

    [Header("Empty Shell Objects")]
    public Transform pickupShell;
    public Transform playerShell;
    public Transform enemyShell;
    public Transform weaponShell;

    [Header("Other Settings")]
    public GameState gameState; // Tracks the current game state using an enum

    public enum GameState { Pregame, Active, Postgame, Pause, Resume } // Enum for game state

    private void Awake()
    {
        // Singleton
        if (!instance) // If the instance is null...
        {
            instance = this; // This instance is set to the static instance
            DontDestroyOnLoad(gameObject); // Do not destroy GameManager on load
        }
        else // If the instance is not null...
        {
            Destroy(gameObject); // Destroy the gameObject
        }
        isCameraMouseControlled = true; // Set camera to mouse controlled (preferred setting for this game)

        gameState = GameState.Pregame; // Set initial game state
    }
    private void Start()
    {
        hudObject = GameObject.FindWithTag("HUD"); // Set the hudObject by finding the object tagged with "HUD"
        startMenu = GameObject.FindWithTag("StartMenu"); // Set the startMenu by finding the object tagged with "StartMenu"
        startMenu.SetActive(true); // Activate the start menu at the beginning of the game
        hudObject.SetActive(false); // Deactivate the HUD at the beginning of the game
        mainCam = Camera.main; // Set the mainCam variable to the main camera
        playerHUD = hudObject.GetComponent<PlayerHUD>(); // Set the playerHUD component
    }
    // TODO: Complete Main Game FSM
    private void Update()
    {
        if (gameState == GameState.Pregame) // If in the pregame state...
        {
            DoPregame(); // Run DoPregame method
        }
        if (gameState == GameState.Active) // If the game is currently active...
        {
            DoActive(); // Run DoActive method
        }
        if (gameState == GameState.Postgame) // If the game is finished...
        {
            DoPostgame(); // Run DoPostgame method
        }
        if (gameState == GameState.Pause) // If the game is paused...
        {
            DoPause(); // Run DoPause method
        }
        if (gameState == GameState.Resume) // If the game is resumed...
        { 
            DoResume(); // Run DoResume method
        }
    }

    // Initialize a new game
    public void Init_Game()
    {
        // Deactivate Start menu
        startMenu.SetActive(false);

        // Activate HUD
        hudObject.SetActive(true);

        // Instantiate Player
        InstantiatePlayer();

        // Instantiate enemies
        InstantiateEnemies();

        // Instantiate Pickups
        InstantiatePickups();

        // Instantiate Weapons
        InstantiateWeapons();
    }

    public void DoPregame()
    {
        // TODO: Default State that runs before game is active
    }
    public void DoActive()
    {
        // TODO: State of an active game
    }
    public void DoPostgame()
    {
        // TODO: State after the game concludes
    }
    public void DoPause()
    {
        // TODO: Pauses the game
    }
    public void DoResume()
    {
        // TODO: Unpauses the game
    }

    // Instantiate the player, reset the player's settings, update the playerHUD and set the camera's follow object
    public void InstantiatePlayer()
    {
        // Instantiate player pawn
        GameObject playerClone = Instantiate(playerPrototype, playerSpawn.position, playerSpawn.rotation, playerShell) as GameObject;

        // Add player to list of active players (will be helpful if I add multiplayer)
        activePlayers.Add(playerClone);

        // Set camera's follow object and the follow object's transform component
        SetCamera(playerClone);
    }

    // Method to instantiate enemies (Starts coroutine to spawn enemies)
    public void InstantiateEnemies()
    {
        StartCoroutine("SpawnEnemiesEvent");
    }
    // Method to instantiate pickups (Starts coroutine to spawn pickups)
    public void InstantiatePickups()
    {
        StartCoroutine("SpawnPickupsEvent");
    }
    // Method to instantiate weapons (Starts coroutine to spawn weapons)
    public void InstantiateWeapons()
    {
        StartCoroutine("SpawnWeaponsEvent");
    }
    // Set the camera's follow object (the object the camera will follow while in trailing mode
    public void SetCamera(GameObject playerToFollow) // Pass in the object to follow
    {
        // Set follow object of camera
        mainCam.GetComponent<CameraManager>().followObject = playerToFollow;

        // Set transform component of the follow object that is contained in the CameraManager script
        mainCam.GetComponent<CameraManager>().fotf = playerToFollow.GetComponent<Transform>();
    }

    // Coroutine to spawn pickups
    public IEnumerator SpawnPickupsEvent()
    {
        yield return new WaitForSeconds(pickupRespawnDelay); // Wait the "pickupRespawnDelay" duration
        while (currentPickupQty < maxPickups) // If the currentPickupQty does not exceed the maxPickups...
        {
            currentPickupQty++; // Increment pickup quantity
            // Randomly choose a temporary pickup spawnpoint
            Transform pickupSpawnpoint = pickupSpawnLocations[Random.Range(0, pickupSpawnLocations.Count)];
            // Randomly choose a temporary pickup prefab to use
            GameObject randomPickup = pickupPrototypes[Random.Range(0, pickupPrototypes.Count)];

            // Instantiate pickup at the pickupSpawnpoint
            GameObject pickupClone = Instantiate(randomPickup, pickupSpawnpoint.position, pickupSpawnpoint.rotation, pickupShell) as GameObject;

            pickupPrototypes.Remove(randomPickup); // Remove pickup from list (prevents duplicates)
            pickupSpawnLocations.Remove(pickupSpawnpoint); // Remove pickup spawn from list (prevents multiple pickups spawning in same location)
            availablePickups.Add(pickupClone); // Add to list of available pickups
        }
    }
    // Coroutine to spawn weapons
    public IEnumerator SpawnWeaponsEvent()
    {
        yield return new WaitForSeconds(pickupRespawnDelay); // Wait the "pickupRespawnDelay" duration
        while (currentWeaponQty < maxWeapons) // If the currentWeaponQty does not exceed the maxWeapons...
        {
            currentWeaponQty++; // Increment currentWeaponQty

            // Randomly choose a temporary pickup spawnpoint
            Transform weaponSpawnpoint = weaponSpawnLocations[Random.Range(0, weaponSpawnLocations.Count)];
            // Randomly choose a temporary weapon prefab to use
            GameObject randomWeapon = weaponPrototypes[Random.Range(0, weaponPrototypes.Count)];

            // Instantiate weapon at the weaponSpawnpoint
            GameObject weaponClone = Instantiate(randomWeapon, weaponSpawnpoint.position, weaponSpawnpoint.rotation, weaponShell) as GameObject;

            weaponPrototypes.Remove(randomWeapon); // Remove weapon from list (prevents duplicates)
            weaponSpawnLocations.Remove(weaponSpawnpoint); // Remove weapon spawn from list (prevents multiple weapons spawning in same location)
            availableWeapons.Add(weaponClone); // Add to list of available weapons
        }
    }
    // Coroutine to spawn enemies
    public IEnumerator SpawnEnemiesEvent()
    {
        yield return new WaitForSeconds(enemyRespawnDelay); // Wait the "enemyRespawnDelay" duration
        while (currentEnemies < maxEnemies) // If the currentEnemies does not exceed the maxEnemies...
        {
            currentEnemies++; // Increment currentEnemies

            // Randomly choose a temporary pickup spawnpoint
            Transform enemySpawnpoint = enemySpawnLocations[Random.Range(0, enemySpawnLocations.Count)];
            // Randomly choose a temporary enemy prefab to use
            GameObject randomEnemy = enemyPrototypes[Random.Range(0, enemyPrototypes.Count)];

            // Instantiate enemy at the enemySpawnpoint
            GameObject enemyClone = Instantiate(randomEnemy, enemySpawnpoint.position, enemySpawnpoint.rotation, enemyShell) as GameObject;

            activeEnemies.Add(enemyClone); // Add to list of available enemies
        } 
    }
}
