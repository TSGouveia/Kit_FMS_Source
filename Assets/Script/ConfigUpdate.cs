using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;
using UnityEngine;
using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

public class ConfigUpdate : MonoBehaviour
{
    private const string YELLOW1 = "Skill_Yellow_1";
    private const string YELLOW2 = "Skill_Yellow_2";
    private const string GREEN1 = "Skill_Green_1";
    private const string GREEN2 = "Skill_Green_2";
    private const string RED1 = "Skill_Red_1";
    private const string RED2 = "Skill_Red_2";
    private const string BLUE1 = "Skill_Blue_1";
    private const string BLUE2 = "Skill_Blue_2";
    private const string SKILLDRILL = "Skill_Drill";
    private const string SKILLSCREW = "Skill_Screw";

    private bool hasOperator = false;
    private bool hasRobotLeft = false;
    private bool hasRobotRight = false;

    //Martelo
    private bool hasStation = true;

    private bool[] robotColors;
    private bool[] humanColors;
    private bool[] actions = { false, false };

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
                torre.Add(SKILLSCREW);
            }
            if (actions[0])
            {
                torre.Add(SKILLDRILL);
            }

            total["Station"] = torre;
        }

        if (hasOperator)
        {
            JArray operador = new JArray();

            if (humanColors[0])
            {
                operador.Add(RED1);
                operador.Add(RED2);
            }
            if (humanColors[1])
            {
                operador.Add(GREEN1);
                operador.Add(GREEN2);
            }
            if (humanColors[3])
            {
                operador.Add(YELLOW1);
                operador.Add(YELLOW2);
            }
            if (humanColors[2])
            {
                operador.Add(BLUE1);
                operador.Add(BLUE2);
            }
            total["Operator1"] = operador;
        }


        if ((hasRobotLeft || hasRobotRight))
        {
            JArray robot = new JArray();
            if (robotColors[0])
            {
                robot.Add(RED1);
                robot.Add(RED2);
            }
            if (robotColors[1])
            {
                robot.Add(GREEN1);
                robot.Add(GREEN2);
            }
            if (robotColors[3])
            {
                robot.Add(YELLOW1);
                robot.Add(YELLOW2);
            }
            if (robotColors[2])
            {
                robot.Add(BLUE1);
                robot.Add(BLUE2);
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