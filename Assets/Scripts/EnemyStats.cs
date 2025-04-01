using UnityEngine;
using System.Collections.Generic;
using Unity.Netcode;
public class EnemyStats : NetworkBehaviour
{
    public NetworkVariable<int> maxHealth = new NetworkVariable<int>(50);
    public NetworkVariable<int> currentHealth = new NetworkVariable<int>(50);

    public void TakeDamage(int damage)
    {
        if (!IsServer) return;
        currentHealth.Value -= damage;
        Debug.Log("Enemy took " + damage + " damage. Current health: " + currentHealth.Value);

        if (currentHealth.Value <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        Debug.Log("Enemy has died.");
        Destroy(gameObject);
    }
}