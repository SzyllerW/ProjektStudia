using UnityEngine;

public class AcornGoal : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerCarry player = other.GetComponent<PlayerCarry>();
            if (player != null && player.CanDeliverAcorn())
            {
                player.DropAcornAtGoal();
                FindObjectOfType<GameManager>()?.AcornDelivered(); 
            }
        }
    }
}
