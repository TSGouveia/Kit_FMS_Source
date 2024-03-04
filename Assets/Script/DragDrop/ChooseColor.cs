using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ChooseColor : MonoBehaviour
{
    [SerializeField]
    Transform draggables;

    [SerializeField]
    Transform slots;

    [SerializeField]
    GameObject chooseColorMenu;

    [SerializeField]
    TMP_Text colorSideText;

    SetupLaunchManager launchManager;

    private void Start()
    {
        launchManager = FindObjectOfType<SetupLaunchManager>();
    }
    public void StartChoosingColor(string side)
    {
        chooseColorMenu.SetActive(true);

        colorSideText.text = side;

        ModifyDrag(false);
    }

    public void ColorChosen(int color)
    {
        chooseColorMenu.SetActive(false);
        ModifyDrag(true);

        if (colorSideText.text == "Color 1")
        {
            launchManager.ChangePieceLeft(color);
        }

        else if (colorSideText.text == "Color 2")
        {
            launchManager.ChangePieceRight(color);
        }
    }

    private void ModifyDrag(bool value)
    {
        for (int i = 0; i < draggables.childCount; ++i)
        {
            draggables.GetChild(i).gameObject.GetComponent<CanvasGroup>().blocksRaycasts = value;
        }
        for (int i = 0; i < slots.childCount; ++i)
        {
            Transform slotParent = slots.GetChild(i);
            slotParent.GetComponent<CanvasGroup>().blocksRaycasts = value;

            /*if(slotParent.childCount != 0)
            {
                slotParent.GetChild(0).gameObject.GetComponent<CanvasGroup>().blocksRaycasts = value;
            }*/
        }
    }
}
