using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    private Transform player;
    [SerializeField] private float smoothSpeed = 0.125f;
    [SerializeField] private Vector2 baseOffset = new Vector2(3f, 2f);
    [SerializeField] private float stopFollowingY = -70f;

    [Header("Zoom Settings")]
    [SerializeField] private float baseSize = 126f;
    [SerializeField] private float jumpZoomSize = 132f;
    [SerializeField] private float idleZoomSize = 140f;
    [SerializeField] private float zoomOutSpeed = 0.3f;
    [SerializeField] private float zoomInSpeed = 5f;

    private Vector3 lastKnownPosition;
    private bool isShaking = false;

    [SerializeField] private float idleThreshold = 2f;
    private float idleTime = 0f;
    private Rigidbody2D playerRb;

    private void Start()
    {
        FindPlayer();
        if (player != null)
        {
            lastKnownPosition = transform.position;
            playerRb = player.GetComponent<Rigidbody2D>();
        }

        Camera.main.orthographicSize = baseSize;
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

        if (playerRb == null)
            playerRb = player.GetComponent<Rigidbody2D>();

        float horizontalVelocity = Mathf.Abs(playerRb.velocity.x);
        float verticalVelocity = playerRb.velocity.y;

        float targetSize = baseSize;

        if (!IsGrounded() && Mathf.Abs(verticalVelocity) > 0.1f)
        {
            targetSize = jumpZoomSize;
            idleTime = 0f;
        }
        else if (horizontalVelocity < 0.05f && Mathf.Abs(verticalVelocity) < 0.05f)
        {
            idleTime += Time.deltaTime;
            if (idleTime > idleThreshold)
            {
                targetSize = idleZoomSize;
            }
        }
        else
        {
            idleTime = 0f;
        }

        float currentZoom = Camera.main.orthographicSize;
        float zoomSpeed = (targetSize > currentZoom) ? zoomOutSpeed : zoomInSpeed;

        Camera.main.orthographicSize = Mathf.Lerp(currentZoom, targetSize, Time.deltaTime * zoomSpeed);

        Vector3 targetPosition = new Vector3(
            player.position.x + (player.localScale.x > 0 ? baseOffset.x : -baseOffset.x),
            player.position.y + baseOffset.y,
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
                playerRb = player.GetComponent<Rigidbody2D>();

                if (player.GetComponent<PlayerMovement>() == null)
                {
                    Debug.LogWarning("Gracz znaleziony, ale nie ma PlayerMovement!");
                }

                return;
            }
        }

        player = null;
        playerRb = null;
    }

    private bool IsGrounded()
    {
        if (player == null) return false;

        PlayerMovement pm = player.GetComponent<PlayerMovement>();
        if (pm == null)
        {
            Debug.LogWarning("Brak komponentu PlayerMovement!");
            return false;
        }

        return pm.IsGrounded();
    }
}

