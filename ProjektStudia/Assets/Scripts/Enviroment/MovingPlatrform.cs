using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(BoxCollider2D))]
public class MovingPlatform : MonoBehaviour
{
    [Header("Platform Points")]
    public Transform startPoint;
    public Transform endPoint;

    [Header("Movement")]
    public float speed = 2f;

    private Vector3 target;
    private bool goingToEnd = true;

    private void Start()
    {
        if (startPoint == null || endPoint == null)
        {
            Debug.LogError("MovingPlatform: startPoint or endPoint not assigned.");
            enabled = false;
            return;
        }

        transform.position = startPoint.position;
        target = endPoint.position;

        // Ustaw collider jako trigger do wykrywania gracza
        BoxCollider2D col = GetComponent<BoxCollider2D>();
        col.isTrigger = true;
    }

    private void FixedUpdate()
    {
        Vector3 direction = (target - transform.position).normalized;
        transform.position = Vector3.MoveTowards(transform.position, target, speed * Time.fixedDeltaTime);

        if (Vector2.Distance(transform.position, target) < 0.05f)
        {
            transform.position = target;
            goingToEnd = !goingToEnd;
            target = goingToEnd ? endPoint.position : startPoint.position;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            other.transform.SetParent(transform);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            other.transform.SetParent(null);
        }
    }
}

