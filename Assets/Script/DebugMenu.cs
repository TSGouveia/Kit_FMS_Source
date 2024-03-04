using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DebugMenu : MonoBehaviour
{
    [SerializeField]
    TMP_Text fpsText;

    [SerializeField]
    TMP_Text arduinoConnection;

    [SerializeField]
    TMP_Text agentsConnection;

    bool lastChangeArduino = true;
    bool lastChangeAgents = true;

    private void Start()
    {
        QualitySettings.vSyncCount = 1;
    }
    void Update()
    {
        fpsText.text = "FPS: "+ Math.Round(1f / Time.smoothDeltaTime).ToString();
        
        if (lastChangeArduino != Client.isConnectedArduino) 
        {
            lastChangeArduino = Client.isConnectedArduino;
            if (lastChangeArduino)
            {
                arduinoConnection.text = "Arduino: online";
            }
            else
            {
                arduinoConnection.text = "Arduino: offline";
            }
        }

        if (lastChangeAgents != AgentsConnection.isConnected)
        {
            lastChangeAgents = AgentsConnection.isConnected;
            if (lastChangeAgents)
            {
                agentsConnection.text = "Agents: online";
            }
            else
            {
                agentsConnection.text = "Agents: offline";
            }
        }

    }
}
