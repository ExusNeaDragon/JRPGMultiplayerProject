using UnityEngine;
using Unity.Netcode;

public class EnemyStats : NetworkBehaviour
{
    public NetworkVariable<int> maxHealth = new(100);
    public NetworkVariable<int> currentHealth = new(100);

    public int localMaxHealth = 100;
    public int localCurrentHealth = 100;

    public float moveSpeed = 3f;
    public int attackDamage = 10;
    public float attackRange = 1.5f;
    public float chaseRange = 7f;
    public float projectileRange = 5f;

    private Animator animator;
    private bool isDead = false;

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
        if (isDead) return;

        animator?.SetTrigger("hit");

        if (isMultiplayer)
        {
            currentHealth.Value -= damage;
            if (currentHealth.Value <= 0)
            {
                isDead = true;
                DieClientRpc(); // Sync death animation to all clients
                StartCoroutine(DestroyAfterDelay(2f)); // Delay destruction
            }
        }
        else
        {
            localCurrentHealth -= damage;
            if (localCurrentHealth <= 0)
            {
                isDead = true;
                Die(); // Local-only
            }
        }
    }

    [ClientRpc]
    private void DieClientRpc()
    {
        animator?.SetTrigger("death");
    }

    private void Die()
    {
        animator?.SetTrigger("death");
        Destroy(gameObject, 2f); // Optional delay
    }

    private System.Collections.IEnumerator DestroyAfterDelay(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        if (IsServer)
        {
            NetworkObject.Despawn(true); // Proper Netcode despawn
        }
    }
}
