using UnityEngine;
using Newtonsoft.Json.Linq;
using System.Net;
using System.IO;
using System.Text;
using System.Threading;
using System;
using System.Collections.Generic;
using System.Linq;

public class ReceiveColor : MonoBehaviour
{
    private HttpListenerContext context;
    private HttpListener listener;
    private Thread listenerThread;

    private PiecesScript script;

    private int piecePos;
    private int pieceColor;

    private bool updateColorPiece;

    private void Start()
    {
        StartServer();
        script = FindObjectOfType<PiecesScript>();
    }
    void StartServer()
    {
        // Replace this with the port you want to listen on
        string url = "http://" + GetLocalIPv4() + ":8082/";

        listener = new HttpListener();
        listener.Prefixes.Add(url);

        listener.Start();

        Debug.Log("Server for colors started. Listening for incoming requests...");

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
    private string GetLocalIPv4()
    {
        return Dns.GetHostEntry(Dns.GetHostName()).AddressList.First(f => f.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork).ToString();
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
        byte[] responseBytes = Encoding.UTF8.GetBytes(responseString);
        context.Response.OutputStream.Write(responseBytes, 0, responseBytes.Length);
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

                    JObject data = JObject.Parse(receivedJson);

                    AtualizarUI(data["Position"].ToString(), data["Color"].ToString());

                    EnviaResposta("Saga");
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

    private void AtualizarUI(string pos, string color)
    {
        if (!Client.isConnectedArduino)
            return;
        Debug.Log("Atualizar");
        piecePos = int.Parse(pos);
        switch (color)
        {
            case "Red":
                pieceColor = 0;
                break;
            case "Green":
                pieceColor = 1;
                break;
            case "Blue":
                pieceColor = 2;
                break;
            case "Yellow":
                pieceColor = 3;
                break;
            default:
                Debug.LogWarning("Color doesn't exist");
                break;
        }
        updateColorPiece = true;
    }
    private void Update()
    {
        if (updateColorPiece)
        {
            updateColorPiece = false;
            script.InsertPiece(piecePos, pieceColor);
        }
    }

}