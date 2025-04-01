using UnityEngine;
using System.Collections.Generic;
using Unity.Netcode;

public class EnemyAI : NetworkBehaviour
{
    public enum EnemyType{Melee,Ranged};
    public EnemyType enemyType;
    public float moveSpeed = 3f;
    public int attackDamage = 10;
    public float attackRange = 1.5f;
    public float projectileRange = 5f;
    public GameObject projectilePrefab;
    public Transform firePoint;

    private PlayerStats playerStats;
    private List<IEnemyAbility> abilities = new List<IEnemyAbility>();

    private NetworkVariable<ulong> targetPlayerId = new NetworkVariable<ulong>(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
    public Transform target;

    void Start()
    {
        abilities.AddRange(GetComponents<IEnemyAbility>());
        
        if (IsServer || !NetworkManager.Singleton.IsConnectedClient) // Works for LAN host & single-player
        {
            InvokeRepeating(nameof(UpdateTarget), 0f, 1f); // Update target every second
        }
    }

    void Update()
    {
        if (IsServer || !NetworkManager.Singleton.IsConnectedClient) // Runs if host or in single-player
        {
            if (target == null)
            {
                AssignTargetFromNetworkId();
                return;
            }

            float distanceToPlayer = Vector2.Distance(transform.position, target.position);

            foreach (var ability in abilities)
            {
                ability.Execute(this);
            }

            if (enemyType == EnemyType.Melee && distanceToPlayer <= attackRange)
            {
                AttackMelee();
            }
            else if (enemyType == EnemyType.Ranged && distanceToPlayer <= projectileRange)
            {
                AttackRanged();
            }
            else
            {
                // Move enemy only if it's the server, or in single-player mode
                transform.position = Vector2.MoveTowards(transform.position, target.position, moveSpeed * Time.deltaTime);
            }
        }
    }

    void UpdateTarget()
    {
        PlayerController[] players = FindObjectsByType<PlayerController>(FindObjectsSortMode.None);
        float closestDistance = Mathf.Infinity;
        Transform closestPlayer = null;
        ulong closestPlayerId = 0;

        foreach (var player in players)
        {
            float distance = Vector2.Distance(transform.position, player.transform.position);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestPlayer = player.transform;

                if (NetworkManager.Singleton.IsConnectedClient) // If in LAN, get the player's NetworkObjectId
                {
                    closestPlayerId = player.GetComponent<NetworkObject>().NetworkObjectId;
                }
            }
        }

        if (closestPlayer != null)
        {
            target = closestPlayer;

            if (IsServer && NetworkManager.Singleton.IsConnectedClient) // Sync target only if multiplayer
            {
                targetPlayerId.Value = closestPlayerId;
            }
        }
    }

    void AssignTargetFromNetworkId()
    {
        if (!NetworkManager.Singleton.IsConnectedClient || targetPlayerId.Value == 0) return;

        if (NetworkManager.Singleton.SpawnManager.SpawnedObjects.TryGetValue(targetPlayerId.Value, out NetworkObject playerObject))
        {
            target = playerObject.transform;
        }
    }

    void AttackMelee()
    {
        Debug.Log("Melee enemy attacking!");
        if (target != null)
        {
            playerStats = target.GetComponent<PlayerStats>();
            if (playerStats != null)
            {
                playerStats.TakeDamage(attackDamage);
            }
        }
    }

    void AttackRanged()
    {
        Debug.Log("Ranged enemy firing projectile!");
        if (projectilePrefab != null && firePoint != null)
        {
            GameObject projectile = Instantiate(projectilePrefab, firePoint.position, firePoint.rotation);
            Rigidbody2D rb = projectile.GetComponent<Rigidbody2D>();
            Vector2 direction = (target.position - firePoint.position).normalized;
            rb.linearVelocity = direction * 5f; // Fixed: use velocity, not linearVelocity
        }
    }
}
