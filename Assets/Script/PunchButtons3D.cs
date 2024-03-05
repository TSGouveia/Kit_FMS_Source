using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PunchButtons3D : MonoBehaviour
{

    [SerializeField]
    private TMP_Text buttonTextDrill;

    [SerializeField]
    private TMP_Text buttonTextScrew;

    [SerializeField]
    private Button buttonDrill;

    [SerializeField]
    private Button buttonScrew;

    private SetupLaunchManager launchManager;

    private const float pressHeight = 0.01f;
    private const float toggledHeight = 0.05f;

    private bool isToggledDrill = false;
    private bool isToggledScrew = false;

    private Color initialColorText;
    private Color initialColorButton;

    [SerializeField]
    private Color buttonColorSelected;
    private void Start()
    {
        initialColorText = buttonTextDrill.color;
        initialColorButton = buttonDrill.image.color;
        launchManager = FindObjectOfType<SetupLaunchManager>();
    }

    public void OnClickDrill()
    {
        if (!isToggledDrill)
        {
            buttonTextDrill.color = Color.white;
            buttonDrill.image.color = buttonColorSelected;
            transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z + toggledHeight);
        }
        else
        {
            buttonTextDrill.color = initialColorText;
            buttonDrill.image.color = initialColorButton;
            transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z + pressHeight);
        }

        isToggledDrill = !isToggledDrill;
        launchManager.ToggleDrillPunch();
    }
    public void OnClickScrew()
    {
        if (!isToggledScrew)
        {
            buttonTextScrew.color = Color.white;
            buttonScrew.image.color = buttonColorSelected;
            transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z + toggledHeight);
        }
        else
        {
            buttonTextScrew.color = initialColorText;
            buttonScrew.image.color = initialColorButton;
            transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z + pressHeight);
        }

        isToggledScrew = !isToggledScrew;
        launchManager.ToggleScrewPunch();
    }
    public bool[] GetActions()
    {
        bool[] returnValue = new bool[2];

        returnValue[0] = isToggledDrill;
        returnValue[1] = isToggledScrew;

        Debug.Log(isToggledDrill);
        Debug.Log(isToggledScrew);

        return returnValue;
    }
}
