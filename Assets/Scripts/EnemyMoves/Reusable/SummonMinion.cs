using UnityEngine;
using System.Collections;

public class SummonMinions : MonoBehaviour, IEnemyAbility
{
    public GameObject minionPrefab;
    public float summonInterval = 3f;
    public IEnemyAbility.EnemyType Type => IEnemyAbility.EnemyType.SpecialPassive;
    private EnemyAI enemyAI;

    private void Start()
    {
        enemyAI = GetComponent<EnemyAI>();
        if (enemyAI != null && enemyAI.IsOwner)
        {
            StartCoroutine(SummonMinionsOverTime());
        }
    }

    private IEnumerator SummonMinionsOverTime()
    {
        while (true)
        {
            if (minionPrefab != null)
            {
                Debug.Log($"{enemyAI.name} passively summons a minion.");
                Instantiate(minionPrefab, transform.position + new Vector3(1, 0, 0), Quaternion.identity);
            }
            yield return new WaitForSeconds(summonInterval);
        }
    }

    // Unused interface methods (required by IEnemyAbility)
    public void Execute(EnemyAI enemy) { }
    public void Execute(EnemyAI enemy, Transform player) { }
}
