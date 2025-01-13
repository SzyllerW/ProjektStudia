using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    public Transform startTransform;
    public Transform endTransform;
    public float speed = 2f;
    public MovementMode movementMode = MovementMode.Horizontal;

    private bool movingToEnd = true;

    public enum MovementMode
    {
        Horizontal,
        Vertical
    }

    void Start()
    {
        if (startTransform != null)
        {
            transform.position = startTransform.position;
        }
        else
        {
            Debug.LogError("[MovingPlatform] Start transform is not set.");
        }
    }

    void Update()
    {
        if (startTransform == null || endTransform == null)
        {
            Debug.LogError("[MovingPlatform] Start or End transform is not set.");
            return;
        }

        if (movementMode == MovementMode.Horizontal)
        {
            MovePlatform(startTransform.position, endTransform.position);
        }
        else if (movementMode == MovementMode.Vertical)
        {
            MovePlatform(startTransform.position, endTransform.position);
        }
    }

    private void MovePlatform(Vector3 start, Vector3 end)
    {
        Vector3 targetPosition = movingToEnd ? end : start;
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime);

        if (Vector3.Distance(transform.position, targetPosition) < 0.01f)
        {
            movingToEnd = !movingToEnd; 
        }
    }
}