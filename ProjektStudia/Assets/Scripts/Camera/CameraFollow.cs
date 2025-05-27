using UnityEngine;
using System.Collections;

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

    [Header("Cinematic Presentation")]
    [SerializeField] private bool playCinematicAtStart = true;
    [SerializeField] private Transform[] cinematicPoints;
    [SerializeField] private float cinematicSpeed = 40f;
    [SerializeField] private float waitAtEachPoint = 1.5f;
    [SerializeField] private GameObject[] objectsToEnableAfterCinematic;
    [SerializeField] private GameObject characterSelectionUI;

    private Transform player;
    private Rigidbody2D playerRb;
    private Vector3 lastKnownPosition;
    private float idleTime = 0f;
    private float currentTargetSize;
    private bool isShaking = false;
    private bool followingRespawn = false;
    private bool isCinematicDone = false;
    private bool isFocused = false;

    private enum ZoomState { Normal, Jumping, Idle }
    private ZoomState zoomState = ZoomState.Normal;

    private void Start()
    {
        Camera.main.orthographicSize = baseSize;
        currentTargetSize = baseSize;

        if (characterSelectionUI != null)
        {
            characterSelectionUI.SetActive(false);
        }

        if (playCinematicAtStart && cinematicPoints.Length > 0)
        {
            transform.position = new Vector3(
                cinematicPoints[0].position.x,
                cinematicPoints[0].position.y,
                transform.position.z
            );
            StartCoroutine(StartCinematic());
        }
        else
        {
            if (respawnPoint != null)
            {
                player = respawnPoint;
                playerRb = null;
                followingRespawn = true;
                isCinematicDone = true;

                if (characterSelectionUI != null)
                {
                    characterSelectionUI.SetActive(true);
                }
            }
        }
    }

    private IEnumerator StartCinematic()
    {
        isCinematicDone = false;
        isShaking = true;

        foreach (Transform point in cinematicPoints)
        {
            Vector3 target = new Vector3(point.position.x, point.position.y, transform.position.z);
            Vector3 startPos = transform.position;

            float distance = Vector3.Distance(startPos, target);
            float duration = Mathf.Max(distance / cinematicSpeed, 0.2f);
            float elapsed = 0f;

            while (elapsed < duration)
            {
                float t = Mathf.SmoothStep(0f, 1f, elapsed / duration);
                transform.position = Vector3.Lerp(startPos, target, t);
                elapsed += Time.deltaTime;
                yield return null;
            }

            transform.position = target;
            yield return new WaitForSeconds(waitAtEachPoint);
        }

        // Return to starting point
        Transform startPoint = cinematicPoints[0];
        Vector3 returnTarget = new Vector3(startPoint.position.x, startPoint.position.y, transform.position.z);
        Vector3 returnStart = transform.position;
        float returnDistance = Vector3.Distance(returnStart, returnTarget);
        float returnDuration = Mathf.Max(returnDistance / cinematicSpeed, 0.2f);
        float returnElapsed = 0f;

        while (returnElapsed < returnDuration)
        {
            float t = Mathf.SmoothStep(0f, 1f, returnElapsed / returnDuration);
            transform.position = Vector3.Lerp(returnStart, returnTarget, t);
            returnElapsed += Time.deltaTime;
            yield return null;
        }

        transform.position = returnTarget;

        isCinematicDone = true;
        isShaking = false;

        FindPlayerByTag();

        foreach (var obj in objectsToEnableAfterCinematic)
        {
            obj.SetActive(true);
        }

        if (characterSelectionUI != null)
        {
            characterSelectionUI.SetActive(true);
        }
    }

    private void LateUpdate()
    {
        if (isShaking || !isCinematicDone) return;

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

            if (isFocused)
            {
                currentTargetSize = baseSize * 0.8f;
            }
            else if (!IsGrounded() && Mathf.Abs(vVel) > 0.1f)
            {
                if (zoomState != ZoomState.Jumping || !isFocused)
                {
                    zoomState = ZoomState.Jumping;
                    idleTime = 0f;

                    if (!isFocused)
                    {
                        zoomState = ZoomState.Normal;
                        currentTargetSize = jumpZoomSize;
                    }
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

    private IEnumerator Shake(float duration, float magnitude)
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

    public void focusOnPortal(bool inRange)
    {
        isFocused = inRange;
    }
}






