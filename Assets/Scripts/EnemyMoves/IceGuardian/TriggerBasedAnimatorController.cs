using UnityEngine;

public class TriggerBasedAnimatorController : MonoBehaviour
{
    public Animator animator;  // Assign parent animator in Inspector
    private bool animationStarted = false;
    private bool animationCompleted = false;

    private void Start()
    {
        if (animator != null)
            animator.speed = 0f;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !animationStarted)
        {
            animationStarted = true;
            animator.speed = 1f;
            animator.Play("reanimate", 0, 0);
        }
    }

    private void Update()
    {
        if (animationStarted && !animationCompleted)
        {
            AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);

            if (stateInfo.IsName("reanimate") && stateInfo.normalizedTime >= 1f)
            {
                animationCompleted = true;
                this.enabled = false;  // Disable this script
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !animationStarted)
        {
            animator.speed = 0f;
        }
    }
}
