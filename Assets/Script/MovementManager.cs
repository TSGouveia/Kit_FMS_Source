using System.Collections;
using UnityEngine;

public class MovementManager : MonoBehaviour
{

    private BitArray[] portR;
    private Mechanism mechanism;
    private WarningManagerBox warningManagerBox;
    private WarningManagerPhysical warningManagerPhysical;

    private void Awake()
    {
        portR = PortManager.portR;
        mechanism = GetComponent<Mechanism>();
        warningManagerBox = GetComponent<WarningManagerBox>();
        warningManagerPhysical = GetComponent<WarningManagerPhysical>();
    }
    public void OnPortValueChanged(char portLetter, int portNumber, int portIndex, bool value)
    {
        if (portLetter == 'R')
        {
            warningManagerBox.ChangeWaitingSensor(portNumber, portIndex, value);
            warningManagerPhysical.ChangeWaitingSensor(portNumber, portIndex, value);

            switch (portNumber)
            {
                case 0:
                    switch (portIndex)
                    {
                        case 0:
                            //Do nothing?
                            PrintErrorMessage(portNumber, portIndex);
                            break;
                        case 1:
                            //Do nothing?
                            PrintErrorMessage(portNumber, portIndex);
                            break;
                        case 2:
                            //Punch Up
                            if (value)
                                mechanism.VerticalDirPunch(false);
                            else
                                mechanism.StopVerticalDirPunch();
                            break;
                        case 3:
                            //Punch Back
                            if (value)
                                mechanism.HorizontalDirPunch(false);
                            else
                                mechanism.StopHorizontalDirPunch();
                            break;
                        case 4:
                            //Conveyor E
                            mechanism.ChangeConveyorState('E', value);
                            break;
                        case 5:
                            //Conveyor E Rotate AntiClockwise
                            if (value)
                                mechanism.RotateConveyor('E', false);
                            else
                                mechanism.StopRotateConveyor('E');
                            break;
                        case 6:
                            //Conveyor C Rotate Clockwise
                            if (value)
                                mechanism.RotateConveyor('C', true);
                            else
                                mechanism.StopRotateConveyor('C');
                            break;
                        case 7:
                            // Conveyor A Backwards
                            mechanism.ChangeConveyorState('A', value);
                            break;
                        case 8:
                            // Conveyor A Left
                            if (value)
                                mechanism.TranslationConveyor('A', false);
                            else
                                mechanism.StopTranslationConveyor('A');
                            break;
                        default:
                            PrintErrorMessage(portNumber, portIndex);
                            break;
                    }
                    break;
                case 1:
                    switch (portIndex)
                    {
                        case 0:
                            //Do nothing?
                            PrintErrorMessage(portNumber, portIndex);
                            break;
                        case 1:
                            //Do nothing?
                            PrintErrorMessage(portNumber, portIndex);
                            break;
                        case 2:
                            //Do nothing?
                            PrintErrorMessage(portNumber, portIndex);
                            break;
                        case 3:
                            //Punch Down
                            if (value)
                                mechanism.VerticalDirPunch(true);
                            else
                                mechanism.StopVerticalDirPunch();
                            break;
                        case 4:
                            //Conveyor F
                            mechanism.ChangeConveyorState('F', value);
                            break;
                        case 5:
                            //Conveyor E Rotate Clockwise
                            if (value)
                                mechanism.RotateConveyor('E', true);
                            else
                                mechanism.StopRotateConveyor('E');
                            break;
                        case 6:
                            //Conveyor C
                            mechanism.ChangeConveyorState('C', value);
                            break;
                        case 7:
                            //Conveyor B
                            mechanism.ChangeConveyorState('B', value);
                            break;
                        case 8:
                            //Conveyor A Right
                            if (value)
                                mechanism.TranslationConveyor('A', true);
                            else
                                mechanism.StopTranslationConveyor('A');
                            break;
                        default:
                            PrintErrorMessage(portNumber, portIndex);
                            break;
                    }
                    break;
                case 2:
                    switch (portIndex)
                    {
                        case 0:
                            //Do nothing?
                            PrintErrorMessage(portNumber, portIndex);
                            break;
                        case 1:
                            //Do nothing?
                            PrintErrorMessage(portNumber, portIndex);
                            break;
                        case 2:
                            //Do nothing?
                            PrintErrorMessage(portNumber, portIndex);
                            break;
                        case 3:
                            //Punch Spin
                            mechanism.RotatePunch(value);
                            break;
                        case 4:
                            //Punch Forward
                            if (value)
                                mechanism.HorizontalDirPunch(true);
                            else
                                mechanism.StopHorizontalDirPunch();
                            break;
                        case 5:
                            //Do nothing?
                            PrintErrorMessage(portNumber, portIndex);
                            break;
                        case 6:
                            //Conveyor D
                            mechanism.ChangeConveyorState('D', value);
                            break;
                        case 7:
                            //Conveyor C Rotate AntiClockwise
                            if (value)
                                mechanism.RotateConveyor('C', false);
                            else
                                mechanism.StopRotateConveyor('C');
                            break;
                        case 8:
                            //Conveyor A
                            mechanism.ChangeConveyorStateBackwards('A', value);
                            break;
                        default:
                            PrintErrorMessage(portNumber, portIndex);
                            break;
                    }
                    break;
                default:
                    PrintErrorMessage(portNumber, portIndex);
                    break;
            }
        }
    }
    private void PrintErrorMessage(int portNumber, int portIndex)
    {
        Debug.LogWarning("Port" + portNumber + "_" + portIndex + " changed not referenced to any movement");
    }
}
