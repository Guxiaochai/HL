using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingSphere : MonoBehaviour
{
    [SerializeField, Range(0f, 100f)]
    float maxSpeed = 10f;

    [SerializeField, Range(0f, 100f)]
    float maxAcceleration = 10f;

    [SerializeField]
    Rect allowArea = new Rect(-5f, -5f, 10f, 10f);

    [SerializeField, Range(0f, 1f)]
    float bunciness = 0.5f;

    Vector3 velocity;

    private void Update()
    {
        Vector2 playerInput;
        playerInput.x = Input.GetAxis("Horizontal");
        playerInput.y = Input.GetAxis("Vertical");
        //playerInput.Normalize();
        playerInput = Vector2.ClampMagnitude(playerInput, 1f);
        Vector3 desiredVelocity = new Vector3(playerInput.x, 0f, playerInput.y) * maxSpeed;
        float maxSpeedChange = maxAcceleration * Time.deltaTime;

        #region same as MoveTowards
        //if (velocity.x < desiredVelocity.x)
        //{
        //    // to prevent the result exceeding the desiredVelocity.x
        //    velocity.x = Mathf.Min(velocity.x + maxSpeedChange, desiredVelocity.x);
        //}
        //else if (velocity.x > desiredVelocity.x)
        //{
        //    velocity.x = Mathf.Max(velocity.x - maxSpeedChange, desiredVelocity.x);
        //}
        #endregion
        velocity.x = Mathf.MoveTowards(velocity.x, desiredVelocity.x, maxSpeedChange);
        velocity.z = Mathf.MoveTowards(velocity.z, desiredVelocity.z, maxSpeedChange);

        Vector3 displacement = velocity * Time.deltaTime;
        Vector3 newPosition = transform.localPosition + displacement;

        if (newPosition.x < allowArea.xMin)
        {
            newPosition.x = allowArea.xMin;
            velocity.x = -velocity.x * bunciness;
        }
        else if (newPosition.x > allowArea.xMax)
        {
            newPosition.x = allowArea.xMax;
            velocity.x = -velocity.x * bunciness;
        }

        if (newPosition.z < allowArea.yMin)
        {
            newPosition.z = allowArea.yMin;
            velocity.z = -velocity.z * bunciness;
        }
        else if (newPosition.z > allowArea.yMax)
        {
            newPosition.z = allowArea.yMax;
            velocity.z = -velocity.z * bunciness;
        }

        transform.localPosition = newPosition;
    }
}
