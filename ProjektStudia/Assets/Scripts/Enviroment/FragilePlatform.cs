using UnityEngine;
using System.Collections;

public class FragilePlatform : MonoBehaviour
{
    public int maxSteps = 1;
    public float fallDelay = 0.5f;
    public float destroyDelay = 1f;

    [SerializeField] private Animator animator;
    [SerializeField] private Rigidbody2D rb;

    private int stepCount = 0;
    private bool isBreaking = false;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        //CameraFollow cameraFollow = Camera.main.GetComponent<CameraFollow>();

        if (collision.gameObject.CompareTag("Player"))
        {
            stepCount++;
            
            //cameraFollow.ShakeBeforeFollow(0.25f, 0.5f);

            if (stepCount == maxSteps)
            {
                animator.SetTrigger("crack");
                StartCoroutine(BreakPlatform());
            }
        }
    }

    private IEnumerator BreakPlatform()
    {
        isBreaking = true;

        yield return new WaitForSeconds(fallDelay);
        rb.bodyType = RigidbodyType2D.Dynamic;
        rb.gravityScale = 20f;
        Destroy(gameObject, destroyDelay);
    }
}



