using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class WarningManagerBox : MonoBehaviour
{
    private const float maxWarningTime = 1f;

    private readonly BitArray[] portI = PortManager.portI;

    [SerializeField]
    private GameObject warningUI;

    [SerializeField]
    private bool warningEnabled;

    private PortManager portManager;


    private int waitingSensorNumber = -1;
    private int waitingSensorIndex = -1;
    private bool lastStateSensor = false;
    private bool physicalSensorActivated;
    private bool digitalSensorActivated;
    private float warningTimer;

    private char lastBoxLocation;

    private void Start()
    {
        portManager = FindObjectOfType<PortManager>();
    }
    void Update()
    {
        if (!warningEnabled)
            return;

        if (digitalSensorActivated && !physicalSensorActivated)
        {
            warningTimer += Time.deltaTime;
        }

        if (warningTimer > maxWarningTime && !warningUI.activeSelf)
        {
            warningUI.SetActive(true);
            Debug.Log("Warning launched at " + waitingSensorNumber + " " + waitingSensorIndex);
        }

        if (digitalSensorActivated && physicalSensorActivated)
        {
            Debug.Log("Variables Reseted");
            //Reset variables
            ChangeWaitingSensorLocal(-1, -1);
        }
    }
    private void ChangeWaitingSensorLocal(int sensorPortNumber, int sensorPortIndex)
    {
        Debug.Log("Sensor changed to " + sensorPortNumber + " " + sensorPortIndex);
        waitingSensorNumber = sensorPortNumber;
        waitingSensorIndex = sensorPortIndex;
        if (waitingSensorNumber != -1 && waitingSensorIndex != -1)
        {
            lastStateSensor = portI[sensorPortNumber][sensorPortIndex];
        }
        digitalSensorActivated = false;
        physicalSensorActivated = false;
        warningTimer = 0;
    }

    public void ChangeWaitingSensor(int portNumber, int portIndex, bool value)
    {
        if (!value)
            return;

        else
        {
            switch (portNumber)
            {
                case 0:
                    switch (portIndex)
                    {
                        case 2:
                            //Punch Up
                            //ChangeWaitingSensorLocal(0, 3);
                            break;
                        case 3:
                            //Punch Back
                            //ChangeWaitingSensorLocal(0, 5);
                            break;
                        case 4:
                            //Conveyor E
                            if (lastBoxLocation == 'E')
                                ChangeWaitingSensorLocal(2, 1);
                            break;
                        case 5:
                            //Conveyor E Rotate AntiClockwise
                            //ChangeWaitingSensorLocal(1, 3);
                            break;
                        case 6:
                            //Conveyor C Rotate Clockwise
                            //ChangeWaitingSensorLocal(1, 4);
                            break;
                        case 7:
                            // Conveyor A Backwards
                            break;
                        case 8:
                            // Conveyor A Right
                            //ChangeWaitingSensorLocal(2, 3);
                            break;
                    }
                    break;
                case 1:
                    switch (portIndex)
                    {
                        case 3:
                            //Punch Down
                            //ChangeWaitingSensorLocal(0, 2);
                            break;
                        case 4:
                            //Conveyor F
                            if (lastBoxLocation == 'F')
                                ChangeWaitingSensorLocal(0, 1);
                            break;
                        case 5:
                            //Conveyor E Rotate Clockwise
                            //ChangeWaitingSensorLocal(1, 2);
                            break;
                        case 6:
                            //Conveyor C
                            if (lastBoxLocation == 'C')
                                ChangeWaitingSensorLocal(1, 1);
                            break;
                        case 7:
                            //Conveyor B
                            if (lastBoxLocation == 'B')
                                ChangeWaitingSensorLocal(1, 0);
                            break;
                        case 8:
                            //Conveyor A Left
                            //ChangeWaitingSensorLocal(2, 2);
                            break;
                    }
                    break;
                case 2:
                    switch (portIndex)
                    {
                        case 3:
                            //Punch Spin
                            break;
                        case 4:
                            //Punch Forward
                            //ChangeWaitingSensorLocal(0, 4);
                            break;
                        case 6:
                            //Conveyor D
                            if (lastBoxLocation == 'D')
                                ChangeWaitingSensorLocal(2, 0);
                            break;
                        case 7:
                            //Conveyor C Rotate AntiClockwise
                            //ChangeWaitingSensorLocal(1, 5);
                            break;
                        case 8:
                            //Conveyor A
                            ChangeWaitingSensorLocal(0, 0);
                            break;
                    }
                    break;
            }
        }
    }

    public void RegisterPhysicalSensorActivation(int portNumber, int portIndex, bool value)
    {
        if (portNumber == waitingSensorNumber && portIndex == waitingSensorIndex && value != lastStateSensor)
        {
            physicalSensorActivated = true;
        }
    }
    public void RegisterDigitalSensorActivation(int portNumber, int portIndex, bool value)
    {
        RegisterLastBoxLocation(portNumber, portIndex, value);
        if (portNumber == waitingSensorNumber && portIndex == waitingSensorIndex && value != lastStateSensor)
        {
            digitalSensorActivated = true;
            warningTimer = 0;

            switch (portNumber)
            {
                case 0:
                    switch (portIndex)
                    {
                        case 0:
                            ChangeConveyorABackwards(false);
                            ChangeConveyorB(false);
                            break;
                        case 1:
                            ChangeConveyorF(false);
                            ChangeConveyorA(false);
                            break;
                        case 2:
                            PunchDown(false);
                            break;
                        case 3:
                            PunchUp(false);
                            break;
                        case 4:
                            PunchForward(false);
                            break;
                        case 5:
                            PunchBackwards(false);
                            break;
                    }
                    break;
                case 1:
                    switch (portIndex)
                    {
                        case 0:
                            ChangeConveyorB(false);
                            ChangeConveyorC(false);
                            break;
                        case 1:
                            ChangeConveyorC(false);
                            ChangeConveyorD(false);
                            break;
                        case 2:
                            portManager.ChangePortValue('R', 1, 5, false);
                            break;
                        case 3:
                            portManager.ChangePortValue('R', 0, 5, false);
                            break;
                        case 4:
                            portManager.ChangePortValue('R', 0, 6, false);
                            break;
                        case 5:
                            portManager.ChangePortValue('R', 2, 7, false);
                            break;
                    }
                    break;
                case 2:
                    switch (portIndex)
                    {
                        case 0:
                            ChangeConveyorD(false);
                            ChangeConveyorE(false);
                            break;
                        case 1:
                            ChangeConveyorE(false);
                            ChangeConveyorF(false);
                            break;
                        case 2:
                            portManager.ChangePortValue('R', 1, 8, false);
                            break;
                        case 3:
                            portManager.ChangePortValue('R', 0, 8, false);
                            break;
                    }
                    break;
            }
            void ChangeConveyorA(bool on)
            {
                portManager.ChangePortValue('R', 0, 7, on);
            }

            void ChangeConveyorABackwards(bool on)
            {
                portManager.ChangePortValue('R', 2, 8, on);
            }
            void ChangeConveyorB(bool on)
            {
                portManager.ChangePortValue('R', 1, 7, on);
            }
            void ChangeConveyorC(bool on)
            {
                portManager.ChangePortValue('R', 1, 6, on);
            }
            void ChangeConveyorD(bool on)
            {
                portManager.ChangePortValue('R', 2, 6, on);
            }
            void ChangeConveyorE(bool on)
            {
                portManager.ChangePortValue('R', 0, 4, on);
            }
            void ChangeConveyorF(bool on)
            {
                portManager.ChangePortValue('R', 1, 4, on);
            }
            void PunchForward(bool on)
            {
                portManager.ChangePortValue('R', 2, 4, on);
            }
            void PunchBackwards(bool on)
            {
                portManager.ChangePortValue('R', 0, 3, on);
            }
            void PunchUp(bool on)
            {
                portManager.ChangePortValue('R', 0, 2, on);
            }
            void PunchDown(bool on)
            {
                portManager.ChangePortValue('R', 1, 3, on);
            }
        }
    }
    private void RegisterLastBoxLocation(int portNumber, int portIndex, bool value)
    {
        if (!value)
            return;

        switch (portNumber)
        {
            case 0:
                switch (portIndex)
                {
                    case 0:
                        lastBoxLocation = 'B';
                        break;
                    case 1:
                        lastBoxLocation = 'A';
                        break;
                }
                break;
            case 1:
                switch (portIndex)
                {
                    case 0:
                        lastBoxLocation = 'C';
                        break;
                    case 1:
                        lastBoxLocation = 'D';
                        break;
                }
                break;
            case 2:
                switch (portIndex)
                {
                    case 0:
                        lastBoxLocation = 'E';
                        break;
                    case 1:
                        lastBoxLocation = 'F';
                        break;
                }
                break;
        }
    }
}