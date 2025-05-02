using UnityEngine;
using Unity.Netcode;
using System.Collections.Generic;
using MyGameNamespace;

public class EnemyAI : NetworkBehaviour
{
    private PlayerStats playerStats;
    private Animator animator;
    private Vector3 originalScale;
    private List<IEnemyAbility> abilities = new List<IEnemyAbility>();
    private NetworkVariable<ulong> targetPlayerId = new NetworkVariable<ulong>(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
    private EnemyStats enemyStats;
    public Transform target;

    void Start()
    {
        animator = GetComponent<Animator>();
        originalScale = transform.localScale;
        enemyStats = GetComponent<EnemyStats>();

        abilities.AddRange(GetComponents<IEnemyAbility>());

        if (GameState.IsSinglePlayer)
        {
            // Handle single-player logic
            AssignTargetInSinglePlayer();
        }
        else
        {
            // Multiplayer logic, make sure to handle target updates for networked players
            if (IsServer)
            {
                InvokeRepeating(nameof(UpdateTarget), 0f, 1f);
            }

            targetPlayerId.OnValueChanged += (oldValue, newValue) => AssignTargetFromNetworkId();
        }
    }

    void Update()
    {
        if (GameState.IsSinglePlayer)
        {
            // In single-player, retry assigning the target until it's found
            if (target == null)
            {
                AssignTargetInSinglePlayer();
                return;
            }
        }
        else
        {
            if (target == null && IsClient)
            {
                AssignTargetFromNetworkId();
            }
        }

        if ((IsServer || GameState.IsSinglePlayer) && target != null)
        {
            float distanceToPlayer = Vector2.Distance(transform.position, target.position);

            if (distanceToPlayer > enemyStats.chaseRange || !HasLineOfSight())
            {
                target = null;
                animator.SetTrigger("idle");
                return;
            }

            foreach (var ability in abilities)
            {
                ability.Execute(this, target);
            }

            if (distanceToPlayer <= enemyStats.attackRange)
            {
                // Handle melee attack
            }
            else if (distanceToPlayer <= enemyStats.projectileRange)
            {
                // Handle ranged attack
            }
            else
            {
                animator.SetTrigger("walk");
                transform.position = Vector2.MoveTowards(transform.position, target.position, enemyStats.moveSpeed * Time.deltaTime);
                FlipSprite();
            }
        }
    }


    void UpdateTarget()
    {
        if (!IsServer || GameState.IsSinglePlayer) return;

        float closestDistance = Mathf.Infinity;
        Transform closestPlayer = null;
        ulong closestPlayerId = 0;

        foreach (NetworkObject obj in NetworkManager.Singleton.SpawnManager.SpawnedObjectsList)
        {
            if (obj == null || !obj.IsSpawned) continue;

            if (obj.TryGetComponent(out PlayerController player))
            {
                float distance = Vector2.Distance(transform.position, obj.transform.position);

                if (distance < closestDistance && distance <= enemyStats.chaseRange)
                {
                    closestDistance = distance;
                    closestPlayer = obj.transform;
                    closestPlayerId = obj.NetworkObjectId;
                }
            }
        }

        if (closestPlayer != null)
        {
            //Debug.Log($"[SERVER] Closest player: {closestPlayer.name} (ID: {closestPlayerId})");
            target = closestPlayer;
            targetPlayerId.Value = closestPlayerId;
        }
        else
        {
            //Debug.Log("[SERVER] No player found in range.");
        }
    }

    void AssignTargetFromNetworkId()
    {
        if (targetPlayerId.Value == 0)
        {
            //Debug.Log("[CLIENT] No targetPlayerId set.");
            return;
        }

        if (NetworkManager.Singleton.SpawnManager.SpawnedObjects.TryGetValue(targetPlayerId.Value, out NetworkObject playerObject))
        {
            target = playerObject.transform;
            //Debug.Log($"[CLIENT] Assigned player target: {playerObject.name}");
        }
        else
        {
            //Debug.LogWarning($"[CLIENT] Could not find NetworkObject with ID {targetPlayerId.Value}");
        }
    }

    // Handle target assignment manually for single-player
    void AssignTargetInSinglePlayer()
    {
        if (target == null)
        {
            GameObject playerObject = GameObject.FindWithTag("Player");  // Assuming player has the "Player" tag
            if (playerObject != null)
            {
                target = playerObject.transform;
                //Debug.Log("[SINGLE PLAYER] Assigned target from player object");
            }
            else
            {
                //Debug.LogWarning("[SINGLE PLAYER] No player found to assign target");
            }
        }
    }

    bool HasLineOfSight()
    {
        if (target == null) return false;

        Vector2 origin = transform.position;
        Vector2 direction = (target.position - transform.position).normalized;
        float distance = Vector2.Distance(transform.position, target.position);

        RaycastHit2D hit = Physics2D.Raycast(origin, direction, distance, LayerMask.GetMask("Player", "Obstacles"));

        return hit.collider != null && hit.collider.CompareTag("Player");
    }

    void FlipSprite()
    {
        if (target == null) return;

        Vector3 scale = originalScale;
        scale.x *= target.position.x < transform.position.x ? -1 : 1;
        transform.localScale = scale;
    }
}
