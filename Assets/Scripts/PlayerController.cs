using UnityEngine;
using Unity.Netcode;

public class PlayerController : NetworkBehaviour
{
    public float moveSpeed = 5f;
    private Rigidbody2D rb;
    private Vector2 moveInput;
    private PlayerStats playerStats;
    private PlayerCombat playerCombat;
    public bool canMove = true;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        playerStats = GetComponent<PlayerStats>();
        playerCombat = GetComponent<PlayerCombat>();
    }

    private void Update()
    {
        if (!IsControllingPlayer()||!canMove) return; // Ensure only the local player processes input

        // Get movement input
        moveInput.x = Input.GetAxis("Horizontal");
        moveInput.y = Input.GetAxis("Vertical");

        // Handle attack input (left mouse button)
        if (Input.GetMouseButtonDown(0))
        {
            if (IsOwner) // Only the owner can attack
            {
                playerCombat.AttackServerRpc();
            }
        }
    }
    public void FreezeMovement()
    {
        canMove = false;
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
            rb.constraints = RigidbodyConstraints2D.FreezeAll;
        }
    }
    public void UnfreezeMovement()
    {
        canMove = true;
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.constraints = RigidbodyConstraints2D.FreezeRotation;
        }
    }
    

    private void FixedUpdate()
    {
        if (!IsControllingPlayer()) return;

        if (IsControllingPlayer())
        {
            // Update movement
            rb.bodyType = RigidbodyType2D.Dynamic;
            rb.linearVelocity = moveInput.normalized * moveSpeed;
        }
    }

    // Helper method to decide if this player can control the character
    private bool IsControllingPlayer()
    {
        // Single-player mode: No NetworkManager, directly control the player
        if (NetworkManager.Singleton == null || !NetworkManager.Singleton.IsListening)
        {
            return true; // Always control the player in single-player
        }

        // In multiplayer mode, only the owner can control their player
        return IsOwner;
    }
}
