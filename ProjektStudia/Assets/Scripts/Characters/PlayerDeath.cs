using UnityEngine;

public class PlayerDeath : MonoBehaviour
{
    public float deathHeight = -5f; // Wysokoœæ œmierci
    private GameManager gameManager;

    private void Start()
    {
        gameManager = FindObjectOfType<GameManager>();
    }

    private void Update()
    {
        if (transform.position.y < deathHeight)
        {
            Debug.Log("Gracz spad³. Prze³¹czam na kolejn¹ postaæ.");
            gameManager.SwitchToNextCharacter();
        }
    }
}