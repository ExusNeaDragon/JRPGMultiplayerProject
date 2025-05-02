using UnityEngine;
using Unity.Netcode;

public class Claw : MonoBehaviour, IEnemyAbility
{
    public LayerMask playerLayer;

    public IEnemyAbility.EnemyType Type => IEnemyAbility.EnemyType.Melee;

    public void Execute(EnemyAI enemy)
    {
        if (enemy == null || enemy.target == null) return;

        EnemyStats stats = enemy.GetComponent<EnemyStats>();
        if (stats == null) return;

        float distanceToTarget = Vector2.Distance(enemy.transform.position, enemy.target.position);

        if (distanceToTarget <= stats.attackRange)
        {
            Debug.Log($"{enemy.name} slashes with Claw!");

            Animator animator = enemy.GetComponent<Animator>();
            if (animator != null) animator.SetTrigger("Attack");

            // Only server applies damage
            if (NetworkManager.Singleton != null && NetworkManager.Singleton.IsServer)
            {
                PlayerStats playerStats = enemy.target.GetComponent<PlayerStats>();
                if (playerStats != null)
                {
                    playerStats.TakeDamage(stats.attackDamage);
                }
            }
        }
    }

    public void Execute(EnemyAI enemy, Transform player) { }
}
