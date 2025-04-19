using UnityEngine;
using System.Collections;

public class FragilePlatform : MonoBehaviour
{
    public GameObject cracks;          
    public float breakDelay = 0.5f;  
    public float destroyDelay = 0.5f; 

    private int stepCount = 0;
    private bool isBreaking = false;

    private void Awake()
    {
        if (cracks != null)
            cracks.SetActive(false); 
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            stepCount++;

            if (stepCount == 1)
            {
                if (cracks != null)
                    cracks.SetActive(true); 
            }
            else if (stepCount == 2 && !isBreaking)
            {
                StartCoroutine(BreakPlatform());
            }
        }
    }

    private IEnumerator BreakPlatform()
    {
        isBreaking = true;

        yield return new WaitForSeconds(breakDelay);

        yield return new WaitForSeconds(destroyDelay);

        gameObject.SetActive(false);
    }
}



