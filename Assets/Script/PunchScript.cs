using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class PunchScript : MonoBehaviour
{
    private const float defaultRotationSpeed = 200;
    private const float backSpeed = 0.38f;
    private const float forwardSpeed = 0.3f;
    private const float upSpeed = 0.27f;
    private const float downSpeed = 0.35f;

    private float horizontalSpeed;
    private float verticalSpeed;

    [SerializeField]
    private Transform punchUpDown;
    [SerializeField]
    private Transform punchForwBack;
    [SerializeField]
    private Transform drill;


    float rotationSpeed = 0;
    Vector3 horizontalDir = Vector3.zero;
    Vector3 verticalDir = Vector3.zero;
    private void Update()
    {
        drill.Rotate(0, rotationSpeed * Time.deltaTime, 0);
        punchForwBack.position += horizontalSpeed * Time.deltaTime * horizontalDir;
        punchUpDown.position += Time.deltaTime * verticalSpeed * verticalDir;
    }
    public void ChangeRotationSpeed(bool on)
    {
        rotationSpeed = on ? defaultRotationSpeed : 0;
    }
    public void ChangePunchDirectionHorizontal(Vector3 newDirection)
    {
        horizontalDir = newDirection;

        if (horizontalDir == Vector3.back)
        {
            horizontalSpeed = forwardSpeed; //back and forward are inverted
        }
        if (horizontalDir == Vector3.forward)
        {
            horizontalSpeed = backSpeed; //back and forward are inverted
        }
    }
    public void ChangePunchDirectionVertical(Vector3 newDirection)
    {
        verticalDir = newDirection;
        if (verticalDir == Vector3.down)
        {
            verticalSpeed = downSpeed;
        }
        if (verticalDir == Vector3.up)
        {
            verticalSpeed = upSpeed;
        }
    }
}
