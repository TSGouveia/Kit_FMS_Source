using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class AgentsConnection : MonoBehaviour
{
    public static bool isConnected = false;
    private bool enviar = true;

    private void Start()
    {
        isConnected = false;
    }
    private void Update()
    {
        if (enviar)
        {
            StartCoroutine(SendPostUpdateTabletIP());
            enviar = false;
        }
    }
    
    private IEnumerator SendPostUpdateTabletIP()
    {
        WWWForm form = new WWWForm();
        string url = "http://localhost:8080/updateTabletIP";

        string body = PlayerPrefs.GetString("Tablet","192.168.2.102");

        // Create a UnityWebRequest with the POST method
        UnityWebRequest request = UnityWebRequest.Post(url, form);

        // Set the request body with your JSON data
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(body);
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();

        // Set the content type header
        request.SetRequestHeader("Content-Type", "text/plain");

        Debug.Log("Sending Post: " + body);
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            Debug.Log("Post has been sent");
            Debug.Log("Request successful!");
            Debug.Log("Response: " + request.downloadHandler.text);

            isConnected = true;
        }
        else
        {
            enviar = true;
        }
    }
}
