using Newtonsoft.Json.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PortCorrecting : MonoBehaviour
{
    private const float conveyorCGiveRotationY = 0;
    private const float conveyorCTakeRotationY = -90;
    private const float conveyorEGiveRotationY = 90;
    private const float conveyorETakeRotationY = 0;

    private Vector3 punchUpPos = new Vector3(0, 0, 0);
    private Vector3 punchDownPos = new Vector3(0, -3, 0);
    private Vector3 punchForwPos = new Vector3(4, 0, 0);
    private Vector3 punchBackPos = new Vector3(0, 0, 0);

    [SerializeField]
    private Transform[] boxPositions;

    [SerializeField]
    private Transform[] conveyorAPositions; //0 RIGHT, 1 LEFT

    private CorrectMovement correctScript;

    private WarningManagerBox warningManagerBox;
    private WarningManagerPhysical warningManagerPhysical;

    private Mechanism mechanism;

    private Vector3[] initialConveyorPositions;

    void Start()
    {
        correctScript = GetComponent<CorrectMovement>();
        warningManagerBox = FindObjectOfType<WarningManagerBox>();
        warningManagerPhysical = FindObjectOfType<WarningManagerPhysical>();
        mechanism = FindObjectOfType<Mechanism>();


        initialConveyorPositions = new Vector3[boxPositions.Length];
        for (int i = 0; i < boxPositions.Length; i++)
        {
            initialConveyorPositions[i] = boxPositions[i].eulerAngles;            
        }
    }

    public void ResetAllBoxPositions()
    {
        for (int i = 0; i < boxPositions.Length; i++)
        {
            boxPositions[i].eulerAngles = initialConveyorPositions[i];
        }
    }

    private void RotateAllBoxPosition()
    {
        for (int i = 0; i < boxPositions.Length; i++)
        {
            boxPositions[i].eulerAngles = new Vector3(boxPositions[i].eulerAngles.x, boxPositions[i].eulerAngles.y, boxPositions[i].eulerAngles.z + 180);

        }
    }
    public void ReceiveSensorPort(int portNumber, int portIndex, bool value)
    {
        warningManagerBox.RegisterPhysicalSensorActivation(portNumber, portIndex, value);
        warningManagerPhysical.RegisterPhysicalSensorActivation(portNumber, portIndex, value);
        switch (portNumber)
        {
            case 0:
                switch (portIndex)
                {
                    case 0:
                        if (value)
                            correctScript.MoveBox(boxPositions[1]);
                        break;
                    case 1:
                        if (value)
                            correctScript.MoveBox(boxPositions[0]);
                        break;
                    case 2:
                        if (!value)
                            correctScript.ChangeUpDownPunchPosition(punchDownPos);
                        break;
                    case 3:
                        if (!value)
                            correctScript.ChangeUpDownPunchPosition(punchUpPos);
                        break;
                    case 4:
                        if (value)
                            correctScript.ChangeFrontBackPunchPosition(punchForwPos);
                        break;
                    case 5:
                        if (value)
                            correctScript.ChangeFrontBackPunchPosition(punchBackPos);
                        break;
                }
                break;
            case 1:
                switch (portIndex)
                {
                    case 0:
                        if (value)
                            correctScript.MoveBox(boxPositions[2]);
                        break;
                    case 1:
                        if (value)
                            correctScript.MoveBox(boxPositions[3]);
                        break;
                    case 2:
                        if (!value)
                            correctScript.RotateConveyorEuler('E', conveyorEGiveRotationY);
                        break;
                    case 3:
                        if (!value)
                            correctScript.RotateConveyorEuler('E', conveyorETakeRotationY);
                        break;
                    case 4:
                        if (!value)
                            correctScript.RotateConveyorEuler('C', conveyorCGiveRotationY);
                        break;
                    case 5:
                        if (!value)
                            correctScript.RotateConveyorEuler('C', conveyorCTakeRotationY);
                        break;
                }
                break;
            case 2:
                switch (portIndex)
                {
                    case 0:
                        if (value)
                            correctScript.MoveBox(boxPositions[4]);
                        break;
                    case 1:
                        if (value)
                            correctScript.MoveBox(boxPositions[5]);
                        break;
                    case 2:
                        if (!value)
                        {
                            correctScript.MoveConveyorA(conveyorAPositions[1].position);
                            RotateAllBoxPosition();
                        }
                        break;
                    case 3:
                        if (!value)
                        {
                            correctScript.MoveConveyorA(conveyorAPositions[0].position);
                            //RotateABoxPosition();
                        }
                        break;
                }
                break;
        }
    }
}
