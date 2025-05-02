using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float speed = 10f;
    public float lifetime = 5f;
    public LayerMask wallLayer;
    private Vector2 moveDirection;

    public void SetDirection(Vector2 dir)
    {
        moveDirection = dir.normalized;
        float angle = Mathf.Atan2(moveDirection.y, moveDirection.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle);
    }

    void Update()
    {
        transform.Translate(moveDirection * speed * Time.deltaTime, Space.World);
    }

    void Start()
    {
        Destroy(gameObject, lifetime); // Auto destroy after lifetime
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (((1 << collision.gameObject.layer) & wallLayer) != 0)
        {
            Destroy(gameObject); // Destroy when hitting a wall
        }

        // Optionally, if you want projectiles to hit enemies too:
        if (collision.CompareTag("Enemy"))
        {
            Destroy(gameObject); // or deal damage etc.
        }
    }
}
