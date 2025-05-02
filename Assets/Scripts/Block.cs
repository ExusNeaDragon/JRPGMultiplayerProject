using UnityEngine;
using Unity.Netcode;
using Unity.Netcode.Components;

public class Block : Ability
{
    public NetworkAnimator networkAnimator;
    private PlayerStats playerStats;

    public BoxCollider2D blockColliderLeft;
    public BoxCollider2D blockColliderRight;
    public SpriteRenderer visual;

    private void Awake()
    {
        playerStats = GetComponent<PlayerStats>();
        networkAnimator = GetComponent<NetworkAnimator>();

        blockColliderLeft.enabled = false;
        blockColliderRight.enabled = false;
    }

    public override void Activate()
    {
        if (networkAnimator != null)
            networkAnimator.SetTrigger("attack1"); // Synced trigger

        ActivateAbilityClientRpc(visual.flipX);

        BoxCollider2D activeCollider = visual.flipX ? blockColliderLeft : blockColliderRight;
        activeCollider.enabled = true;

        Vector2 forwardDirection = visual.flipX ? Vector2.left : Vector2.right;

        LayerMask mask = LayerMask.GetMask("Projectile") | LayerMask.GetMask("Enemy");
        ContactFilter2D filter = new ContactFilter2D();
        filter.SetLayerMask(mask);
        filter.useTriggers = true;

        Collider2D[] hits = new Collider2D[10];
        int hitCount = activeCollider.Overlap(filter, hits);

        for (int i = 0; i < hitCount; i++)
        {
            Collider2D hit = hits[i];
            Vector2 dir = (hit.transform.position - transform.position).normalized;
            float angle = Vector2.Angle(forwardDirection, dir);

            if (angle <= 45f)
            {
                EnemyProjectile enemyProjectile = hit.GetComponent<EnemyProjectile>();
                EnemyStats enemy = hit.GetComponent<EnemyStats>();
                if (enemy != null)
                {
                    int dmg = playerStats.attackPower.Value;
                    enemy.TakeDamage(dmg);
                }
                else if (enemyProjectile != null)
                {
                    enemyProjectile.DestroyProjectile();
                }
            }
        }

        Invoke(nameof(DisableColliders), .3f);
    }

    [ClientRpc]
    private void ActivateAbilityClientRpc(bool flipX)
    {
        BoxCollider2D activeCollider = flipX ? blockColliderLeft : blockColliderRight;
        activeCollider.enabled = true;
    }

    private void DisableColliders()
    {
        blockColliderLeft.enabled = false;
        blockColliderRight.enabled = false;
    }
}
