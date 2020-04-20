using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// GameManager script is a singleton
public class GameManager : MonoBehaviour
{
    [HideInInspector] public static GameManager instance; // Static instance of the GameManager for Singleton Pattern
    [Header ("Camera Settings")]
    public Camera mainCam; // Main camera component
    public bool isCursorLocked = true; // Is cursor locked and invisible
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
    public GameObject pauseMenu; // Pause menu gameObject
    public Menus menuComponent; // Component for the pause menu

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
    public List<Transform> enemyWaypoints; // List of enemy weapoints
    public int maxEnemies = 1; // Maximum number of enemies spawned
    public int currentEnemies = 0; // Tracks current number of enemies
    public float enemyRespawnDelay = 3.0f; // Duration to wait before spawning/respawning enemies

    [Header("Empty Shell Objects")]
    public Transform pickupShell; // Empty Game object used to contain all pickups
    public Transform playerShell; // Empty Game object used to contain all players
    public Transform enemyShell; // Empty Game object used to contain all enemies
    public Transform weaponShell; // Empty Game object used to contain all weapons
    public Transform itemDropShell; // Empty game object used to contain items that are dropped

    [Header("Enemy Patrol Data")]
    public Vector3 lastSoundLocation; // Location of the last time an AI heard the player
    public Vector3 lastPlayerLocation; // Location of the last time an AI saw the player
    public bool isAlerted; // Indicates if an enemy has seen the player and alerts all allies

    [Header("Other Settings")]
    public GameState gameState; // Tracks the current game state using an enum
    public enum GameState { Pregame, Active, Postgame, Pause, PrePause, Resume, Quit } // Enum for game state

    public bool isPaused; // Bool to indicate if the game is paused
    public int playerLives; // Tracks the number of lives the player has
    public List<Sprite> weaponIconsList; // List of weapon icons
    public Sprite currentWeaponIcon; // Current weapon icon based on the currently equipped weapon
    public WeaponIcon weaponIcon; // Enum for weapon icons
    public enum WeaponIcon { Pistol, Rifle, Shotgun, None };

    private void Awake()
    {
        // Singleton
        if (!instance) // If the instance is null...
        {
            instance = this; // This instance is set to the static instance
            //DontDestroyOnLoad(gameObject); // Destroy on load for new games
        }
        else // If the instance is not null...
        {
            Destroy(gameObject); // Destroy the gameObject
        }

        hudObject = GameObject.FindWithTag("HUD"); // Set the hudObject by finding the object tagged with "HUD"
        startMenu = GameObject.FindWithTag("StartMenu"); // Set the startMenu by finding the object tagged with "StartMenu"
        pauseMenu = GameObject.FindWithTag("PauseMenu"); // Set the pauseMenu
        playerHUD = hudObject.GetComponent<PlayerHUD>(); // Set the playerHUD component
        menuComponent = pauseMenu.GetComponentInParent<Menus>(); // Get the UI menu component attached to the parent // TODO: MIGHT NOT NEED THIS
        mainCam = Camera.main; // Set the mainCam variable to the main camera
    }
    private void Start()
    {
        gameState = GameState.Pregame; // Set initial game state
        startMenu.SetActive(true); // Activate the start menu at the beginning of the game
        hudObject.SetActive(false); // Deactivate the HUD at the beginning of the game
        pauseMenu.SetActive(false); // Deactivate the Pause menu at the beginning of the game
        playerLives = 3; // Set player lives to 3
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
            if (isPaused) // If game is paused
            {
                SetGameState(GameState.PrePause); // Transition to prepause state
            }
        }
        if (gameState == GameState.Postgame) // If the game is finished...
        {
            DoPostgame(); // Run DoPostgame method
        }
        if (gameState == GameState.Pause) // If the game is paused...
        {
            DoPause(); // Run DoPause method
            if (!isPaused) // If game is not paused
            {
                SetGameState(GameState.Resume); // Transition to resume state
            }
        }
        if (gameState == GameState.Resume) // If the game is resumed...
        { 
            DoResume(); // Run DoResume method
            SetGameState(GameState.Active); // Set to active game state
        }

        if (gameState == GameState.Quit) // Quit game
        {
            DoQuit();
        }

        if (gameState == GameState.PrePause) // Pre pause state
        {
            DoPrePause(); // Run prepause state
            SetGameState(GameState.Pause); // Transition directly to pause state
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

        // Set the game to active state
        SetGameState(GameState.Active);
    }
    private void DoPrePause() // PrePause runs before game enters pause since the following code should only run once
    {
        Debug.Log("PrePause");
        pauseMenu.SetActive(true); // Activate the pause menu
        menuComponent.UpdateToggles(); // Update the toggle options in the pause menu
        LockCursor(false); // Set the cursor mode to not locked
        Time.timeScale = 0f; // Pause time
    }
    private void DoPregame() // Run pregame state
    {
        Debug.Log("Pregame");
        startMenu.SetActive(true); // Set the start menu to active
    }
    private void DoActive() // Run the active state
    {
        Debug.Log("Active");
        LockCursor(true); // Set cursor mode to locked
    }
    private void DoPostgame() // Run post game state
    {
        Debug.Log("Postgame");
        LockCursor(false); // Set cursor to not locked
        SetGameState(GameState.Pregame); // Enter pregame state
        Time.timeScale = 1f; // Unpause time
        UnityEngine.SceneManagement.SceneManager.LoadScene(1); // Load end game scene
    }
    private void DoPause() // State when game is currently paused
    {
        Debug.Log("Paused");
        // Game in paused state, do nothing
    }
    private void DoResume() // Run the resume state (go from paused to unpaused)
    {
        Debug.Log("Resume");
        // Unpauses the game
        pauseMenu.SetActive(false); // Deactivate the pause menu
        LockCursor(true); // Set the cursor mode to locked
        Time.timeScale = 1f; // Activate time
    }

    private void DoQuit() // Quit game
    {
        Debug.Log("Quit");
        Application.Quit(); // Quit application

        // This will only run in the unity editor and will close out the editor play mode
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }

    // Set the game state
    public void SetGameState(GameState gs)
    {
        gameState = gs;
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

    // Lock or unlock the cursor, allows use of menus while paused
    public void LockCursor(bool isLocked)
    {
        isCursorLocked = isLocked; // Set isCursorLocked

        if (isCursorLocked) // If the cursor is locked
        {
            Cursor.lockState = CursorLockMode.Locked; // Lock the cursor (It will stay centered)
            Cursor.visible = false; // Makes the cursor invisible
        }
        else // If the cursor is not locked
        {
            Cursor.lockState = CursorLockMode.None; // Unlocks the cursor, allowing it to be moved anywhere
            Cursor.visible = true; // Makes the cursor visible
        }
    }
}
