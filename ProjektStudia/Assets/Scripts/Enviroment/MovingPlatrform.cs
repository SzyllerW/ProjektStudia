using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
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
    }

    private void FixedUpdate()
    {
        Vector3 direction = (target - transform.position).normalized;
        transform.position += direction * speed * Time.fixedDeltaTime;

        if (Vector2.Distance(transform.position, target) < 0.05f)
        {
            goingToEnd = !goingToEnd;
            target = goingToEnd ? endPoint.position : startPoint.position;
        }
    }
}

