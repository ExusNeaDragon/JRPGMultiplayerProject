using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target;
    public Vector3 offset;
    public float smoothSpeed = 0.125f;

    void FixedUpdate()
    {
        if (target != null)
        {
            Vector3 desiredPosition = target.position + offset;
            Vector3 smoothed = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
            transform.position = smoothed;
        }
    }

    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
    }
}
