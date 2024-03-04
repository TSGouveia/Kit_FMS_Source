using UnityEngine;
using Newtonsoft.Json.Linq;
using System.Net;
using System.IO;
using System.Text;
using System.Threading;
using Unity.Collections;
using System;
using System.Collections.Generic;
using Unity.VisualScripting.FullSerializer;

public class CurrentStatus : MonoBehaviour
{
    private HttpListenerContext context;
    private HttpListener listener;
    private Thread listenerThread;

    private Dictionary<string, string> dicionarioResource;
    private string productLocation;

    private bool atualizaUI;

    private bool robotIsOnLeft;
    private bool robotIsOnRight;
    private bool humanIsOnLeft;
    private bool humanIsOnRight;

    private Mechanism mechanism;

    private ConfigUpdate config;

    bool atualizaConfig = false;

    private void Start()
    {
        StartServer();
        mechanism = FindObjectOfType<Mechanism>();
        config = FindObjectOfType<ConfigUpdate>();
    }
    void StartServer()
    {
        // Replace this with the port you want to listen on
        string url = "http://localhost:8081/updateHMI/";

        listener = new HttpListener();
        listener.Prefixes.Add(url);

        listener.Start();

        Debug.Log("Server started. Listening for incoming requests...");

        // Start listening for incoming requests on a separate thread
        listenerThread = new Thread(new ThreadStart(ListenForRequests));
        listenerThread.Start();
    }
    void OnDisable()
    {
        // Stop the listener when the script is disabled or the game is closed
        if (listener != null && listener.IsListening)
        {
            listener.Stop();
            Debug.Log("Server stopped.");
        }

        // Stop the listener thread
        if (listenerThread != null && listenerThread.IsAlive)
        {
            listenerThread.Abort();
        }
    }

    void ListenForRequests()
    {
        while (listener.IsListening)
        {
            // Wait for a request to come in
            context = listener.GetContext();


            // Process the request on the main thread
            HandleRequest(context);
        }
    }

    void EnviaResposta(string str)
    {
        string responseString = str;
        byte[] responsebytes = Encoding.UTF8.GetBytes(responseString);
        context.Response.OutputStream.Write(responsebytes, 0, responsebytes.Length);
        context.Response.Close();
    }

    void HandleRequest(HttpListenerContext context)
    {
        if (context.Request.HttpMethod == "POST")
        {
            // Read the request body
            using (Stream body = context.Request.InputStream)
            {
                using (StreamReader reader = new StreamReader(body, context.Request.ContentEncoding))
                {
                    string receivedJson = reader.ReadToEnd();

                    Debug.Log("Received JSON: " + receivedJson);

                    JObject currentStatus = JObject.Parse(receivedJson);

                    // Access values from the JObject directly
                    dicionarioResource = currentStatus["Resources"].ToObject<Dictionary<string, string>>();
                    productLocation = currentStatus["ProductLocation"].ToObject<string>();

                    EnviaResposta("Saga");
                    atualizaUI = true;
                }
            }
        }
        else
        {
            // Handle other HTTP methods or provide an error response
            context.Response.StatusCode = (int)HttpStatusCode.MethodNotAllowed;
            context.Response.Close();
        }
    }

    private void Update()
    {
        if (atualizaUI)
        {
            if (dicionarioResource.ContainsKey("Station"))
            {
                //Martelo
                string pos = dicionarioResource["Station"];
                Debug.Log("Station");
                Debug.Log("Recebi " + pos);
            }

            if (dicionarioResource.ContainsKey("Operator1"))
            {
                string pos = dicionarioResource["Operator1"];
                if (pos == "B")
                {
                    Debug.Log("HUMAN LEFT3");
                    if (!humanIsOnLeft || humanIsOnRight)
                        atualizaConfig = true;

                    humanIsOnLeft = true;
                    humanIsOnRight = false;

                    mechanism.SpawnHumanLeft();
                    mechanism.DispawnHumanRight();
                }
                if (pos == "F")
                {
                    Debug.Log("HUMAN LEFT2");
                    if (humanIsOnLeft || !humanIsOnRight)
                        atualizaConfig = true;

                    humanIsOnLeft = false;
                    humanIsOnRight = true;

                    mechanism.SpawnHumanRight();
                    mechanism.DispawnHumanLeft();
                }
                //Funcao de atualizar o home para o seu sitio (pos)
            }
            else
            {
                Debug.Log("HUMAN LEFT1");
                if (humanIsOnLeft || humanIsOnRight)
                    atualizaConfig = true;

                humanIsOnLeft = false;
                humanIsOnRight = false;

                mechanism.DispawnHumanLeft();
                mechanism.DispawnHumanRight();
            }

            if (dicionarioResource.ContainsKey("RobotB"))
            {
                string pos = dicionarioResource["RobotB"];

                if (!robotIsOnLeft)
                    atualizaConfig = true;
                Debug.Log(pos);
                robotIsOnLeft = pos == "B";
                mechanism.SpawnRobotLeft();

                //Funcao de atualizar o bot para o seu sitio (pos)
            }
            else
            {
                if (robotIsOnLeft)
                    atualizaConfig = true;


                robotIsOnLeft = false;
                mechanism.DispawnRobotLeft();

            }

            if (dicionarioResource.ContainsKey("RobotF"))
            {
                string pos = dicionarioResource["RobotF"];

                if (!robotIsOnRight)
                    atualizaConfig = true;
                Debug.Log(pos);
                robotIsOnRight = pos == "F";

                mechanism.SpawnRobotRight();

                //Funcao de atualizar o bot para o seu sitio (pos)
            }
            else
            {

                if (robotIsOnRight)
                    atualizaConfig = true;

                robotIsOnRight = false;
                mechanism.DispawnRobotRight();
            }

            if (productLocation != string.Empty)
            {
                //Ignorar esta variavel porque temos digital shadow
            }

            atualizaUI = false;
            if (atualizaConfig)
            {
                atualizaConfig = false;
                config.UpdateValues();
            }

            SetupLaunchManager.left = humanIsOnLeft;
            SetupLaunchManager.right = humanIsOnRight;
        }
    }
    public bool[] GetOperatorsPositions()
    {
        bool[] positions = { robotIsOnLeft, robotIsOnRight, humanIsOnLeft, humanIsOnRight };

        return positions;
    }
}