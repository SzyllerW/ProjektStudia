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
                Debug.Log(" �o��d� dostarczony! Poziom uko�czony.");
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
            }
        }
    }
}
