using UnityEngine;
using Unity.Netcode;
public class PlayerStats : NetworkBehaviour
{
    public NetworkVariable<int> maxHealth = new NetworkVariable<int>(100);
    public NetworkVariable<int> currentHealth = new NetworkVariable<int>(100);
    public NetworkVariable<int> attackPower = new NetworkVariable<int>(10);
    public NetworkVariable<int> defense = new NetworkVariable<int>(5);

    public void TakeDamage(int damage)
    {
        if (!IsServer) return; // Only the server processes damage
        int finalDamage = Mathf.Max(damage - defense.Value, 1);
        currentHealth.Value -= finalDamage;
        Debug.Log("Player took " + finalDamage + " damage. Current health: " + currentHealth.Value);

        if (currentHealth.Value <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        Debug.Log("Player has died.");
        // Handle player death (respawn, game over, etc.)
    }
}