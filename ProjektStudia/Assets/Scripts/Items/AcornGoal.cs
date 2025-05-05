using UnityEngine;

public class AcornGoal : MonoBehaviour
{
    [SerializeField] Animator animator;
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerCarry player = other.GetComponent<PlayerCarry>();
            if (player != null && player.CanDeliverAcorn())
            {
                player.DropAcornAtGoal();
                FindObjectOfType<GameManager>()?.AcornDelivered();
                animator.enabled = true;
            }
        }
    }
}
