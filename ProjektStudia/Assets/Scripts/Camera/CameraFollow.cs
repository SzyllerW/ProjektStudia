using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    private Transform player;
    [SerializeField] private float smoothSpeed = 0.125f;
    [SerializeField] private Vector2 offset = new Vector2(3f, 2f);
    [SerializeField] private float stopFollowingY = -70f;

    private void Start()
    {
        FindPlayer();
    }

    private void LateUpdate()
    {
        if (player == null || !player.gameObject.activeSelf || player.position.y <= stopFollowingY)
        {
            FindPlayer();
            return;
        }

        Vector3 targetPosition = new Vector3(
            player.position.x + (player.localScale.x > 0 ? offset.x : -offset.x),
            player.position.y + offset.y,
            transform.position.z
        );

        transform.position = Vector3.Lerp(transform.position, targetPosition, smoothSpeed);
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
    }
}
