using UnityEngine;

public class Leap : MonoBehaviour, IEnemyAbility
{
    public float leapRange = 3f;
    public float leapForce = 10f;
    public float leapHeight = 5f;
    private bool isLeaping = false;
    private Rigidbody2D rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    public void Execute(EnemyAI enemy)
    {
        if (enemy == null || enemy.IsOwner == false || isLeaping) return;

        float distanceToTarget = Vector2.Distance(enemy.transform.position, enemy.target.position);

        if (distanceToTarget <= leapRange)
        {
            Debug.Log($"{enemy.name} is leaping!");
            isLeaping = true;

            Vector2 direction = (enemy.target.position - enemy.transform.position).normalized;
            Vector2 leapVelocity = new Vector2(direction.x * leapForce, leapHeight);

            rb.linearVelocity= leapVelocity; // Apply jump force
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("TransparentFX") || collision.gameObject.CompareTag("Player"))
        {
            isLeaping = false; // Reset when landing
        }
    }

    public void Execute(EnemyAI enemy, Transform player) { }
}
