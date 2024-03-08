using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingSphere : MonoBehaviour
{
    [SerializeField, Range(0f, 100f)]
    float maxSpeed = 10f;

    [SerializeField, Range(0f, 100f)]
    float maxAcceleration = 10f;

    private Vector2 velocity;
    private Vector3 desiredVelocity;

    Rigidbody body;

    private void Awake()
    {
        body = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        velocity = PlayerStates.Instance.MoveDir * (maxSpeed * Time.deltaTime);
        velocity.x = Mathf.Clamp(velocity.x, -maxAcceleration, maxAcceleration * Time.deltaTime);
        velocity.y = Mathf.Clamp(velocity.y, -maxAcceleration, maxAcceleration * Time.deltaTime);
        
        desiredVelocity.x = velocity.x;
        desiredVelocity.z = velocity.y;
    }

    private void FixedUpdate()
    {
        body.velocity += desiredVelocity;
    }
}
