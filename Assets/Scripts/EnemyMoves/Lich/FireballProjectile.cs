using UnityEngine;
using Unity.Netcode;

public class FireballProjectile : NetworkBehaviour
{
    private Vector2 targetPosition;
    private int damage;
    private float speed = 5f;

    public void Initialize(Vector2 target, int damageAmount)
    {
        targetPosition = target;
        damage = damageAmount;
        Destroy(gameObject, 5f); // Destroy after 5 seconds if it doesn't hit anything
    }

    void Update()
    {
        if (!IsServer) return; // Ensure only the server handles movement

        transform.position = Vector2.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime);

        if ((Vector2)transform.position == targetPosition)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            other.GetComponent<PlayerStats>()?.TakeDamage(damage);
            Destroy(gameObject);
        }
    }
}
