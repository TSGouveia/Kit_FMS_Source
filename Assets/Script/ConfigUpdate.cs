using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;
using UnityEngine;
using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

public class ConfigUpdate : MonoBehaviour
{
    private const string yellow1 = "Skill_Yellow_1";
    private const string yellow2 = "Skill_Yellow_2";
    private const string green1 = "Skill_Green_1";
    private const string green2 = "Skill_Green_2";
    private const string red1 = "Skill_Red_1";
    private const string red2 = "Skill_Red_2";
    private const string blue1 = "Skill_Blue_1";
    private const string blue2 = "Skill_Blue_2";
    private const string skillDrill = "Skill_Drill";
    private const string skillScrew = "Skill_Screw";

    private bool hasOperator = false;
    private bool hasRobotLeft = false;
    private bool hasRobotRight = false;

    //Martelo
    private bool hasStation = true;

    private bool[] robotColors;
    private bool[] humanColors;
    private bool[] actions;

    private CurrentStatus status;
    private SetupLaunchManager manager;

    private void Start()
    {
        status = FindObjectOfType<CurrentStatus>();
        manager = FindObjectOfType<SetupLaunchManager>();
    }

    private IEnumerator SendPostUpdateConfiguration()
    {
        WWWForm form = new WWWForm();

        string url = "http://" + PlayerPrefs.GetString("Agents", "localhost") + ":8080/updateConfigurations";

        // construcao da string
        string jsonBody = JsonBodyStringBuilder();

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

    string JsonBodyStringBuilder()
    {
        string jsonBody;
        JObject total = new JObject();

        if (hasStation)
        {
            JObject resources = new JObject();

            JArray torre = new JArray();

            if (actions[1])
            {
                torre.Add(skillScrew);
            }
            if (actions[0])
            {
                torre.Add(skillDrill);
            }

            total["Station"] = torre;
        }

        if (hasOperator)
        {
            JArray operador = new JArray();

            if (humanColors[0])
            {
                operador.Add(red1);
                operador.Add(red2);
            }
            if (humanColors[1])
            {
                operador.Add(green1);
                operador.Add(green2);
            }
            if (humanColors[3])
            {
                operador.Add(yellow1);
                operador.Add(yellow2);
            }
            if (humanColors[2])
            {
                operador.Add(blue1);
                operador.Add(blue2);
            }
            total["Operator1"] = operador;
        }


        if ((hasRobotLeft || hasRobotRight))
        {
            JArray robot = new JArray();
            if (robotColors[0])
            {
                robot.Add(red1);
                robot.Add(red2);
            }
            if (robotColors[1])
            {
                robot.Add(green1);
                robot.Add(green2);
            }
            if (robotColors[3])
            {
                robot.Add(yellow1);
                robot.Add(yellow2);
            }
            if (robotColors[2])
            {
                robot.Add(blue1);
                robot.Add(blue2);
            }
            if (hasRobotLeft)
            {
                total["RobotB"] = robot;
            }
            else if (hasRobotRight)
            {
                total["RobotF"] = robot;
            }
        }

        JObject Resources = new JObject();

        Resources["Resources"] = total;

        jsonBody = Resources.ToString();
        Debug.Log(jsonBody);
        return jsonBody;
    }

    public void UpdateValues()
    {
        bool[] colorsLeft = manager.GetActiveColorsLeft();
        bool[] colorsRight = manager.GetActiveColorsRight();
        actions = manager.GetActiveActionsPunch();

        //robotIsOnLeft,robotIsOnRight,humanIsOnLeft,humanIsOnRight
        bool[] operators = status.GetOperatorsPositions();

        hasRobotLeft = operators[0];
        hasRobotRight = operators[1];
        hasOperator = operators[2] || operators[3];

        if (operators[0])
        {
            robotColors = colorsLeft;
        }

        if (operators[1])
        {
            robotColors = colorsRight;
        }

        if (operators[2])
        {
            humanColors = colorsLeft;
        }

        if (operators[3])
        {
            humanColors = colorsRight;
        }

        StartCoroutine(SendPostUpdateConfiguration());
    }
}
