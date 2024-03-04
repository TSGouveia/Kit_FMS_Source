using Newtonsoft.Json.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using static System.Collections.Specialized.BitVector32;
using UnityEngine.UIElements;

public class Launch : MonoBehaviour
{
    private IEnumerator SendPostLaunch(List<paraJson> dados, string priority)
    {
        WWWForm form = new WWWForm();
        string url = "http://localhost:8080/launchProduct";

        string jsonBody = JsonBodyStringBuilder(dados, priority);

        // Create a UnityWebRequest with the POST method
        UnityWebRequest request = UnityWebRequest.Post(url, form);

        // Set the request body with your JSON data
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonBody);
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();

        // Set the content type header
        request.SetRequestHeader("Content-Type", "application/json");

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            Debug.Log("Post has been sent");
            Debug.Log("Request successful!");
            Debug.Log("Response: " + request.downloadHandler.text);
        }
        else
        {
            Debug.LogWarning("Request failed. Error: " + request.error);
        }
    }



    string JsonBodyStringBuilder(List<paraJson> dados, string priority)
    {
        string jsonBody;
        string paraArray;
        JObject total = new JObject();

        JArray instructions = new JArray();

        foreach (paraJson json in dados)
        {
            if (json.type == 0)
            {
                paraArray = "Skill_" + json.skill;
                instructions.Add(paraArray);
            }
            else if (json.type == 1)
            {
                paraArray = "Skill_" + json.skill + "_1";
                instructions.Add(paraArray);
            }
            else if (json.type == 2)
            {
                paraArray = "Skill_" + json.skill + "_2";
                instructions.Add(paraArray);
            }
            else
            {
                Debug.LogError("Error creating json");
            }

        }

        total["ListOfSkills"] = instructions;
        total["Priority"] = priority;

        jsonBody = total.ToString();
        Debug.Log(jsonBody);
        return jsonBody;
    }

    public void SubmitLaunch(List<paraJson> dados, string priority)
    {
        StartCoroutine(SendPostLaunch(dados, priority));
    }
    public struct paraJson
    {
        public int type; //0 para action / 1 para cor pos1 / 2 para cor pos2
        public string skill;

        public paraJson(int type, string skill)
        {
            this.type = type;
            this.skill = skill;
        }
    }
}
