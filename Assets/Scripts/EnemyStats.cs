using UnityEngine;
using Unity.Netcode;

public class EnemyStats : NetworkBehaviour
{
    public NetworkVariable<int> maxHealth = new NetworkVariable<int>(100);
    public NetworkVariable<int> currentHealth = new NetworkVariable<int>(100);

    public int localMaxHealth = 100;
    public int localCurrentHealth = 100;

    public float moveSpeed = 3f;
    public int attackDamage = 10;
    public float attackRange = 1.5f;
    public float chaseRange = 7f;
    public float projectileRange = 5f;

    private Animator animator;

    private void Start()
    {
        animator = GetComponent<Animator>();
    }

    [ServerRpc(RequireOwnership = false)]
    public void TakeDamageServerRpc(int damage)
    {
        TakeDamageInternal(damage, isMultiplayer: true);
    }

    public void TakeDamage(int damage)
    {
        bool isMultiplayer = NetworkManager.Singleton != null && NetworkManager.Singleton.IsListening;

        if (isMultiplayer && !IsServer) return;

        TakeDamageInternal(damage, isMultiplayer);
    }

    private void TakeDamageInternal(int damage, bool isMultiplayer)
    {
        animator?.SetTrigger("hit");

        if (isMultiplayer)
        {
            currentHealth.Value -= damage;
            if (currentHealth.Value <= 0) Die();
        }
        else
        {
            localCurrentHealth -= damage;
            if (localCurrentHealth <= 0) Die();
        }
    }

    private void Die()
    {
        animator?.SetTrigger("death");

        if (IsServer || !NetworkManager.Singleton.IsListening)
            Destroy(gameObject);
    }
}
