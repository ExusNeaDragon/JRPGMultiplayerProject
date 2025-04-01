using UnityEngine;

public class SummonMinions : MonoBehaviour, IEnemyAbility
{
    public GameObject minionPrefab;
    public float summonCooldown = 10f;
    private float nextSummonTime;

    public void Execute(EnemyAI enemy)
    {
        if (enemy == null || enemy.IsOwner == false) return;
        if (Time.time >= nextSummonTime)
        {
            Debug.Log("Enemy summoning minions!");
            Instantiate(minionPrefab, enemy.transform.position + new Vector3(1, 0, 0), Quaternion.identity);
            nextSummonTime = Time.time + summonCooldown;
        }
    }
    public void Execute(EnemyAI enemy, Transform player) { }
}
