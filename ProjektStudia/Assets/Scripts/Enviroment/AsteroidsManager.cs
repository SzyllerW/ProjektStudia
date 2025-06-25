using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AsteroidsManager : MonoBehaviour
{
    [SerializeField] private GameObject[] asteroids;
    [SerializeField] private float amplitude = 0.15f;
    [SerializeField] private float speed = 2.0f;

    void Start()
    {
        StartCoroutine(IdleAnimation());
    }

    private IEnumerator IdleAnimation()
    {
        while (true)
        {
            foreach (GameObject asteroid in asteroids)
            {
                if (asteroid != null)
                {
                    float time = Mathf.Sin(Time.time * speed) * amplitude;

                    Vector3 startPosition = asteroid.transform.position;
                    asteroid.transform.position = startPosition + new Vector3(0, time, 0);
                }
            }

            yield return null;
        }
    }

}
