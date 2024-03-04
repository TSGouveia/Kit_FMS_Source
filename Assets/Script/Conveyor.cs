using System.IO;
using UnityEngine;

public class Conveyor : MonoBehaviour
{
    private const float rotationSpeedTake = 78.72f;
    private const float rotationSpeedGive = 81.22f;

    private const float translationSpeedLeft = 0.32f;
    private const float translationSpeedRight = 0.316f;

    private Transform conveyor;
    [SerializeField]
    private float animationSpeed;
    private const string BoxTag = "Box";
    private GameObject box;
    private BoxMovement boxMovement;
    private bool isConveyorON = false;
    private bool boxIsColliding = false;

    float rotationMultiplier;
    float translationSpeed;
    private bool isConveyorRotating;


    Vector3 directionTranslation = Vector3.zero;

    bool isConveyorTranslating = false;

    private void Awake()
    {
        box = GameObject.FindGameObjectWithTag(BoxTag);
        boxMovement = box.GetComponent<BoxMovement>();
        conveyor = transform.parent;
    }
    void FixedUpdate()
    {
        GetComponent<MeshRenderer>().material.mainTextureOffset += animationSpeed * Time.deltaTime * Vector2.up;
        if (isConveyorRotating)
        {
            conveyor.Rotate(0, rotationMultiplier * Time.deltaTime, 0);

            if (boxIsColliding)
            {
                box.transform.Rotate(0, 0, rotationMultiplier * Time.deltaTime);
            }
        }
        if (isConveyorTranslating)
        {
            conveyor.position += Time.deltaTime * translationSpeed * directionTranslation;
            if (boxIsColliding)
            {
                box.transform.position += Time.deltaTime * translationSpeed * directionTranslation;
            }

        }
    }

    public void ChangeAnimationSpeed(float newAnimSpeed)
    {
        animationSpeed = newAnimSpeed;
        if (newAnimSpeed == 0)
        {
            TurnOffConveyorPhysics();
        }
        else
        {
            bool backwards = newAnimSpeed < 0;
            TurnOnConveyorPhysics(backwards);
        }
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Box"))
        {
            boxIsColliding = true;

            TurnOnConveyorPhysics(false);
        }
    }
    private void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.CompareTag("Box"))
        {
            if (gameObject.transform.parent.name == "ConveyorA" && transform.parent.position.x < 0)
            {
                TurnOffConveyorPhysics();
                TurnOnConveyorPhysics(true);
            }
            if (gameObject.transform.parent.name == "ConveyorC")
            {
                if (transform.parent.eulerAngles.y > 315)
                {
                    boxMovement.DecrementConveyorNumber();
                    boxMovement.IncrementConveyorNumber(Vector3.right);
                }
            }
            if (gameObject.transform.parent.name == "ConveyorE")
            {
                if(transform.parent.eulerAngles.y > 45)
                {
                    boxMovement.DecrementConveyorNumber();
                    boxMovement.IncrementConveyorNumber(Vector3.back);
                }

            }   
        }
    }
    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Box"))
        {
            TurnOffConveyorPhysics();
            boxIsColliding = false;
        }
    }
    private void TurnOnConveyorPhysics(bool backwards)
    {
        if (!isConveyorON && boxIsColliding)
        {
            //Debug.Log("Direction updated: " + transform.forward);
            Vector3 direction = transform.forward;
            if (backwards)
            {
                direction = -direction;
            }
            boxMovement.IncrementConveyorNumber(direction);
            isConveyorON = true;
        }
    }
    private void TurnOffConveyorPhysics()
    {
        if (isConveyorON && boxIsColliding)
        {
            boxMovement.DecrementConveyorNumber();
            isConveyorON = false;
        }
    }

    public void RotateConveyor(bool clockwise)
    {
        rotationMultiplier = clockwise ? rotationSpeedGive : -rotationSpeedTake;
        isConveyorRotating = true;
        //boxMovement.DecrementConveyorNumber();
    }
    public void StopRotateConveyor()
    {
        rotationMultiplier = 0;
        isConveyorRotating = false;
        //boxMovement.IncrementConveyorNumber(transform.position);
    }
    public void TranslationConveyor(bool direction) // true -> left // false -> right
    {
        if (direction)
        {
            directionTranslation = Vector3.left;
            translationSpeed = translationSpeedLeft;

        }
        else
        {
            directionTranslation = Vector3.right;
            translationSpeed = translationSpeedRight;
        }
        isConveyorTranslating = true;
    }
    public void StopTranslationConveyor()
    {
        directionTranslation = Vector3.zero;
        isConveyorTranslating = false;
    }
}