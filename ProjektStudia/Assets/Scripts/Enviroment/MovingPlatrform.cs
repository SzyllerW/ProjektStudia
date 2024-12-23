using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    public Vector3 startPoint;          
    public Vector3 endPoint;            
    public float speed = 2f;  
    private bool movingToEnd = true;  

    void Start()
    {
        transform.position = startPoint;
    }

    void Update()
    {
        if (movingToEnd)
        {
            transform.position = Vector3.MoveTowards(transform.position, endPoint, speed * Time.deltaTime);
            if (Vector3.Distance(transform.position, endPoint) < 0.1f)
            {
                movingToEnd = false; 
            }
        }
        else
        {
            transform.position = Vector3.MoveTowards(transform.position, startPoint, speed * Time.deltaTime);
            if (Vector3.Distance(transform.position, startPoint) < 0.1f)
            {
                movingToEnd = true;
            }
        }
    }
}