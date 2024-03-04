using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class SetupLaunchManager : MonoBehaviour
{
    //SETUP

    [SerializeField]
    GameObject buttonsLeft;
    [SerializeField]
    GameObject buttonsSetup;

    [SerializeField]
    GameObject[] cameras; //0 left, 1 punch, 2 right

    [SerializeField]
    GameObject[] toggles; //0 left, 1 punch, 2 right

    [SerializeField]
    GameObject punchText;

    [SerializeField]
    GameObject[] piecesLeft; //RGBY
    [SerializeField]
    GameObject[] piecesRight;

    //LAUNCH

    [SerializeField]
    GameObject buttonsLaunch;

    [SerializeField]
    GameObject cameraLaunch;

    [SerializeField]
    GameObject[] enviorment;

    [SerializeField]
    GameObject[] piecesInBox;
    [SerializeField]
    GameObject[] fakePiecesInBox;
    [SerializeField]
    Transform box;

    Vector3 initialBoxRotation;

    MeshRenderer pieceLMesh;
    MeshRenderer pieceRMesh;

    [SerializeField]
    Material[] pieceMaterials; //RGBY

    [SerializeField]
    Transform slotsParent;

    [SerializeField]
    GameObject launchConveyor;

    CameraMovement cameraMovement;

    Launch launchScript;

    int setupStage;

    ConfigUpdate config;

    public static bool right = false;
    public static bool left = false;

    private bool[] leftPieces = new bool[4] { false, false, false, false };
    private bool[] rightPieces = new bool[4] { false, false, false, false };
    private bool[] actions = new bool[2] { false, false };

    int leftColor;
    int rightColor;

    [SerializeField]
    Transform draggableParent;

    [SerializeField]
    Image[] icons;

    [SerializeField]
    Sprite robotIcon;

    [SerializeField]
    Sprite humanIcon;

    [SerializeField]
    GameObject priorityScreen;

    CurrentStatus status;

    private string priority = string.Empty;

    private PortCorrecting portCorrecting;
    private void Start()
    {
        cameraMovement = FindObjectOfType<CameraMovement>();

        CancelSetup();
        CancelLaunch();
        launchScript = FindObjectOfType<Launch>();
        config = FindObjectOfType<ConfigUpdate>();
        status = FindObjectOfType<CurrentStatus>();
        portCorrecting = FindObjectOfType<PortCorrecting>();

        initialBoxRotation = box.eulerAngles;

    }

    public void ToggleRedLeft()
    {
        bool value = !leftPieces[0];

        piecesLeft[0].SetActive(value);
        leftPieces[0] = value;
        config.UpdateValues();
    }
    public void ToggleGreenLeft()
    {
        bool value = !leftPieces[1];

        piecesLeft[1].SetActive(value);
        leftPieces[1] = value;
        config.UpdateValues();
    }
    public void ToggleBlueLeft()
    {
        bool value = !leftPieces[2];

        piecesLeft[2].SetActive(value);
        leftPieces[2] = value;
        config.UpdateValues();
    }
    public void ToggleYellowLeft()
    {
        bool value = !leftPieces[3];

        piecesLeft[3].SetActive(value);
        leftPieces[3] = value;
        config.UpdateValues();
    }
    public void ToggleDrillPunch()
    {
        bool value = !actions[1];

        actions[1] = value;
        config.UpdateValues();
    }
    public void ToggleScrewPunch()
    {
        bool value = !actions[0];

        actions[0] = value;
        config.UpdateValues();

    }
    public void ToggleRedRight()
    {
        bool value = !rightPieces[0];

        piecesRight[0].SetActive(value);
        rightPieces[0] = value;
        config.UpdateValues();
    }
    public void ToggleGreenRight()
    {
        bool value = !rightPieces[1];

        piecesRight[1].SetActive(value);
        rightPieces[1] = value;
        config.UpdateValues();
    }
    public void ToggleBlueRight()
    {
        bool value = !rightPieces[2];

        piecesRight[2].SetActive(value);
        rightPieces[2] = value;
        config.UpdateValues();
    }
    public void ToggleYellowRight()
    {
        bool value = !rightPieces[3];

        piecesRight[3].SetActive(value);
        rightPieces[3] = value;
        config.UpdateValues();
    }
    public void StartSetup()
    {
        buttonsLeft.SetActive(false);
        buttonsSetup.SetActive(true);
        cameras[0].SetActive(true); //left
        toggles[0].SetActive(true);

        setupStage = 0;
    }
    public void Next()
    {
        toggles[setupStage].SetActive(false);
        if (setupStage == 1)
        {
            punchText.SetActive(false);
        }
        setupStage++;
        if (setupStage > 2)
        {
            CancelSetup();
        }
        else
        {
            cameras[setupStage].SetActive(true);
            toggles[setupStage].SetActive(true);
            if (setupStage == 1)
            {
                punchText.SetActive(true);
            }
        }
    }
    public void CancelSetup()
    {
        buttonsLeft.SetActive(true);
        buttonsSetup.SetActive(false);
        foreach (var cam in cameras)
        {
            cam.SetActive(false);
        }
        foreach (var toggle in toggles)
        {
            toggle.SetActive(false);
        }
        punchText.SetActive(false);
    }

    public void StartLaunch()
    {
        launchConveyor.SetActive(true);

        buttonsLaunch.SetActive(true);
        buttonsLeft.SetActive(false);

        pieceLMesh = fakePiecesInBox[0].GetComponent<MeshRenderer>();
        pieceRMesh = fakePiecesInBox[1].GetComponent<MeshRenderer>();

        cameraLaunch.SetActive(true);
        //pieceLMesh.material = pieceMaterialsFaded[GetColorByString(dropDown[0].text)];
        //pieceRMesh.material = pieceMaterialsFaded[GetColorByString(dropDown[1].text)];

        foreach (var go in enviorment)
        {
            go.SetActive(false);
        }

        cameraMovement.cameraMovementEnabled = false;
    }
    public void ChangePieceLeft(int mat)
    {
        if (!pieceLMesh.gameObject.activeSelf)
            pieceLMesh.gameObject.SetActive(true);
        pieceLMesh.material = pieceMaterials[mat];
        leftColor = mat;
    }
    public void DisablePieceLeft()
    {
        pieceLMesh.gameObject.SetActive(false);
    }
    public void ChangePieceRight(int mat)
    {
        if (!pieceRMesh.gameObject.activeSelf)
            pieceRMesh.gameObject.SetActive(true);
        pieceRMesh.material = pieceMaterials[mat];
        rightColor = mat;
    }
    public void DisablePieceRight()
    {
        pieceRMesh.gameObject.SetActive(false);
    }
    public void SubmitLaunch()
    {
        bool[] operators = status.GetOperatorsPositions();

        bool hasOperator = false;

        foreach (bool op in operators)
        {
            hasOperator = op || hasOperator;
        }

        List<Launch.paraJson> skills = new List<Launch.paraJson>();

        foreach (var go in enviorment)
        {
            go.SetActive(true);
        }
        foreach (var piece in piecesInBox)
        {
            piece.SetActive(false);
        }

        cameraLaunch.SetActive(false);
        buttonsLaunch.SetActive(false);

        for (int i = 0; i < slotsParent.childCount; i++)
        {
            Transform slot = slotsParent.GetChild(i);
            if (slot.childCount == 0)
                continue;
            else
            {
                string skillText = slot.GetChild(0).GetChild(0).GetComponent<TMP_Text>().text;

                int type = -1;
                string data = string.Empty;

                switch (skillText)
                {
                    case "Color 1":
                        type = 1;
                        data = GetColorString(leftColor);
                        break;
                    case "Color 2":
                        type = 2;
                        data = GetColorString(rightColor);
                        break;
                    case "Drill":
                        type = 0;
                        data = "Drill";
                        break;
                    case "Screw":
                        type = 0;
                        data = "Screw";
                        break;
                }
                skills.Add(new Launch.paraJson(type, data));

                RemoveSlot(slot.GetChild(0));
            }
        }

        portCorrecting.ResetAllBoxPositions();

        if (!hasOperator)
        {
            launchConveyor.SetActive(false);
            priorityScreen.SetActive(false);
            StartCoroutine(PrepareLaunch(skills));
            cameraMovement.cameraMovementEnabled = true;
            priority = "Station";
        }
        else
        {
            if (skills.Count > 0)
            {
                bool robotIsOnLeft = operators[0];
                bool robotIsOnRight = operators[1];
                bool humanIsOnLeft = operators[2];
                bool humanIsOnRight = operators[3];

                if (robotIsOnLeft)
                {
                    icons[0].sprite = robotIcon;
                }
                else if (humanIsOnLeft)
                {
                    icons[0].sprite = humanIcon;
                }

                if (robotIsOnRight)
                {
                    icons[1].sprite = robotIcon;
                }
                else if (humanIsOnRight)
                {
                    icons[1].sprite = humanIcon;
                }
            }
            launchConveyor.SetActive(false);
            priorityScreen.SetActive(true);
            StartCoroutine(PrepareLaunch(skills));
            cameraMovement.cameraMovementEnabled = true;
        }
    }

    public void ChoosePriority(int side)
    {
        bool[] operators = status.GetOperatorsPositions();

        bool robotIsOnLeft = operators[0];
        bool robotIsOnRight = operators[1];
        bool humanIsOnLeft = operators[2];
        bool humanIsOnRight = operators[3];

        if (side == 0)
        {
            if (robotIsOnLeft)
                priority = "Robot";
            else if (humanIsOnLeft)
                priority = "Human";
        }
        else if (side == 1)
        {
            if (robotIsOnRight)
                priority = "Robot";
            else if (humanIsOnRight)
                priority = "Human";
        }
    }

    IEnumerator PrepareLaunch(List<Launch.paraJson> skills)
    {

        yield return new WaitUntil(() => priority != string.Empty);

        launchScript.SubmitLaunch(skills, priority);

        priority = string.Empty;

        buttonsLeft.SetActive(true);
        priorityScreen.SetActive(false);
    }

    public void CancelLaunch()
    {
        foreach (var piece in fakePiecesInBox)
        {
            piece.SetActive(false);
        }
        cameraLaunch.SetActive(false);
        buttonsLaunch.SetActive(false);
        buttonsLeft.SetActive(true);
        launchConveyor.SetActive(false);

        foreach (var go in enviorment)
        {
            go.SetActive(true);
        }

        cameraMovement.cameraMovementEnabled = true;
    }
    private string GetColorString(int color)
    {
        switch (color)
        {
            case 0:
                return "Red";
            case 1:
                return "Green";
            case 2:
                return "Blue";
            case 3:
                return "Yellow";
            default:
                Debug.LogError("Color doesn't exist");
                return string.Empty;
        }
    }
    private int GetColorInt(string color)
    {
        switch (color)
        {
            case "Red":
                return 0;
            case "Green":
                return 1;
            case "Blue":
                return 2;
            case "Yellow":
                return 3;
            default:
                Debug.LogError("Color doesn't exist");
                break;
        }
        return -1;
    }

    public bool[] GetActiveColorsLeft()
    {
        return leftPieces;
    }
    public bool[] GetActiveColorsRight()
    {
        return rightPieces;
    }
    public bool[] GetActiveActionsPunch()
    {
        return actions;
    }
    private void RemoveSlot(Transform slot)
    {
        string buttonText = slot.GetChild(0).GetComponent<TMP_Text>().text;
        int childIndex = -1;
        if (buttonText == "Color 1")
        {
            childIndex = 0;
            ActionSlot.leftIsOnDraggables = true;

            DisablePieceLeft();
        }
        else if (buttonText == "Color 2")
        {
            if (ActionSlot.leftIsOnDraggables)
                childIndex = 1;
            else
                childIndex = 0;

            DisablePieceRight();
        }
        if (childIndex == -1)
        {
            Destroy(slot.parent.gameObject);
            Destroy(slot.gameObject);
        }
        else
        {
            StartCoroutine(ReturnColorToDraggable(slot, draggableParent, childIndex));
        }
    }
    IEnumerator ReturnColorToDraggable(Transform oldGO, Transform newParent, int childIndex)
    {
        yield return new WaitForEndOfFrame();

        Transform oldParent = oldGO.parent;
        oldGO.SetParent(newParent);
        oldGO.SetSiblingIndex(childIndex);

        Destroy(oldParent.gameObject);

    }
}
