using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;
using MyGameNamespace;

public class PlayerSpawner : MonoBehaviour
{
    public GameObject playerPrefab;
    private Transform spawnPoint;

    private void Start()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
        FindSpawnPoint();
        
        if (GameState.IsSinglePlayer)
        {
            // Single-player setup (NetworkManager will handle spawning)
            if (!NetworkManager.Singleton.IsHost && !NetworkManager.Singleton.IsServer)
            {
                NetworkManager.Singleton.StartHost(); // Start the host for single-player
            }

            SpawnLocalPlayer();
        }
        else if (NetworkManager.Singleton != null)
        {
            // Multiplayer setup
            if (NetworkManager.Singleton.IsHost || NetworkManager.Singleton.IsServer)
            {
                // Server/Host auto-handles spawning
            }
            else
            {
                NetworkManager.Singleton.OnClientConnectedCallback += SpawnPlayer;
            }
        }
    }

    private void OnDisable()
    {
        if (!GameState.IsSinglePlayer &&
            NetworkManager.Singleton != null &&
            !NetworkManager.Singleton.IsServer)
        {
            NetworkManager.Singleton.OnClientConnectedCallback -= SpawnPlayer;
        }

        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void FindSpawnPoint()
    {
        GameObject sp = GameObject.Find("SpawnPoint");
        if (sp != null) spawnPoint = sp.transform;
        else Debug.LogError("SpawnPoint not found in scene!");
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        FindSpawnPoint();

        if (spawnPoint != null)
        {
            var players = FindObjectsByType<NetworkObject>(FindObjectsSortMode.None);
            foreach (var networkObj in players)
            {
                if (networkObj.IsPlayerObject)
                {
                    GameObject playerObj = networkObj.gameObject;
                    playerObj.transform.position = spawnPoint.position;

                    if (scene.name == "WorldLoaderMultiplayer" || scene.name == "WorldLoaderSingleplayer")
                    {
                        var controller = playerObj.GetComponent<PlayerController>();
                        if (controller != null)
                            controller.FreezeMovement();
                    }
                }
            }
        }
    }

    private void SpawnPlayer(ulong clientId)
    {
        if (playerPrefab == null || spawnPoint == null) return;

        GameObject player = Instantiate(playerPrefab, spawnPoint.position, Quaternion.identity);
        player.name = $"Player {clientId}";

        var networkObject = player.GetComponent<NetworkObject>();
        if (NetworkManager.Singleton.IsServer || NetworkManager.Singleton.IsHost)
        {
            networkObject.SpawnAsPlayerObject(clientId);
        }
        else
        {
            networkObject.Spawn();
        }

        if (SceneManager.GetActiveScene().name == "WorldLoaderMultiplayer" || SceneManager.GetActiveScene().name == "WorldLoaderSingleplayer")
        {
            var movement = player.GetComponent<PlayerController>();
            if (movement != null) movement.FreezeMovement();
        }
    }

    private void SpawnLocalPlayer()
    {
        if (playerPrefab == null || spawnPoint == null) return;

        // Start the NetworkManager in single-player mode (auto-spawns player)
        if (!NetworkManager.Singleton.IsHost && !NetworkManager.Singleton.IsServer)
        {
            NetworkManager.Singleton.StartHost(); // This spawns the playerPrefab
        }

        // Player prefab will not be set to DontDestroyOnLoad in either mode
        // Let the NetworkManager handle the persistence

        // Wait briefly or ensure player is spawned before using it
        if (SceneManager.GetActiveScene().name == "WorldLoaderMultiplayer" ||
            SceneManager.GetActiveScene().name == "WorldLoaderSingleplayer")
        {
            var localPlayer = NetworkManager.Singleton.LocalClient?.PlayerObject?.GetComponent<PlayerController>();
            if (localPlayer != null)
            {
                localPlayer.FreezeMovement();
            }
        }
    }
}
