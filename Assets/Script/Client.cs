using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using Newtonsoft.Json.Linq;
using System.Threading;
using Unity.VisualScripting;
using TMPro;

public class Client : MonoBehaviour
{
    private string ip;
    private const int serverPort = 8888; // replace with your server's port

    private IPAddress serverIPAddress; // replace with your server's IP address

    private TcpClient client;
    public static bool isConnectedArduino = false;
    private NetworkStream stream;
    private PortManager portManager;
    private PortCorrecting portCorrecting;
    private Queue<jsonChange> changeQueue = new Queue<jsonChange>();

    [SerializeField]
    private GameObject connectButton;
    [SerializeField]
    private GameObject disconnectButton;

    Thread thread;

    // Start is called before the first frame update
    void Start()
    {
        ip = PlayerPrefs.GetString("Arduino", "192.168.2.28");
        portManager = FindObjectOfType<PortManager>();
        portCorrecting = FindObjectOfType<PortCorrecting>();
        Debug.Log("É Hora do Show");
        serverIPAddress = IPAddress.Parse(ip);
        isConnectedArduino = false;
        disconnectButton.SetActive(false);
    }

    public void ConnectClientThread()
    {
        if (isConnectedArduino) 
            return;

        // Create a TCP/IP socket
        client = new TcpClient();
        thread = new Thread(GetStreamData);
        thread.Start();    
        
        disconnectButton.SetActive(true);
        connectButton.SetActive(false);
    }

    public void DisconnectClient()
    {
        if (isConnectedArduino)
        {
            Debug.Log("Disconnecting from server");
            thread.Abort();
            client.Close();
            isConnectedArduino = false;
            Debug.Log("Connection disconnected");
            disconnectButton.SetActive(false);
            connectButton.SetActive(true);
        }
    }
    private void GetStreamData()
    {
        while (true)
        {
            if (!isConnectedArduino)
            {
                Debug.Log("Trying to connect");
                // Connect to the server
                client.Connect(serverIPAddress, serverPort);
                Debug.Log("Connected to the server.");

                // Get a network stream for reading and writing
                stream = client.GetStream();
                isConnectedArduino = true;
            }
            else
            {
                // Receive data from the server
                byte[] data = new byte[25];

                int bytesRead = stream.Read(data, 0, data.Length);
                string receivedMessage = Encoding.ASCII.GetString(data, 0, bytesRead);
                Debug.Log(receivedMessage);
                // Parse the JSON string into a JObject
                JObject json = JObject.Parse(receivedMessage);

                // Access values from the JObject directly
                String Port = (String)json["Port"];
                int Value = (int)json["Value"];

                //if (lastPort == Port) continue;
                //lastPort = Port;

                jsonChange tmp = new jsonChange();

                tmp.port = Port;
                tmp.value = Value;

                changeQueue.Enqueue(tmp);
            }
        }
    }
    private void Update()
    {
        while (changeQueue.Count > 0)
        {
            jsonChange change = changeQueue.Dequeue();

            string port = change.port.ToString();
            //port example  ->  I0_1
            //char location ->  0123
            char portLetter = port[0];
            int portNumber = port[1] - '0';
            int portIndex = port[3] - '0';
            int valueInt = change.value;
            bool value = valueInt == 1;
            portManager.ChangePortValue(portLetter, portNumber, portIndex, value);
            if(portLetter == 'I')
            {
                portCorrecting.ReceiveSensorPort(portNumber, portIndex, value);
            }
        }
    }
    private struct jsonChange
    {
        public String port;
        public int value;
    }

    private void OnApplicationQuit()
    {
        if (isConnectedArduino)
        {
            Debug.Log("Disconnecting from server");
            thread.Abort();
            client.Close();
            isConnectedArduino = false;
            Debug.Log("Connection disconnected");
        }
    }
}