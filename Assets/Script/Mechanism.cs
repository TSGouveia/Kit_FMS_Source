using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mechanism : MonoBehaviour
{
    private const float defaultAnimationSpeed = 0.5f;
    private const float defaultBoxSpeed = .90f;

    private const float conveyorASpeed = 0.87f;
    private const float conveyorBSpeed = 0.95f;
    private const float conveyorCSpeed = 0.96f;
    private const float conveyorDSpeed = 1.04f;
    private const float conveyorESpeed = 0.9f;
    private const float conveyorFSpeed = 0.93f;

    [SerializeField]
    private Conveyor[] conveyorScript;

    [SerializeField]
    private BoxMovement boxScript;

    [SerializeField]
    private PunchScript punchScript;

    [SerializeField]
    private GameObject robotPrefab;

    [SerializeField]
    private Transform robotPosLeft;
    [SerializeField]
    private Transform robotPosRight;

    private GameObject robotL;
    private GameObject robotR;

    [SerializeField]
    private GameObject humanPrefab;

    [SerializeField]
    private Transform humanPosLeft;
    [SerializeField]
    private Transform humanPosRight;

    private GameObject humanL;
    private GameObject humanR;

    private main_ui_control uiABB;

    private void Start()
    {
        uiABB = FindObjectOfType<main_ui_control>();
        uiABB.TaskOnClick_ConnectBTN();
    }
    private void ChangeSingleConveyorState(int convNumber, float multiplier)
    {
        conveyorScript[convNumber].ChangeAnimationSpeed(defaultAnimationSpeed * multiplier * defaultBoxSpeed);
        boxScript.ChangeSpeed(defaultBoxSpeed * multiplier);
    }
    private void RotateSingleConveyor(int convNumber, bool clockwise)
    {
        conveyorScript[convNumber].RotateConveyor(clockwise);
    }
    private void StopRotateSingleConveyor(int convNumber)
    {
        conveyorScript[convNumber].StopRotateConveyor();
    }
    private void TranslationSingleConveyor(int convNumber, bool direction)
    {
        conveyorScript[convNumber].TranslationConveyor(direction);
    }
    private void StopTranslationSingleConveyor(int convNumber)
    {
        conveyorScript[convNumber].StopTranslationConveyor();
    }

    public void ChangeConveyorState(char convLetter, bool on)
    {
        float speedMultiplier = on ? 1 : 0;
        char convLetterUpper = convLetter.ToString().ToUpper()[0];
        switch (convLetterUpper)
        {
            case 'A':
                ChangeSingleConveyorState(0, speedMultiplier * conveyorASpeed);
                break;
            case 'B':
                ChangeSingleConveyorState(1, speedMultiplier * conveyorBSpeed);
                break;
            case 'C':
                ChangeSingleConveyorState(2, speedMultiplier * conveyorCSpeed);
                ChangeSingleConveyorState(3, speedMultiplier * conveyorCSpeed);
                break;
            case 'D':
                ChangeSingleConveyorState(4, speedMultiplier * conveyorDSpeed);
                break;
            case 'E':
                ChangeSingleConveyorState(5, speedMultiplier * conveyorESpeed);
                ChangeSingleConveyorState(6, speedMultiplier * conveyorESpeed);
                break;
            case 'F':
                ChangeSingleConveyorState(7, speedMultiplier * conveyorFSpeed);
                break;
            default:
                Debug.LogError("ERROR CHANGING CONVEYOR STATE: CONV LETTER NOT FOUND");
                break;
        }
    }
    public void ChangeConveyorStateBackwards(char convLetter, bool on)
    {
        float speedMultiplier = on ? 1 : 0;
        char convLetterUpper = convLetter.ToString().ToUpper()[0];
        switch (convLetterUpper)
        {
            case 'A':
                ChangeSingleConveyorState(0, -speedMultiplier * conveyorASpeed);
                break;
            default:
                Debug.LogError("ERROR CHANGING CONVEYOR STATE: CONV NOT SUPPOSED TO GO BACKWARDS");
                break;
        }
    }

    public void RotateConveyor(char convLetter, bool clockwise)
    {
        char convLetterUpper = convLetter.ToString().ToUpper()[0];
        switch (convLetterUpper)
        {
            case 'C':
                RotateSingleConveyor(2, clockwise);
                break;
            case 'E':
                RotateSingleConveyor(5, clockwise);
                break;
            default:
                Debug.LogError("ERROR: conveyor " + convLetterUpper + " cant rotate");
                break;
        }
    }

    public void StopRotateConveyor(char convLetter)
    {
        char convLetterUpper = convLetter.ToString().ToUpper()[0];
        switch (convLetterUpper)
        {
            case 'C':
                StopRotateSingleConveyor(2);
                break;
            case 'E':
                StopRotateSingleConveyor(5);
                break;
            default:
                break;
        }
    }

    public void TranslationConveyor(char convLetter, bool left) // true -> left // false -> right
    {
        char convLetterUpper = convLetter.ToString().ToUpper()[0];
        switch (convLetterUpper)
        {
            case 'A':
                TranslationSingleConveyor(0, left);
                break;
            default:
                Debug.LogError("ERROR: conveyor " + convLetterUpper + " cant move");
                break;
        }
    }
    public void StopTranslationConveyor(char convLetter)
    {
        char convLetterUpper = convLetter.ToString().ToUpper()[0];
        switch (convLetterUpper)
        {
            case 'A':
                StopTranslationSingleConveyor(0);
                break;
            default:
                break;
        }
    }
    public void RotatePunch(bool on)
    {
        punchScript.ChangeRotationSpeed(on);
    }
    public void HorizontalDirPunch(bool forward)
    {
        Vector3 dir = forward ? Vector3.back : Vector3.forward; // directions are inverted because prespective
        punchScript.ChangePunchDirectionHorizontal(dir);
    }
    public void VerticalDirPunch(bool down)
    {
        Vector3 dir = down ? Vector3.down : Vector3.up;
        punchScript.ChangePunchDirectionVertical(dir);
    }
    public void StopVerticalDirPunch()
    {
        punchScript.ChangePunchDirectionVertical(Vector3.zero);
    }
    public void StopHorizontalDirPunch()
    {
        punchScript.ChangePunchDirectionHorizontal(Vector3.zero);
    }
    public void SpawnRobotLeft()
    {
        if (robotL == null)
        {
            Debug.Log("Robot on left");
            robotL = Instantiate(robotPrefab, robotPosLeft);
            uiABB.TaskOnClick_ConnectBTN();
        }
    }
    public void SpawnRobotRight()
    {
        if (robotR == null)
        {
            Debug.Log("Robot on right");
            robotR = Instantiate(robotPrefab, robotPosRight);
            uiABB.TaskOnClick_ConnectBTN();
        }
    }
    public void DispawnRobotLeft()
    {
        Debug.Log("KILL Robot on left");
        Destroy(robotL);
        uiABB.TaskOnClick_DisconnectBTN();
    }
    public void DispawnRobotRight()
    {
        Debug.Log("KILL Robot on left");
        Destroy(robotR);
        uiABB.TaskOnClick_DisconnectBTN();
    }
    public void SpawnHumanLeft()
    {
        if (humanL == null)
        {
            Debug.Log("Human on left");
            humanL = Instantiate(humanPrefab, humanPosLeft);
        }
    }
    public void SpawnHumanRight()
    {
        if (humanR == null)
        {
            Debug.Log("Human on right");
            humanR = Instantiate(humanPrefab, humanPosRight);
        }
    }
    public void DispawnHumanLeft()
    {
        Debug.Log("KILL Human on left");
        Destroy(humanL);
    }
    public void DispawnHumanRight()
    {
        Debug.Log("KILL Human on right");
        Destroy(humanR);
    }
}
