using UnityEngine;

public class PlayerDeath : MonoBehaviour
{
    public float deathHeight = -200f; 
    private GameManager gameManager;

    private void Start()
    {
        gameManager = FindObjectOfType<GameManager>();
    }

    private void Update()
    {
        if (transform.position.y < deathHeight)
        {
            Debug.Log("Gracz spad�. Prze��czam na kolejn� posta�.");
            gameManager.SwitchToNextCharacter();
        }
    }
}