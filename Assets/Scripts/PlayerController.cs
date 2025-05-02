using UnityEngine;
using Unity.Netcode;

public class PlayerController : NetworkBehaviour
{
    public float moveSpeed = 5f;
    private Rigidbody2D rb;
    private Vector2 moveInput;
    public bool canMove = true;
    public SpriteRenderer visual;
    private Animator animator;

    // NetworkVariable to sync flipX across clients
    public NetworkVariable<bool> isFlipped = new NetworkVariable<bool>(
        false,
        NetworkVariableReadPermission.Everyone,
        NetworkVariableWritePermission.Owner
    );

    private void Start()
    {
        animator = visual.GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
    }

    public override void OnNetworkSpawn()
    {
        // Apply the current flipX state on spawn
        visual.flipX = isFlipped.Value;

        // Subscribe to changes for non-owners
        isFlipped.OnValueChanged += OnFlipChanged;
    }

    public override void OnNetworkDespawn()
    {
        isFlipped.OnValueChanged -= OnFlipChanged;
    }

    private void OnFlipChanged(bool previousValue, bool newValue)
    {
        if (!IsOwner)
        {
            visual.flipX = newValue;
        }
    }

    private void Update()
    {
        // Allow local player input OR fallback for offline (local) mode
        if (!IsControllingPlayer() || !canMove) return;

        moveInput.x = Input.GetAxis("Horizontal");
        moveInput.y = Input.GetAxis("Vertical");
    }

    private void FixedUpdate()
    {
        if (!IsControllingPlayer()) return;

        bool isMoving = moveInput.magnitude > 0.1f;

        if (animator != null)
            animator.SetBool("run", isMoving);

        // Determine local direction
        bool flipped = moveInput.x < 0;

        // Multiplayer: update shared flipX state
        if (IsNetworkActive())
        {
            if (Mathf.Abs(moveInput.x) > 0.01f)
                isFlipped.Value = flipped;

            // Apply flip locally
            visual.flipX = isFlipped.Value;
        }
        else
        {
            // Local mode only
            if (Mathf.Abs(moveInput.x) > 0.01f)
                visual.flipX = flipped;
        }

        rb.bodyType = RigidbodyType2D.Dynamic;
        rb.linearVelocity = moveInput.normalized * moveSpeed;
    }

    public void FreezeMovement()
    {
        canMove = false;
        rb.linearVelocity = Vector2.zero;
        rb.constraints = RigidbodyConstraints2D.FreezeAll;
    }

    public void UnfreezeMovement()
    {
        canMove = true;
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;
    }

    private bool IsControllingPlayer()
    {
        return !IsNetworkActive() || IsOwner;
    }

    private bool IsNetworkActive()
    {
        return NetworkManager.Singleton != null && NetworkManager.Singleton.IsListening;
    }
}
