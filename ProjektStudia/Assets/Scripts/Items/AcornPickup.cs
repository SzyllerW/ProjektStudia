using UnityEngine;

public class AcornPickup : MonoBehaviour
{
    public Transform lastCheckpoint;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerCarry player = other.GetComponent<PlayerCarry>();
            if (player != null && !player.HasAcorn())
            {
                player.PickUpAcorn(this);
            }
        }
    }

    public void RespawnAtCheckpoint()
    {
        Debug.Log("[AcornPickup] Respawnujê ¿o³¹dŸ w: " + lastCheckpoint.position);
        transform.SetParent(null);
        transform.position = lastCheckpoint.position;
        gameObject.SetActive(true);
    }

    public void SetCheckpoint(Transform checkpoint)
    {
        lastCheckpoint = checkpoint;
    }
}