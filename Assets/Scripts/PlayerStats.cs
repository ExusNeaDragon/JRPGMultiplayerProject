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
        bool isMultiplayer = NetworkManager.Singleton != null && NetworkManager.Singleton.IsListening;
        bool isServer = IsServer;

        int finalDamage;

        if (isMultiplayer)
        {
            if (!isServer)
                return; // Only the server modifies networked health

            finalDamage = Mathf.Max(damage - defense.Value, 1);
            currentHealth.Value -= finalDamage;
            Debug.Log($"[Multiplayer] Player took {finalDamage} damage. Health: {currentHealth.Value}");

            if (currentHealth.Value <= 0)
                DieClientRpc();
        }
        else
        {
            finalDamage = Mathf.Max(damage - localDefense, 1);
            localCurrentHealth -= finalDamage;
            Debug.Log($"[Local] Player took {finalDamage} damage. Health: {localCurrentHealth}");

            if (localCurrentHealth <= 0)
                Die();
        }
    }

    [ClientRpc]
    void DieClientRpc() => Die();

    void Die()
    {
        Debug.Log("Player has died.");
        animator?.SetTrigger("death");

        GetComponent<PlayerController>()?.FreezeMovement();
        isDead.Value = true;

        if (IsOwner && gameOverUI != null)
            gameOverUI.SetActive(true);
    }

}
