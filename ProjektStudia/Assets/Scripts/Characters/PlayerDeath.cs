using UnityEngine;

public class PlayerDeath : MonoBehaviour
{
    public float deathHeight = -200f; 
    private GameManager gameManager;

    [SerializeField] private AudioClip deathSoundClip;

    private void Start()
    {
        gameManager = FindObjectOfType<GameManager>();
    }

    private void Update()
    {
        if (transform.position.y < deathHeight)
        {
            //play sound FX
            SoundFXManager.instance.PlaySoundFXClip(deathSoundClip, transform, 1.3f);

            Debug.Log("Gracz spad³. Prze³¹czam na kolejn¹ postaæ.");
            gameManager.SwitchToNextCharacter();
        }
    }
}