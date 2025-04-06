using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlatformVelocity : MonoBehaviour
{
    public Vector2 Velocity { get; private set; }
    private Vector2 previousPosition;

    private void Start()
    {
        previousPosition = transform.position;
    }

    private void FixedUpdate()
    {
        Vector2 currentPosition = transform.position;
        Velocity = (currentPosition - previousPosition) / Time.fixedDeltaTime;
        previousPosition = currentPosition;
    }
}

