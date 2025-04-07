using UnityEngine;

public class PlayerDeath : MonoBehaviour
{
    public float deathHeight = -200f; 
    private GameManager gameManager;
    private bool isDead = false;
    private Coroutine currentRespawnRoutine;


    [SerializeField] private AudioClip deathSoundClip;

    private void Start()
    {
        gameManager = FindObjectOfType<GameManager>();
    }

    private void Update()
    {
        if (transform.position.y < deathHeight && !isDead && currentRespawnRoutine == null)
        {
            isDead = true;
            currentRespawnRoutine = StartCoroutine(RespawnWithDelay(1f));
        }
    }

    private System.Collections.IEnumerator RespawnWithDelay(float delay)
    {
        SoundFXManager.instance.PlaySoundFXClip(deathSoundClip, transform, 1.3f);

        CameraFollow cameraFollow = Camera.main.GetComponent<CameraFollow>();
        if (cameraFollow != null)
        {
            cameraFollow.ShakeBeforeFollow(1f, 5f);
        }

        PlayerCarry carry = GetComponent<PlayerCarry>();
        if (carry != null)
        {
            carry.DropAcornAtCheckpoint();
        }

        yield return new WaitForSeconds(delay);

        gameManager.SwitchToNextCharacter();

        isDead = false;
        currentRespawnRoutine = null;
    }
}