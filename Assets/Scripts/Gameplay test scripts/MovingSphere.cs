using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

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
        var inputsource = GetComponent<InputReader>().GetInputSource();
        if (inputsource != default)
        {
            inputsource.Player.Jump.canceled += OnJumpInvoke;
            inputsource.Player.Jump.performed += OnJumpInvoke;
        }
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

    private void OnJumpInvoke(InputAction.CallbackContext cbc)
    {
        if (cbc.duration > 0.15f && cbc.canceled)
        {
            return;
        }
         
        body.AddForce(Vector3.up * (Mathf.Clamp((float)cbc.duration, 0.1f, 0.15f)/(0.15f - 0.1f) * PlayerStates.Instance.JumpPower), ForceMode.Impulse);
    }
}
