using UnityEngine;

public class Regeneration : MonoBehaviour, IEnemyAbility
{
    public int regenAmount = 2;
    public float regenCooldown = 5f;
    private float nextRegenTime;

    public void Execute(EnemyAI enemy)
    {
        if (enemy == null || enemy.IsOwner == false) return;

        if (Time.time >= nextRegenTime)
        {
            Debug.Log($"{enemy.name} is regenerating health!");
            enemy.GetComponent<EnemyStats>().currentHealth.Value += regenAmount;
            nextRegenTime = Time.time + regenCooldown;
        }
    }
    // This is required by IEnemyAbility but not used here
    public void Execute(EnemyAI enemy, Transform player) { }
}
