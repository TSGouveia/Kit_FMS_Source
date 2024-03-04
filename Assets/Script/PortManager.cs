using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class PortManager : MonoBehaviour
{
    private const int ISlots = 6;
    private const int RSlots = 9;
    private const int IPortCount = 3;
    private const int RPortCount = 3;

    public static BitArray[] portI = new BitArray[IPortCount];
    public static BitArray[] portR = new BitArray[RPortCount];

    public TMP_Text textUiPortI;
    public TMP_Text textUiPortR;

    private MovementManager movementManager;
    private WarningManagerBox warningManagerBox;
    private WarningManagerPhysical warningManagerPhysical;

    private void Awake()
    {
        BitArray portI0 = new BitArray(ISlots, false);
        BitArray portI1 = new BitArray(ISlots, false);
        BitArray portI2 = new BitArray(ISlots, false);

        BitArray portR0 = new BitArray(RSlots, false);
        BitArray portR1 = new BitArray(RSlots, false);
        BitArray portR2 = new BitArray(RSlots, false);

        portI[0] = portI0;
        portI[1] = portI1;
        portI[2] = portI2;

        portR[0] = portR0;
        portR[1] = portR1;
        portR[2] = portR2;

        movementManager = GetComponent<MovementManager>();
        warningManagerBox = FindObjectOfType<WarningManagerBox>();
        warningManagerPhysical = FindObjectOfType<WarningManagerPhysical>();
    }

    void Start()
    {
        for (int i = 0; i < RPortCount; i++)
        {
            for (int j = 0; j < RSlots; j++)
            {
                ChangePortValue('R', i, j, false);
            }
        }
    }
    public void ChangePortValue(char portLetter, int portNumber, int portIndex, bool value)
    {
        if (portLetter == 'I')
        {
            warningManagerBox.RegisterDigitalSensorActivation(portNumber, portIndex, value);
            warningManagerPhysical.RegisterDigitalSensorActivation(portNumber, portIndex, value);
            portI[portNumber].Set(portIndex, value);
            UpdatePortUI(portI, textUiPortI);
        }
        else if (portLetter == 'R')
        {
            portR[portNumber].Set(portIndex, value);
            UpdatePortUI(portR, textUiPortR);
            movementManager.OnPortValueChanged(portLetter, portNumber, portIndex, value);
        }
    }
    private void UpdatePortUI(BitArray[] bitArray, TMP_Text textToUpdate)
    {
        string portText = "";
        for (int i = 0; i < bitArray.Length; i++)
        {
            for (int j = 0; j < bitArray[i].Length; j++)
            {
                string booleanText = bitArray[i][j] ? "1" : "0";
                portText += booleanText;
            }
            portText += "\n";
            textToUpdate.text = portText;
        }
    }
}