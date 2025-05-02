using UnityEngine;
using Unity.Netcode;
using System.Collections;

public class Fireball : MonoBehaviour, IEnemyAbility
{
    public GameObject fireballPrefab;

    public IEnemyAbility.EnemyType Type => IEnemyAbility.EnemyType.Ranged;
    private Animator animator;
    private bool isAttacking = false;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    public void Execute(EnemyAI enemy, Transform player)
    {
        if (isAttacking || enemy == null || player == null || fireballPrefab == null) return;

        EnemyStats stats = enemy.GetComponent<EnemyStats>();
        if (stats == null) return;

        float distanceToTarget = Vector2.Distance(enemy.transform.position, player.position);
        if (distanceToTarget > stats.projectileRange) return;

        StartCoroutine(AttackThenFire(enemy, player, stats));
    }

    private IEnumerator AttackThenFire(EnemyAI enemy, Transform player, EnemyStats stats)
    {
        isAttacking = true;

        animator.SetTrigger("attack");

        yield return null;

        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
        float waitTime = 0.5f;

        if (stateInfo.IsName("attack"))
        {
            waitTime = stateInfo.length;
        }

        yield return new WaitForSeconds(waitTime);

        GameObject fireball = Instantiate(fireballPrefab, enemy.transform.position, Quaternion.identity);

        NetworkObject netObj = fireball.GetComponent<NetworkObject>();
        if (netObj != null && NetworkManager.Singleton.IsServer)
        {
            netObj.Spawn();
        }
        else if (netObj == null)
        {
            Debug.LogError("Fireball prefab must have a NetworkObject component!");
        }

        EnemyProjectile projectile = fireball.GetComponent<EnemyProjectile>();
        if (projectile != null)
        {
            projectile.Initialize((Vector2)player.position, stats.attackDamage);
        }
        else
        {
            Debug.LogError("Fireball prefab is missing the EnemyProjectile component!");
        }

        isAttacking = false;
    }

    public void Execute(EnemyAI enemy) { }
}
