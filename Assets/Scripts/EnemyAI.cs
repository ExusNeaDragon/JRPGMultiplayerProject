using UnityEngine;
using System.Collections.Generic;
using Unity.Netcode;

public class EnemyAI : NetworkBehaviour
{
    public enum EnemyType { Melee, Ranged };
    public EnemyType enemyType;
    public float moveSpeed = 3f;
    public int attackDamage = 10;
    public float attackRange = 1.5f;
    public float chaseRange = 7f;

    public float projectileRange = 5f;
    public GameObject projectilePrefab;
    public Transform firePoint;

    private PlayerStats playerStats;
    private Vector3 originalScale;
    private List<IEnemyAbility> abilities = new List<IEnemyAbility>();
    private NetworkVariable<ulong> targetPlayerId = new NetworkVariable<ulong>(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);


    private bool hasNetworkManager;
    public Transform target;

    void Start()
    {
        originalScale = transform.localScale;
        hasNetworkManager = (NetworkManager.Singleton != null);
        abilities.AddRange(GetComponents<IEnemyAbility>());

        if (!hasNetworkManager || IsServer)
        {
            InvokeRepeating(nameof(UpdateTarget), 0f, 1f);
        }

        if (hasNetworkManager)
        {
            targetPlayerId.OnValueChanged += (oldValue, newValue) => AssignTargetFromNetworkId();
        }
    }

    void AssignTargetFromNetworkId()
    {
        if (targetPlayerId.Value == 0) return; // No valid target assigned

        if (NetworkManager.Singleton.SpawnManager.SpawnedObjects.TryGetValue(targetPlayerId.Value, out NetworkObject playerObject))
        {
            target = playerObject.transform;
            Debug.Log($"[EnemyAI] Client assigned target: {target.name} (ID: {targetPlayerId.Value})");
        }
        else
        {
            Debug.LogWarning($"[EnemyAI] Failed to find player with ID {targetPlayerId.Value}");
        }
    }

    void Update()
    {
        if (target == null && IsClient) 
        {
            AssignTargetFromNetworkId(); // Ensure clients always assign target
        }

        if (IsServer || !hasNetworkManager) 
        {
            if (target == null) return;

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
            else{
                if (HasLineOfSight() && distanceToPlayer <= chaseRange){
                    transform.position = Vector2.MoveTowards(transform.position, target.position, moveSpeed * Time.deltaTime);
                    FlipSprite();
                }
            }
        }
    }


    void UpdateTarget()
    {
        if (hasNetworkManager && !IsServer) return;

        PlayerController[] players = FindObjectsByType<PlayerController>(FindObjectsSortMode.None);
        Debug.Log($"[EnemyAI] Found {players.Length} players.");
        
        float closestDistance = Mathf.Infinity;
        Transform closestPlayer = null;
        ulong closestPlayerId = 0;

        foreach (var player in players)
        {
            float distance = Vector2.Distance(transform.position, player.transform.position);
            Debug.Log($"[EnemyAI] Checking player {player.name}, Distance: {distance}");

            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestPlayer = player.transform;
                closestPlayerId = player.GetComponent<NetworkObject>().NetworkObjectId;
            }
        }

        if (closestPlayer != null)
        {
            target = closestPlayer;

            if (hasNetworkManager && IsServer)
            {
                // Sync only if in networked mode
                targetPlayerId.Value = closestPlayerId;
                Debug.Log($"[EnemyAI] Server assigned target: {closestPlayer.name} (ID: {closestPlayerId})");
            }
            else
            {
                Debug.Log($"[EnemyAI] Single-player target assigned: {closestPlayer.name}");
            }
        }

    }

    bool HasLineOfSight()
    {
        if (target == null) return false;

        Vector2 origin = transform.position;
        Vector2 direction = (target.position - transform.position).normalized;
        float distance = Vector2.Distance(transform.position, target.position);

        RaycastHit2D hit = Physics2D.Raycast(origin, direction, distance, LayerMask.GetMask("Player", "Obstacles")); // Add walls/obstacles layers

        if (hit.collider != null && hit.collider.CompareTag("Player"))
        {
            return true;
        }

        return false;
    }

    void FlipSprite()
    {
        if (target == null) return;

        Vector3 scale = originalScale;
        scale.x *= target.position.x < transform.position.x ? -1 : 1;
        transform.localScale = scale;
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
            rb.linearVelocity = direction * 5f; // Fixed: use velocity instead of linearVelocity
        }
    }
}
