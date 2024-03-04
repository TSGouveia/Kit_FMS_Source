using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIButtons : MonoBehaviour
{
    [SerializeField]
    private GameObject portUI;

    [SerializeField]
    private GameObject setupUI;

    [SerializeField]
    private GameObject launchUI;

    [SerializeField]
    private TMP_Text ipText;


    private bool portIsEnabled = false;
    private void Start()
    {
        ipText.text = "My IP: " + GetLocalIPv4();
    }
    public void PortDebug()
    {
        portIsEnabled = !portIsEnabled;
        portUI.SetActive(portIsEnabled);
    }
    public void Quit()
    {
        Application.Quit();
    }
    private string GetLocalIPv4()
    {
        return Dns.GetHostEntry(Dns.GetHostName()).AddressList.First(f => f.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork).ToString();
    }
    public void Restart()
    {
        Scene scene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(scene.name);
    }
}
