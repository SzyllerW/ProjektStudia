using System.Collections;
using UnityEngine;

public class PlayerDeath : MonoBehaviour
{
    [SerializeField] private Transform respawnPoint; 
    [SerializeField] private float respawnDelay = 1f; 
    [SerializeField] private float deathHeight = -5f; 

    private Rigidbody2D rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
     
        if (transform.position.y < deathHeight)
        {
            Debug.Log("Gracz umar³."); 
            StartCoroutine(Respawn());
        }
    }

    private IEnumerator Respawn()
    {
        Debug.Log("Rozpoczynam respawn.");
        yield return new WaitForSeconds(respawnDelay);

        if (respawnPoint != null)
        {
            Debug.Log("Respawn w pozycji: " + respawnPoint.position); 

            transform.position = respawnPoint.position;

            rb.velocity = Vector2.zero;

            rb.velocity = new Vector2(0f, -10f); 
        }
        else
        {
            Debug.Log("RespawnPoint jest null, u¿ywam domyœlnej pozycji."); 
            transform.position = new Vector2(0, 0);
        }
    }
}