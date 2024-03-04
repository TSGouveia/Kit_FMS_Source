using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static System.Net.Mime.MediaTypeNames;

public class Settings : MonoBehaviour
{
    private const string ABB = "ABB";
    private const string Agents = "Agents";
    private const string Arduino = "Arduino";
    private const string Tablet = "Tablet";


    [SerializeField]
    TMP_InputField[] ipsTexts;

    [SerializeField]
    TMP_Text[] ipsPlaceholders;

    [SerializeField]
    Slider sensSlider;

    [SerializeField]
    TMP_Text sensNumber;

    CameraMovement cameraMovement;
    private void Start()
    {
        cameraMovement = FindObjectOfType<CameraMovement>();

        if (PlayerPrefs.HasKey(ABB))
        {
            ipsPlaceholders[0].text = PlayerPrefs.GetString(ABB);
        }
        if (PlayerPrefs.HasKey(Arduino))
        {
            ipsPlaceholders[1].text = PlayerPrefs.GetString(Arduino);
        }
        if (PlayerPrefs.HasKey(Tablet))
        {
            ipsPlaceholders[2].text = PlayerPrefs.GetString(Tablet);
        }
        if (PlayerPrefs.HasKey("Sensitivity"))
        {
            float value = PlayerPrefs.GetFloat("Sensitivity");

            sensSlider.value = value;
            sensNumber.text = string.Format("{0:F1}", value);
        }
    }
    public void ChangeSensitivity(float value)
    {
        PlayerPrefs.SetFloat("Sensitivity", value);
        sensNumber.text = string.Format("{0:F1}", value);
        cameraMovement.ChangeSensitivity();
    }
    public void ChangeABBIP()
    {
        string text = ipsTexts[0].text;
        PlayerPrefs.SetString(ABB, text);
    }
    public void ChangeArdunioIP()
    {
        string text = ipsTexts[1].text;
        PlayerPrefs.SetString(Arduino, text);
    }
    public void ChangeTabletIP()
    {
        string text = ipsTexts[2].text;
        PlayerPrefs.SetString(Tablet, text);
    }
}
