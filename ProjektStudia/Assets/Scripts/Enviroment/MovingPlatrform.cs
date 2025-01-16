using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    public Transform startTransform;
    public Transform endTransform;
    public float speed = 2f;

    private bool movingToEnd = true;

    private void Start()
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

    private void Update()
    {
        if (startTransform == null || endTransform == null)
        {
            Debug.LogError("[MovingPlatform] Start or End transform is not set.");
            return;
        }

        MovePlatform(startTransform.position, endTransform.position);
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

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Player"))
        {
            Debug.Log("Player attached to platform.");
            collision.collider.transform.SetParent(transform); 
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Player"))
        {
            Debug.Log("Player detached from platform.");
            collision.collider.transform.SetParent(null);
        }
    }
}