using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode;
using UnityEngine.SceneManagement;

public class GameOverUI : MonoBehaviour
{
    public Button respawnButton;
    public Button exitButton;

    private void Awake()
    {
        respawnButton.onClick.AddListener(OnRespawnButtonClick);
        exitButton.onClick.AddListener(OnExitButtonClick);
    }

    void OnRespawnButtonClick()
    {
        if (NetworkManager.Singleton.IsServer)
        {
            // Trigger respawn logic on the server (host)
            RespawnPlayer();
        }
        else
        {
            // Client requests respawn
            RequestRespawnServerRpc();
        }
    }

    void OnExitButtonClick()
    {
        // Exit to the main menu logic
        ExitToMenu();
    }

    void RespawnPlayer()
    {
        // Respawn logic for the player
        PlayerStats stats = GetComponentInParent<PlayerStats>();
        if (stats == null) return;
        stats.currentHealth.Value = stats.maxHealth.Value;
        stats.isDead.Value = false;
        stats.GetComponent<PlayerController>().UnfreezeMovement();
        gameObject.SetActive(false); // Hide GameOver UI after respawn
    }

    // ServerRpc to trigger respawn
    [ServerRpc]
    void RequestRespawnServerRpc()
    {
        RespawnPlayer();
    }

    void ExitToMenu()
    {
        if (NetworkManager.Singleton.IsHost)
        {
            // Destroy GameManager if it exists
            GameObject gm = GameObject.Find("GameManager");
            if (gm != null) Destroy(gm);

            // Host triggers scene change for all
            NetworkManager.Singleton.SceneManager.LoadScene("Menu", LoadSceneMode.Single);
        }
        else if (NetworkManager.Singleton.IsClient)
        {
            // Client requests exit to menu
            ExitToMenuServerRpc();
        }
    }


    [ServerRpc(RequireOwnership = false)]
    void ExitToMenuServerRpc()
    {
        GameObject gm = GameObject.Find("GameManager");
        if (gm != null) Destroy(gm);

        NetworkManager.Singleton.SceneManager.LoadScene("Menu", LoadSceneMode.Single);
    }

}
