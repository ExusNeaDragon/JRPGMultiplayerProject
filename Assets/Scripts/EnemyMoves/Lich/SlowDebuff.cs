using UnityEngine;
using System.Collections;

public class SlowDebuff : MonoBehaviour, IEnemyAbility
{
    public float slowMultiplier = 0.85f; // Reduce speed to 85% of original (15% slow)
    public float duration = 3f;

    public void Execute(EnemyAI enemy, Transform player)
    {}
    public void Execute(EnemyAI enemy)
    {
        Collider2D hit = Physics2D.OverlapCircle(enemy.transform.position, 2f, LayerMask.GetMask("Player")); // Adjust radius if needed

        if (hit != null)
        {
            PlayerController player = hit.GetComponent<PlayerController>();
            if (player != null)
            {
                Debug.Log($"{enemy.name} applies Slow to {player.name}!");
                enemy.StartCoroutine(ApplySlow(player));
            }
        }
    }

    private IEnumerator ApplySlow(PlayerController player)
    {
        float originalSpeed = player.moveSpeed;
        player.moveSpeed *= slowMultiplier;

        yield return new WaitForSeconds(duration);

        player.moveSpeed = originalSpeed; // Reset speed after duration
        Debug.Log($"{player.name} is no longer slowed!");
    }
}

