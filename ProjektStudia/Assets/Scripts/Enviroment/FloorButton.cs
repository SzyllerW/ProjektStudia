using UnityEngine;

public class FloorButton : MonoBehaviour
{
    public Door door; 

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Player"))
        {
            Debug.Log("Gracz aktywowa³ przycisk!");
            door.ToggleDoor(); 
        }
    }
}
