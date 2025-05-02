using UnityEngine;
using Unity.Netcode;
using System.Collections;

public class EnemyProjectile : NetworkBehaviour
{
    private Vector2 moveDirection;
    private int damage;
    private float speed = 5f;
    private bool hasExploded = false;
    private Animator animator;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    public void Initialize(Vector2 targetPosition, int damageAmount)
    {
        //Debug.Log(targetPosition);
        // Calculate direction to move in
        moveDirection = (targetPosition - (Vector2)transform.position).normalized;
        damage = damageAmount;
        float angle = Mathf.Atan2(moveDirection.y, moveDirection.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0f, 0f, angle);

        // Self-destruct failsafe
        Invoke(nameof(ExplodeAndDestroy), 5f);
    }

    void Update()
    {
        bool isNetworked = NetworkManager.Singleton != null && NetworkManager.Singleton.IsListening;

        if (isNetworked && (!IsOwner || hasExploded)) return;
        if (!isNetworked && hasExploded) return;

        transform.position += (Vector3)(moveDirection * speed * Time.deltaTime);
        //Debug.Log("Fireball moving: " + moveDirection);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        bool isNetworked = NetworkManager.Singleton != null && NetworkManager.Singleton.IsListening;

        if (isNetworked && (!IsOwner || hasExploded)) return;
        if (!isNetworked && hasExploded) return;
        
        if (other.CompareTag("Player"))
        {
            //Debug.Log(damage);
            other.GetComponent<PlayerStats>()?.TakeDamage(damage);
            ExplodeAndDestroy();
        }else if(other.CompareTag("Wall")){
            ExplodeAndDestroy();
        }
    }

    public void DestroyProjectile()
    {
        Destroy(gameObject);
    }

    private void ExplodeAndDestroy()
    {
        if (hasExploded) return;
        hasExploded = true;

        if (animator != null)
        {
            animator.SetTrigger("Explosion");
        }

        StartCoroutine(DestroyAfterAnimation());
    }

    private IEnumerator DestroyAfterAnimation()
    {
        yield return new WaitForSeconds(0.5f);
        Destroy(gameObject);
    }
}
