using UnityEngine;
using Unity.Netcode;
using System.Collections;

public class MeleeSlash : MonoBehaviour, IEnemyAbility
{
    public float attackAngle = 90f;
    public float attackCooldown = 1.5f;

    private Animator animator;
    private bool isAttacking = false;

    public IEnemyAbility.EnemyType Type => IEnemyAbility.EnemyType.Melee;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    public void Execute(EnemyAI enemy)
    {
        if (!enemy || isAttacking) return;

        Transform player = enemy.target;
        if (!player) return;

        EnemyStats stats = enemy.GetComponent<EnemyStats>();
        if (stats == null) return;

        float distance = Vector2.Distance(enemy.transform.position, player.position);
        if (distance > stats.attackRange) return;

        Vector2 dirToPlayer = (player.position - enemy.transform.position).normalized;
        Vector2 enemyForward = enemy.transform.right; // Adjust if enemy faces up/down in sprites

        float angle = Vector2.Angle(enemyForward, dirToPlayer);
        if (angle > attackAngle / 2f) return;

        StartCoroutine(Attack(enemy, player, stats));
    }

    public void Execute(EnemyAI enemy, Transform player)
    {
        Execute(enemy); // Redirect to simpler version
    }

    private IEnumerator Attack(EnemyAI enemy, Transform player, EnemyStats stats)
    {
        isAttacking = true;

        if (animator != null)
            animator.SetTrigger("attack");

        yield return null;

        float waitTime = 0.4f;
        AnimatorStateInfo state = animator.GetCurrentAnimatorStateInfo(0);
        if (state.IsName("attack"))
            waitTime = state.length;

        yield return new WaitForSeconds(waitTime * 0.5f); // Hit lands mid-animation

        Vector2 origin = enemy.transform.position;
        LayerMask playerMask = LayerMask.GetMask("Player");
        Collider2D[] hits = Physics2D.OverlapCircleAll(origin, stats.attackRange, playerMask);

        foreach (var hit in hits)
        {
            if (hit.TryGetComponent(out PlayerStats targetStats))
            {
                targetStats.TakeDamage(stats.attackDamage);
                Debug.Log($"Melee hit player for {stats.attackDamage} damage.");
            }
        }

        yield return new WaitForSeconds(attackCooldown);
        isAttacking = false;
    }
}
