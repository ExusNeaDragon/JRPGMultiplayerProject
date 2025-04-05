using UnityEngine;
using Unity.Netcode;

public class PlayerSpawner : MonoBehaviour
{
    public GameObject playerPrefab; // Player prefab with camera attached
    private Transform spawnPoint; // Assign in Inspector or Find in Scene

    private void Start()
    {
        // Ensure spawn point exists before spawning
        if (spawnPoint == null)
        {
            GameObject sp = GameObject.Find("SpawnPoint");
            if (sp != null) spawnPoint = sp.transform;
            else Debug.LogError("SpawnPoint not found in scene!");
        }

        if (NetworkManager.Singleton != null)
        {
            if (NetworkManager.Singleton.IsHost)
            {
                // Host is both server and player. Spawn for the host and other clients.
                SpawnPlayer(NetworkManager.Singleton.LocalClientId);
                foreach (var client in NetworkManager.Singleton.ConnectedClients)
                {
                    if (client.Key != NetworkManager.Singleton.LocalClientId)
                    {
                        SpawnPlayer(client.Key);
                    }
                }
            }
            else if (NetworkManager.Singleton.IsServer)
            {
                // Server is responsible for spawning players for all clients
                foreach (var client in NetworkManager.Singleton.ConnectedClients)
                {
                    SpawnPlayer(client.Key);
                }
            }
            else
            {
                // Client listens for connection and spawns player when connected
                NetworkManager.Singleton.OnClientConnectedCallback += SpawnPlayer;
            }
        }
        else
        {
            // Single Player: Spawn immediately
            SpawnLocalPlayer();
        }
    }

    private void OnEnable()
    {
        if (NetworkManager.Singleton != null && !NetworkManager.Singleton.IsServer)
        {
            NetworkManager.Singleton.OnClientConnectedCallback += SpawnPlayer;
        }
    }

    private void OnDisable()
    {
        if (NetworkManager.Singleton != null && !NetworkManager.Singleton.IsServer)
        {
            NetworkManager.Singleton.OnClientConnectedCallback -= SpawnPlayer;
        }
    }

    void SpawnPlayer(ulong clientId){
    if (playerPrefab == null || spawnPoint == null) return;

    GameObject player = Instantiate(playerPrefab, spawnPoint.position, Quaternion.identity);
    NetworkObject networkObject = player.GetComponent<NetworkObject>();

    if (NetworkManager.Singleton.IsServer || NetworkManager.Singleton.IsHost){
        networkObject.SpawnAsPlayerObject(clientId);
    }
    else{
        networkObject.Spawn(); // Client-side instantiation
    }
    }


    void SpawnLocalPlayer(){
    if (playerPrefab == null || spawnPoint == null) return;
    Instantiate(playerPrefab, spawnPoint.position, Quaternion.identity);
    }

}
