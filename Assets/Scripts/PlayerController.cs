using UnityEngine;
using Unity.Netcode;

public class PlayerController : NetworkBehaviour
{
    public float moveSpeed = 5f;
    private Rigidbody2D rb;
    private Vector2 moveInput;
    private PlayerStats playerStats;
    private PlayerCombat playerCombat;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        playerStats = GetComponent<PlayerStats>();
        playerCombat = GetComponent<PlayerCombat>();
    }

    void Update()
    {
        if (!IsOwner) return; // Ensure only the local player controls movement

        moveInput.x = Input.GetAxis("Horizontal");
        moveInput.y = Input.GetAxis("Vertical");

        if (Input.GetMouseButtonDown(0)) // Left click to attack
        {
            playerCombat.AttackServerRpc();
        }
    }

    void FixedUpdate()
    {
        if (!IsOwner) return;
        rb.linearVelocity = moveInput.normalized * moveSpeed;
    }
}