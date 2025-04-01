using UnityEngine;

public class Claw : MonoBehaviour, IEnemyAbility
{
    public int damage = 10;
    public float attackRange = 1.5f;
    public LayerMask playerLayer;

    public void Execute(EnemyAI enemy)
    {
        if (enemy == null || enemy.target == null) return;

        float distanceToTarget = Vector2.Distance(enemy.transform.position, enemy.target.position);

        if (distanceToTarget <= attackRange)
        {
            Debug.Log($"{enemy.name} slashes with Claw!");

            Animator animator = enemy.GetComponent<Animator>();
            if (animator != null) animator.SetTrigger("Attack");

            if (enemy.IsServer)
            {
                PlayerStats playerStats = enemy.target.GetComponent<PlayerStats>();
                if (playerStats != null)
                {
                    playerStats.TakeDamage(damage);
                }
            }
        }
    }

    public void Execute(EnemyAI enemy, Transform player) { }
}
