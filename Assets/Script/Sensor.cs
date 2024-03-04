using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Sensor : MonoBehaviour
{
    private const string SensorActivatorTag = "SensorActivator";
    public bool activeHigh;
    public int[] sensorPort = new int[2];
    private PortManager portHandlerScript;

    private bool collisionCheck = false;
    private void Start()
    {
        portHandlerScript = FindObjectOfType<PortManager>();
        portHandlerScript.ChangePortValue('I', sensorPort[0], sensorPort[1], !activeHigh);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(SensorActivatorTag))
        {
            collisionCheck = true;
            portHandlerScript.ChangePortValue('I', sensorPort[0], sensorPort[1], activeHigh);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag(SensorActivatorTag))
        {
            portHandlerScript.ChangePortValue('I', sensorPort[0], sensorPort[1], !activeHigh);
        }
    }

    void OnTriggerStay(Collider other)
    {
        if (!collisionCheck)
        {
            if (other.CompareTag(SensorActivatorTag))
            {
                portHandlerScript.ChangePortValue('I', sensorPort[0], sensorPort[1], activeHigh);
            }
            else
            {
                portHandlerScript.ChangePortValue('I', sensorPort[0], sensorPort[1], !activeHigh);
            }
            collisionCheck = true;
        }
    }
}
