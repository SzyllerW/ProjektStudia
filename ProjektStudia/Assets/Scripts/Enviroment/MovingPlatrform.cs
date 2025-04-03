using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class MovingPlatform : MonoBehaviour
{
    public Transform startTransform;
    public Transform endTransform;
    public float speed = 2f;

    private bool movingToEnd = true;
    private Rigidbody2D rb;
    private Vector2 previousPosition;

    public Vector2 Velocity { get; private set; }

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.bodyType = RigidbodyType2D.Kinematic;
        rb.interpolation = RigidbodyInterpolation2D.Interpolate;

        if (startTransform != null)
        {
            transform.position = startTransform.position;
            previousPosition = transform.position;
        }
    }

    void FixedUpdate()
    {
        if (startTransform == null || endTransform == null) return;

        Vector2 targetPosition = (movingToEnd ? endTransform : startTransform).position;
        Vector2 newPosition = Vector2.MoveTowards(rb.position, targetPosition, speed * Time.fixedDeltaTime);

        Velocity = (newPosition - previousPosition) / Time.fixedDeltaTime;
        previousPosition = newPosition;

        rb.MovePosition(newPosition);

        if (Vector2.Distance(rb.position, targetPosition) < 0.01f)
        {
            movingToEnd = !movingToEnd;
        }
    }
}

