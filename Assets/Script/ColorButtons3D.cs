using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorButtons3D : MonoBehaviour
{
    [SerializeField]
    private int buttonFunction;

    private SetupLaunchManager launchManager;

    private const float pressHeight = 0.01f;
    private const float toggledHeight = 0.05f;

    private bool isToggled = false;
    private void Start()
    {
        launchManager = FindObjectOfType<SetupLaunchManager>();
    }

    private void OnMouseDown()
    {
        if (!isToggled)
        {
            transform.position = new Vector3(transform.position.x, transform.position.y - toggledHeight, transform.position.z);
        }
        else
        {
            transform.position = new Vector3(transform.position.x, transform.position.y - pressHeight, transform.position.z);
        }
        
        isToggled = !isToggled;
        DoButtonFunction(buttonFunction);        
    }
    private void OnMouseUp()
    {
        if (isToggled)
        {
            transform.position = new Vector3(transform.position.x, transform.position.y + pressHeight, transform.position.z);
        }
        else
        {
            transform.position = new Vector3(transform.position.x, transform.position.y + toggledHeight, transform.position.z);
        }

    }
    public void DoButtonFunction(int function)
    {
        switch (function)
        {
            case 0:
                launchManager.ToggleRedLeft();
                break;
            case 1:
                launchManager.ToggleGreenLeft();
                break;
            case 2:
                launchManager.ToggleBlueLeft();
                break;
            case 3:
                launchManager.ToggleYellowLeft();
                break;
            case 4:
                launchManager.ToggleDrillPunch();
                break;
            case 5:
                launchManager.ToggleScrewPunch();
                break;
            case 6:
                launchManager.ToggleRedRight();
                break;
            case 7:
                launchManager.ToggleGreenRight();
                break;
            case 8:
                launchManager.ToggleBlueRight();
                break;
            case 9:
                launchManager.ToggleYellowRight();
                break;
        }
    }
}
