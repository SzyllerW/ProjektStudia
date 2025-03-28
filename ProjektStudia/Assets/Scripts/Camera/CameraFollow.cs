using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    private Transform player;
    [SerializeField] private float smoothSpeed = 0.125f;
    [SerializeField] private Vector2 offset = new Vector2(3f, 2f);
    [SerializeField] private float stopFollowingY = -70f;

    private bool isShaking = false;
    private Vector3 lastKnownPosition;

    private void Start()
    {
        FindPlayer();
        if (player != null)
        {
            lastKnownPosition = transform.position;
        }
    }

    private void LateUpdate()
    {
        if (isShaking) return;

        if (player == null || !player.gameObject.activeSelf || player.position.y <= stopFollowingY)
        {
            FindPlayer();
            if (player == null)
            {
                transform.position = Vector3.Lerp(transform.position, lastKnownPosition, smoothSpeed);
            }
            return;
        }

        Vector3 targetPosition = new Vector3(
            player.position.x + (player.localScale.x > 0 ? offset.x : -offset.x),
            player.position.y + offset.y,
            transform.position.z
        );

        lastKnownPosition = targetPosition;
        transform.position = Vector3.Lerp(transform.position, targetPosition, smoothSpeed);
    }

    public void ShakeBeforeFollow(float duration, float magnitude)
    {
        if (!isShaking)
        {
            StartCoroutine(Shake(duration, magnitude));
        }
    }

    private System.Collections.IEnumerator Shake(float duration, float magnitude)
    {
        isShaking = true;
        Vector3 originalPosition = lastKnownPosition;

        float elapsed = 0.0f;

        while (elapsed < duration)
        {
            float x = Random.Range(-1f, 1f) * magnitude;
            float y = Random.Range(-1f, 1f) * magnitude;

            transform.position = originalPosition + new Vector3(x, y, 0);

            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.position = originalPosition;
        isShaking = false;
    }

    private void FindPlayer()
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        foreach (GameObject p in players)
        {
            if (p.activeSelf && p.transform.position.y > stopFollowingY)
            {
                player = p.transform;
                return;
            }
        }
        player = null;
    }
}
