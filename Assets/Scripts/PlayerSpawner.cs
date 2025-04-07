using UnityEngine;
using Unity.Netcode;
using UnityEngine.SceneManagement;

public class PlayerSpawner : MonoBehaviour
{
    public GameObject playerPrefab;
    private Transform spawnPoint;

    private void Start()
    {
        DontDestroyOnLoad(gameObject);
        SceneManager.sceneLoaded += OnSceneLoaded;
        FindSpawnPoint();

        if (NetworkManager.Singleton != null)
        {
            if (NetworkManager.Singleton.IsHost || NetworkManager.Singleton.IsServer)
            {
                // Server or Host automatically handles player spawning.
            }
            else
            {
                NetworkManager.Singleton.OnClientConnectedCallback += SpawnPlayer;
            }
        }
        else
        {
            SpawnLocalPlayer();  // For local play
        }
    }

    private void OnDisable()
    {
        if (NetworkManager.Singleton != null && !NetworkManager.Singleton.IsServer)
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
            // Freeze players and move them to spawn point if needed
            var network=FindObjectsByType<NetworkObject>(FindObjectsSortMode.None);
            foreach (var networkObj in network)
            {
                if (networkObj.IsPlayerObject)
                {
                    GameObject playerObj = networkObj.gameObject;
                    playerObj.transform.position = spawnPoint.position;

                    // Freeze movement in specific scenes
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

        // Dynamically spawn players
        GameObject player = Instantiate(playerPrefab, spawnPoint.position, Quaternion.identity);
        player.name = $"Player {clientId}";

        NetworkObject networkObject = player.GetComponent<NetworkObject>();
        if (NetworkManager.Singleton.IsServer || NetworkManager.Singleton.IsHost)
        {
            networkObject.SpawnAsPlayerObject(clientId);
        }
        else
        {
            networkObject.Spawn();
        }

        // Additional logic like freezing movement
        if (SceneManager.GetActiveScene().name == "WorldLoaderMultiplayer" || SceneManager.GetActiveScene().name == "WorldLoaderSingleplayer")
        {
            var movement = player.GetComponent<PlayerController>();
            if (movement != null) movement.FreezeMovement();
        }
    }

    private void SpawnLocalPlayer()
    {
        if (playerPrefab == null || spawnPoint == null) return;

        // Instantiate player for local mode
        GameObject player = Instantiate(playerPrefab, spawnPoint.position, Quaternion.identity);
        player.name = "LocalPlayer";

        // Additional logic for freezing movement
        if (SceneManager.GetActiveScene().name == "WorldLoaderMultiplayer" || SceneManager.GetActiveScene().name == "WorldLoaderSingleplayer")
        {
            var movement = player.GetComponent<PlayerController>();
            if (movement != null) movement.FreezeMovement();
        }
    }
}
