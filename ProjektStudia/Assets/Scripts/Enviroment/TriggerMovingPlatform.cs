using UnityEngine;

public class TriggerMovingPlatform : MonoBehaviour
{
    [SerializeField] private GameObject movingPlatform;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if (movingPlatform != null)
            {
                MovingPlatform platformScript = movingPlatform.GetComponent<MovingPlatform>();

                if (platformScript != null)
                {
                    platformScript.enabled = true;
                }
            }
            
            Destroy(gameObject);
        }
    }
}