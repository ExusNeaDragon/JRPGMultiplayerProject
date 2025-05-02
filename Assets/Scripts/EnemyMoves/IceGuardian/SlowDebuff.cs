using UnityEngine;
using System.Collections;

public class SlowDebuff : MonoBehaviour, IEnemyAbility
{
    public float slowMultiplier = 0.85f; // Reduce to 85% of original speed
    public float duration = 3f;
    public float triggerThresholdPercent = 0.5f; // Activate at 50% health
    private bool hasActivated = false;

    public IEnemyAbility.EnemyType Type => IEnemyAbility.EnemyType.SpecialActive;

    public void Execute(EnemyAI enemy, Transform player) { }

    public void Execute(EnemyAI enemy)
    {
        if (hasActivated) return;

        EnemyStats stats = enemy.GetComponent<EnemyStats>();
        if (stats == null) return;

        float healthPercent = stats.currentHealth.Value / (float)stats.maxHealth.Value;
        if (healthPercent <= triggerThresholdPercent)
        {
            Collider2D hit = Physics2D.OverlapCircle(enemy.transform.position, 2f, LayerMask.GetMask("Player"));

            if (hit != null)
            {
                PlayerController player = hit.GetComponent<PlayerController>();
                if (player != null)
                {
                    Debug.Log($"{enemy.name} applies Slow to {player.name}!");
                    enemy.StartCoroutine(ApplySlow(player));
                    hasActivated = true;
                }
            }
        }
    }

    private IEnumerator ApplySlow(PlayerController player)
    {
        float originalSpeed = player.moveSpeed;
        player.moveSpeed *= slowMultiplier;

        yield return new WaitForSeconds(duration);

        player.moveSpeed = originalSpeed;
        Debug.Log($"{player.name} is no longer slowed!");
    }
}
