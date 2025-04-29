using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] private Transform respawnPoint;
    [SerializeField] private float smoothSpeed = 0.125f;
    [SerializeField] private Vector2 baseOffset = new Vector2(3f, 2f);
    [SerializeField] private float baseSize = 126f;
    [SerializeField] private float jumpZoomSize = 132f;
    [SerializeField] private float idleZoomSize = 140f;
    [SerializeField] private float zoomInSpeed = 10f;
    [SerializeField] private float jumpZoomOutSpeed = 6f;
    [SerializeField] private float idleZoomOutSpeed = 2f;
    [SerializeField] private float idleThreshold = 2f;

    private Transform player;
    private Rigidbody2D playerRb;
    private Vector3 lastKnownPosition;
    private float idleTime = 0f;
    private float currentTargetSize;
    private bool isShaking = false;
    private bool followingRespawn = false;

    private enum ZoomState { Normal, Jumping, Idle }
    private ZoomState zoomState = ZoomState.Normal;

    private void Start()
    {
        Camera.main.orthographicSize = baseSize;
        currentTargetSize = baseSize;

        if (respawnPoint != null)
        {
            player = respawnPoint;
            playerRb = null;
            followingRespawn = true;
        }
    }

    private void LateUpdate()
    {
        if (isShaking) return;

        if (player == null || !player.gameObject.activeSelf || followingRespawn)
        {
            FindPlayerByTag();
        }

        if (player == null) return;

        if (!followingRespawn)
        {
            if (playerRb == null)
                playerRb = player.GetComponent<Rigidbody2D>();

            float hVel = Mathf.Abs(playerRb.velocity.x);
            float vVel = playerRb.velocity.y;

            if (!IsGrounded() && Mathf.Abs(vVel) > 0.1f)
            {
                if (zoomState != ZoomState.Jumping)
                {
                    zoomState = ZoomState.Jumping;
                    currentTargetSize = jumpZoomSize;
                    idleTime = 0f;
                }
            }
            else if (hVel < 0.5f && Mathf.Abs(vVel) < 0.5f)
            {
                idleTime += Time.deltaTime;
                if (idleTime > idleThreshold && zoomState != ZoomState.Idle)
                {
                    zoomState = ZoomState.Idle;
                    currentTargetSize = idleZoomSize;
                }
            }
            else
            {
                if (zoomState != ZoomState.Normal)
                {
                    zoomState = ZoomState.Normal;
                    currentTargetSize = baseSize;
                    idleTime = 0f;
                }
            }

            float zoomSpeed = zoomInSpeed;
            if (zoomState == ZoomState.Jumping) zoomSpeed = jumpZoomOutSpeed;
            else if (zoomState == ZoomState.Idle) zoomSpeed = idleZoomOutSpeed;

            Camera.main.orthographicSize = Mathf.MoveTowards(
                Camera.main.orthographicSize,
                currentTargetSize,
                zoomSpeed * Time.deltaTime
            );
        }

        Vector3 targetPosition = new Vector3(
            player.position.x + (player.localScale.x > 0 ? baseOffset.x : -baseOffset.x),
            player.position.y + baseOffset.y,
            transform.position.z
        );

        lastKnownPosition = targetPosition;
        transform.position = Vector3.Lerp(transform.position, targetPosition, smoothSpeed);
    }

    private void FindPlayerByTag()
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");

        foreach (GameObject p in players)
        {
            if (p.activeSelf)
            {
                player = p.transform;
                playerRb = player.GetComponent<Rigidbody2D>();
                followingRespawn = false;
                Debug.Log("[CameraFollow] Znaleziono gracza: " + p.name);
                return;
            }
        }

        if (respawnPoint != null)
        {
            player = respawnPoint;
            playerRb = null;
            followingRespawn = true;
        }
    }

    private bool IsGrounded()
    {
        if (player == null) return false;

        PlayerMovement pm = player.GetComponent<PlayerMovement>();
        return pm != null && pm.IsGrounded();
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
        float elapsed = 0f;

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
}







