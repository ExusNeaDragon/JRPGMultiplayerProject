using UnityEngine;
using System.Collections;
using Unity.Netcode;

public class PlayerCombat : NetworkBehaviour
{
    private PlayerStats playerStats;
    public Animator animator;

    [Header("Abilities")]
    public Ability leftClickAbility;
    public Ability rightClickAbility;

    private void Awake()
    {
        playerStats = GetComponent<PlayerStats>();
    }

    void Update()
    {
        // Prevent non-owners from processing actions in a networked game.
        if (IsNetworkedGame() && !IsOwner) return;

        HandleInput();
    }

    // Check if the game is networked and we are playing as a networked client.
    private bool IsNetworkedGame()
    {
        return NetworkManager.Singleton != null && NetworkManager.Singleton.IsListening;
    }

    // Handle input actions for left-click, right-click, and interact.
    private void HandleInput()
    {
        if (Input.GetMouseButtonDown(0)) // Left Click
            HandleAttack(0);

        if (Input.GetMouseButtonDown(1)) // Right Click
            HandleAttack(1);

        if (Input.GetKeyDown(KeyCode.F)) // Interact
            Interact();
    }

    // Handle the attack based on the clicked button (0 for left click, 1 for right click).
    private void HandleAttack(int button)
    {
        if (IsNetworkedGame())
            AttackServerRpc(button); // Multiplayer
        else
            AttackLocal(button); // Singleplayer
    }

    // Activate the ability locally (singleplayer).
    private void AttackLocal(int button)
    {
        Ability ability = button == 0 ? leftClickAbility : rightClickAbility;
        ability?.Activate();
    }

    // Server RPC to activate attack abilities in multiplayer.
    [ServerRpc]
    private void AttackServerRpc(int button)
    {
        Ability ability = button == 0 ? leftClickAbility : rightClickAbility;
        ability?.Activate();
    }

    // Handle the revival interaction when the player presses F.
    private void Interact()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, 2f); // 2f revive radius

        foreach (var hit in hits)
        {
            if (hit.gameObject == this.gameObject) continue;

            PlayerStats targetStats = hit.GetComponent<PlayerStats>();
            if (targetStats != null && targetStats.isDead.Value)
            {
                StartCoroutine(ReviveRoutine(targetStats));
                break;
            }
        }
    }

    // Coroutine that handles the revive process by holding F key.
    private IEnumerator ReviveRoutine(PlayerStats target)
    {
        float holdTime = 2f;
        float timer = 0f;

        Debug.Log("Hold F to revive...");

        while (Input.GetKey(KeyCode.F) && timer < holdTime)
        {
            timer += Time.deltaTime;
            yield return null;
        }

        if (timer >= holdTime)
        {
            Debug.Log("Revive triggered");
            RevivePlayerServerRpc(target.NetworkObjectId);
        }
    }

    // Server RPC to handle the revival of a player.
    [ServerRpc]
    private void RevivePlayerServerRpc(ulong targetId)
    {
        NetworkObject targetObj = NetworkManager.Singleton.SpawnManager.SpawnedObjects[targetId];
        PlayerStats stats = targetObj.GetComponent<PlayerStats>();

        if (stats != null && stats.isDead.Value)
        {
            stats.currentHealth.Value = stats.maxHealth.Value;
            stats.isDead.Value = false;

            stats.GetComponent<PlayerController>().UnfreezeMovement();
            Debug.Log("Player revived!");
        }
    }
}
