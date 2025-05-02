using UnityEngine;
using Unity.Netcode;

public class RegenOverTime : MonoBehaviour, IEnemyAbility
{
    public int regenAmount = 2;
    public float regenCooldown = 3f;
    private float nextRegenTime;

    public IEnemyAbility.EnemyType Type => IEnemyAbility.EnemyType.SpecialPassive;

    public void Execute(EnemyAI enemy)
    {
        if (enemy == null) return;

        bool shouldRun =
            NetworkManager.Singleton == null ||                          // Local play
            (NetworkManager.Singleton != null && enemy.IsServer);       // Server in LAN

        if (!shouldRun) return;

        if (Time.time >= nextRegenTime)
        {
            var stats = enemy.GetComponent<EnemyStats>();
            if (stats != null)
            {
                int current = stats.currentHealth.Value;
                int max = stats.maxHealth.Value;

                stats.currentHealth.Value = Mathf.Min(max, current + regenAmount);
            }

            nextRegenTime = Time.time + regenCooldown;
        }
    }

    public void Execute(EnemyAI enemy, Transform player) { }
}
