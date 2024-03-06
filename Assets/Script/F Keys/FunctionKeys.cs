using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FunctionKeys : MonoBehaviour
{
    bool isFullscreen = true;

    [SerializeField] int widthPixels;

    [SerializeField] int heightPixels;


    [SerializeField] GameObject helpMenu;

    private void Start()
    {
        isFullscreen = Screen.fullScreen;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F1))
        {
            helpMenu.SetActive(!helpMenu.activeSelf);
        }
        if (Input.GetKeyDown(KeyCode.F11))
        {
            isFullscreen = !isFullscreen;

            if(isFullscreen)
            {
                Screen.SetResolution(widthPixels,heightPixels,true);
            }
            else
            {
                Screen.fullScreen = false;
            }
        }
    }
}
