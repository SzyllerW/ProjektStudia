using UnityEngine;

public class PlayerDeath : MonoBehaviour
{
    public float deathHeight = -200f; 
    private GameManager gameManager;
    private bool isDead = false;

    [SerializeField] private AudioClip deathSoundClip;

    private void Start()
    {
        gameManager = FindObjectOfType<GameManager>();
    }

    private void Update()
    {
        if (transform.position.y < deathHeight && !isDead)
        {
            isDead = true;
            StartCoroutine(RespawnWithDelay(1f));
        }
    }

    private System.Collections.IEnumerator RespawnWithDelay(float delay)
    {
        //play sound FX
        SoundFXManager.instance.PlaySoundFXClip(deathSoundClip, transform, 1.3f);

        //shake camera
        CameraFollow cameraFollow = Camera.main.GetComponent<CameraFollow>();
        if (cameraFollow != null)
        {
            cameraFollow.ShakeBeforeFollow(1f, 5f); //duration nad magnitude
        }

        yield return new WaitForSeconds(delay);
        gameManager.SwitchToNextCharacter();

        isDead = false;
    }
}