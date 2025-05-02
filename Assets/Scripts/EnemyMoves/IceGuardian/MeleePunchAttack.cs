using UnityEngine;
using System.Collections;

public class MeleePunchTrigger : MonoBehaviour, IEnemyAbility
{
    public float attackCooldown = 1f;
    private bool isOnCooldown = false;
    private Animator animator;
    private EnemyStats enemyStats;

    public IEnemyAbility.EnemyType Type => IEnemyAbility.EnemyType.Melee;

    private void Awake()
    {
        animator = GetComponentInParent<Animator>();
        enemyStats = GetComponentInParent<EnemyStats>();

        if (!GetComponent<Collider2D>().isTrigger)
            Debug.LogWarning("MeleePunchTrigger collider must be marked as Trigger.");
    }

    public void Execute(EnemyAI enemy, Transform target)
    {
        if (isOnCooldown || target == null || enemyStats == null) return;

        // Check for overlap within a small range (e.g. 1.0f)
        Collider2D hit = Physics2D.OverlapCircle(enemy.transform.position, 1.0f, LayerMask.GetMask("Player"));
        if (hit != null && hit.CompareTag("Player"))
        {
            if (hit.TryGetComponent(out PlayerStats playerStats))
            {
                animator?.SetTrigger("attack");
                playerStats.TakeDamage(enemyStats.attackDamage);
                Debug.Log($"[MeleePunchTrigger] Player hit for {enemyStats.attackDamage} damage.");
                enemy.StartCoroutine(Cooldown());
            }
        }
    }

    public void Execute(EnemyAI enemy) { /* Unused for this ability */ }

    private IEnumerator Cooldown()
    {
        isOnCooldown = true;
        yield return new WaitForSeconds(attackCooldown);
        isOnCooldown = false;
    }
}
