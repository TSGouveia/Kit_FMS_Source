using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoxMovement : MonoBehaviour
{
    private int beltsInContact = 0;

    private float speed = 0;

    private Rigidbody boxrb;
    private Vector3 direction = Vector3.zero;
    private void Start()
    {
        boxrb = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        if (beltsInContact > 0 && speed != 0)
        {
            boxrb.MovePosition(boxrb.position + speed * Time.deltaTime * direction);
        }
    }

    public void ChangeSpeed(float newSpeed)
    {
        speed = newSpeed;
    }
    public void IncrementConveyorNumber(Vector3 direction)
    {
        beltsInContact++;
        if (direction.x > 0.1f && direction.x < 0.1f)
        {
            direction.x = 0;
        }
        if (direction.y > 0.1f && direction.y < 0.1f)
        {
            direction.y = 0;
        }
        if (direction.z > 0.1f && direction.z < 0.1f)
        {
            direction.z = 0;
        }
        this.direction = direction;
    }
    public void DecrementConveyorNumber()
    {
        beltsInContact--;
    }
}
