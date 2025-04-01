using UnityEngine;
using Unity.Netcode;

public class Fireball : MonoBehaviour, IEnemyAbility
{
    public int damage = 15;
    public GameObject fireballPrefab;

    public void Execute(EnemyAI enemy, Transform player) // Remove 'override'
    {
        Debug.Log($"{enemy.name} casts a Fireball!");

        if (fireballPrefab == null)
        {
            Debug.LogError("Fireball prefab is missing!");
            return;
        }

        GameObject fireball = Instantiate(fireballPrefab, enemy.transform.position, Quaternion.identity);
        FireballProjectile projectile = fireball.GetComponent<FireballProjectile>();

        if (projectile != null)
        {
            projectile.Initialize(player.position, damage);
        }
        else
        {
            Debug.LogError("Fireball prefab is missing the FireballProjectile component!");
        }
    }
    public void Execute(EnemyAI enemy){}
}
