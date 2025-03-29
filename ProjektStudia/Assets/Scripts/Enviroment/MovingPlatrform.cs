using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    public Transform startTransform;
    public Transform endTransform;
    public float speed = 2f;

    private bool movingToEnd = true;
    private Rigidbody2D rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        if (startTransform != null)
        {
            transform.position = startTransform.position;
        }
    }

    private void FixedUpdate()
    {
        if (startTransform == null || endTransform == null) return;

        Vector3 targetPosition = movingToEnd ? endTransform.position : startTransform.position;
        Vector3 newPosition = Vector3.MoveTowards(transform.position, targetPosition, speed * Time.fixedDeltaTime);

        rb.MovePosition(newPosition);

        if (Vector3.Distance(transform.position, targetPosition) < 0.01f)
        {
            movingToEnd = !movingToEnd;
        }
    }
}
