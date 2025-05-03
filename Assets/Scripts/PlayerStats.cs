using UnityEngine;
using Unity.Netcode;

public class PlayerStats : NetworkBehaviour
{
    public NetworkVariable<int> maxHealth = new(100);
    public NetworkVariable<int> currentHealth = new(100);
    public NetworkVariable<int> attackPower = new(10);
    public NetworkVariable<int> defense = new(5);
    public NetworkVariable<bool> isDead = new(false,
    NetworkVariableReadPermission.Everyone,
    NetworkVariableWritePermission.Server);


    private int localMaxHealth = 100;
    private int localCurrentHealth = 100;
    private int localAttackPower = 10;
    private int localDefense = 5;

    public GameObject gameOverUI;
    public Animator animator;

    private void Awake()
    {
        animator = GameObject.Find("visual").GetComponent<Animator>();
        if (gameOverUI != null)
            gameOverUI.SetActive(false);
    }

    public void TakeDamage(int damage)
    {
        if (NetworkManager.Singleton.IsListening) // Multiplayer
        {
            if (!IsServer)
                return;

            int finalDamage = Mathf.Max(damage - defense.Value, 1);
            currentHealth.Value -= finalDamage;

            Debug.Log($"[Multiplayer] Player took {finalDamage} damage. Health: {currentHealth.Value}");

            if (currentHealth.Value <= 0)
            {
                isDead.Value = true;
                DieClientRpc(new ClientRpcParams {
                    Send = new ClientRpcSendParams {
                        TargetClientIds = new ulong[] { OwnerClientId }
                    }
                });
            }
        }
        else // Local single-player
        {
            int finalDamage = Mathf.Max(damage - localDefense, 1);
            localCurrentHealth -= finalDamage;
            Debug.Log($"[Local] Player took {finalDamage} damage. Health: {localCurrentHealth}");

            if (localCurrentHealth <= 0)
            {
                Die();
            }
        }
    }

    [ClientRpc]
    void DieClientRpc(ClientRpcParams rpcParams = default)
    {
        Die(); // Only called on client that owns the dead player
    }

    void Die()
    {
        Debug.Log("Player has died.");
        animator?.SetTrigger("death");
        GetComponent<PlayerController>()?.FreezeMovement();

        if (IsOwner && gameOverUI != null)
            gameOverUI.SetActive(true);
    }

    public void Respawn()
    {
        currentHealth.Value = maxHealth.Value;
        isDead.Value = false;
        animator?.SetTrigger("respawn");
        GetComponent<PlayerController>()?.UnfreezeMovement();

        if (IsOwner && gameOverUI != null)
            gameOverUI.SetActive(false);
    }

    [ServerRpc(RequireOwnership = false)]
    public void RequestRespawnServerRpc()
    {
        Respawn();
    }


}
