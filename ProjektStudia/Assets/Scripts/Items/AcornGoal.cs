using UnityEngine;
using UnityEngine.SceneManagement;

public class AcornGoal : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerCarry player = other.GetComponent<PlayerCarry>();
            if (player != null && player.HasAcorn())
            {
                Debug.Log(" ¯o³¹dŸ dostarczony! Poziom ukoñczony.");
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
            }
        }
    }
}
