using UnityEngine;
using Unity.Netcode;
using Unity.Netcode.Components;

public class Slice : Ability
{
    public NetworkAnimator networkAnimator; // Use this instead of regular Animator
    private PlayerStats playerStats;

    public BoxCollider2D sliceColliderLeft;
    public BoxCollider2D sliceColliderRight;
    public SpriteRenderer visual;

    private void Awake()
    {
        playerStats = GetComponent<PlayerStats>();
        networkAnimator = GetComponent<NetworkAnimator>();

        sliceColliderLeft.enabled = false;
        sliceColliderRight.enabled = false;
    }

    public override void Activate()
    {
        // Ensure the NetworkObject is spawned and then trigger the animation
        if (networkAnimator != null && networkAnimator.GetComponent<NetworkObject>().IsSpawned)
        {
            networkAnimator.SetTrigger("attack2"); // Synced trigger
        }
        else
        {
            Debug.LogWarning("NetworkObject is not spawned yet. Delaying trigger activation.");
            return;
        }

        ActivateAbilityClientRpc(visual.flipX);

        BoxCollider2D activeCollider = visual.flipX ? sliceColliderLeft : sliceColliderRight;
        activeCollider.enabled = true;

        Vector2 attackDirection = visual.flipX ? Vector2.left : Vector2.right;

        // Attack detection
        ContactFilter2D filter = new ContactFilter2D();
        filter.SetLayerMask(LayerMask.GetMask("Enemy"));
        Collider2D[] hits = new Collider2D[10];
        int hitCount = activeCollider.Overlap(filter, hits);

        for (int i = 0; i < hitCount; i++)
        {
            Collider2D hit = hits[i];
            Vector2 directionToEnemy = (hit.transform.position - transform.position).normalized;
            float angle = Vector2.Angle(attackDirection, directionToEnemy);

            if (angle <= 45f)
            {
                EnemyStats enemy = hit.GetComponent<EnemyStats>();
                if (enemy != null)
                {
                    int dmg = playerStats.attackPower.Value;
                    enemy.TakeDamage(dmg);
                }
            }
        }

        // Disable colliders after the attack finishes
        Invoke(nameof(DisableColliders), .3f);
    }

    // ClientRpc to synchronize collider enabling across all clients
    [ClientRpc]
    private void ActivateAbilityClientRpc(bool flipX)
    {
        BoxCollider2D activeCollider = flipX ? sliceColliderLeft : sliceColliderRight;
        activeCollider.enabled = true;
    }

    // Disable colliders after a short duration
    private void DisableColliders()
    {
        sliceColliderLeft.enabled = false;
        sliceColliderRight.enabled = false;
    }
}
