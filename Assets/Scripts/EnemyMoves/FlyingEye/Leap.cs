using UnityEngine;

public class Leap : MonoBehaviour, IEnemyAbility
{
    public float leapRange = 3f;
    public float leapForce = 10f;
    private float cooldown = 5f;
    private float lastLeapTime = -Mathf.Infinity;
    private bool isLeaping = false;
    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;
    public IEnemyAbility.EnemyType Type => IEnemyAbility.EnemyType.SpecialActive;
    private Transform spriteVisual; // child object to fake vertical motion
    private Animator animator;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        spriteVisual = transform.Find("SpriteVisual"); // Create a child GameObject named "SpriteVisual"
    }

    public void Execute(EnemyAI enemy)
    {
        if (enemy == null || !enemy.IsOwner || isLeaping) return;
        float timeSinceLastLeap = Time.time - lastLeapTime;
        if (timeSinceLastLeap < cooldown) return;

        float distanceToTarget = Vector2.Distance(enemy.transform.position, enemy.target.position);
        if (distanceToTarget > leapRange) return;
        this.animator.SetTrigger("fly");
        Debug.Log($"{enemy.name} leaps toward the player!");
        lastLeapTime = Time.time;
        isLeaping = true;

        // Direction on X axis only
        Vector2 direction = (enemy.target.position - enemy.transform.position).normalized;
        Vector2 leapVelocity = new Vector2(direction.x * leapForce, 0f);

        // Apply movement
        rb.linearVelocity = leapVelocity;

        // Flip sprite based on direction
        Vector3 scale = enemy.transform.localScale;
        scale.x = direction.x < 0 ? -1 : 1;
        enemy.transform.localScale = scale;

        // Trigger animation
        Animator animator = enemy.GetComponent<Animator>();
        if (animator != null) animator.SetTrigger("Leap");

        // Fake arc movement
        if (spriteVisual != null)
        {
            StopAllCoroutines();
            StartCoroutine(FakeArc(spriteVisual, 0.3f, 0.5f));
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Wall") ||
            collision.gameObject.CompareTag("Player") ||
            collision.gameObject.CompareTag("Floor"))
        {
            isLeaping = false;
        }
    }

    // Simulate a fake jump arc by changing local Y position
    private System.Collections.IEnumerator FakeArc(Transform visual, float height, float duration)
    {
        float elapsed = 0f;
        Vector3 originalPos = visual.localPosition;

        while (elapsed < duration)
        {
            float progress = elapsed / duration;
            float arcHeight = Mathf.Sin(Mathf.PI * progress) * height; // smooth up and down
            visual.localPosition = new Vector3(originalPos.x, originalPos.y + arcHeight, originalPos.z);
            elapsed += Time.deltaTime;
            yield return null;
        }

        visual.localPosition = originalPos;
    }

    public void Execute(EnemyAI enemy, Transform player) { }
}
