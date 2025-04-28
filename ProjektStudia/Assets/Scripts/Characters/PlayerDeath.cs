using System.Collections;
using UnityEngine;

public class PlayerDeath : MonoBehaviour
{
    public float deathHeight = -200f;
    private GameManager gameManager;
    private bool isDead = false;
    private Coroutine currentRespawnRoutine;
    private bool deathBlockedTemporarily = false;

    [SerializeField] private AudioClip deathSoundClip;

    private void Start()
    {
        gameManager = FindObjectOfType<GameManager>();
    }

    private void Update()
    {
        if (currentRespawnRoutine != null) return;
        if (isDead || deathBlockedTemporarily) return;

        if (transform.position.y < deathHeight)
        {
            isDead = true; 
            Debug.Log($"[PlayerDeath] {gameObject.name} - START ŒMIERCI | pos: {transform.position}");
            currentRespawnRoutine = StartCoroutine(RespawnWithDelay(1f));
        }
    }

    private IEnumerator RespawnWithDelay(float delay)
    {
        if (SoundFXManager.instance != null)
        {
            SoundFXManager.instance.PlaySoundFXClip(deathSoundClip, transform, 1.3f);
        }

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

        if (gameManager != null)
        {
            gameManager.CharacterFellOffMap(gameObject); 
        }
    }

    public void ResetDeath()
    {
        isDead = false;
        currentRespawnRoutine = null;
        StartCoroutine(BlockDeathBriefly());
    }

    private IEnumerator BlockDeathBriefly()
    {
        deathBlockedTemporarily = true;
        yield return new WaitForSeconds(0.3f);
        deathBlockedTemporarily = false;
    }
}
