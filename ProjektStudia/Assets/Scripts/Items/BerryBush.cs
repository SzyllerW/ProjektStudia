using System.Collections.Generic;
using UnityEngine;

public class BerryBush : MonoBehaviour
{
    [SerializeField] private List<GameObject> berries = new List<GameObject>();
    public List<GameObject> Berries => berries;
    [SerializeField] private int maxBerries = 4;
    [SerializeField] private AudioClip pickUpItemClip;
    [SerializeField] private float pickUpItemVolume = 1f;

    private HashSet<GameObject> playersThatCollected = new HashSet<GameObject>();
    private int berriesCollected = 0;

    private void Start()
    {
        Debug.Log("[BerryBush] Start dzia³a!");

        for (int i = 0; i < berries.Count; i++)
        {
            if (i >= maxBerries && berries[i] != null)
            {
                berries[i].SetActive(false);
            }
        }
    }


    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("[BerryBush] Coœ wesz³o do triggera: " + other.name);

        if (!other.CompareTag("Player")) return;
        if (playersThatCollected.Contains(other.gameObject)) return;

        Debug.Log("[BerryBush] Gracz wykryty: " + other.name);

        foreach (var berry in berries)
        {
            if (berry != null && berry.activeSelf)
            {
                berry.SetActive(false); // Mo¿na tu dodaæ animacjê
                Debug.Log("[BerryBush] Zebrano jagodê: " + berry.name);

                berriesCollected++;
                playersThatCollected.Add(other.gameObject);
                SoundFXManager.instance.PlaySoundFXClip(pickUpItemClip, transform, pickUpItemVolume);

                if (GameManager.Instance != null)
                {
                    GameManager.Instance.BerryCollected();
                }
                else
                {
                    Debug.LogWarning("[BerryBush] GameManager.Instance jest null!");
                }

                return; 
            }
        }
    }
}



