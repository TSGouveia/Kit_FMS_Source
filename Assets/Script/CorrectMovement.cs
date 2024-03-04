using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CorrectMovement : MonoBehaviour
{
    [SerializeField]
    private Transform boxGO;

    [SerializeField]
    private Transform conveyorA;

    [SerializeField]
    private Transform conveyorC;

    [SerializeField]
    private Transform conveyorE;

    [SerializeField]
    private Transform upDownPunch;

    [SerializeField]
    private Transform frontBackPunch;


    public void ChangeUpDownPunchPosition(Vector3 pos)
    {
        upDownPunch.localPosition = pos;
    }
    public void ChangeFrontBackPunchPosition(Vector3 pos)
    {
        frontBackPunch.localPosition = pos;
    }
    public void RotateConveyorEuler(char conveyorLetter, float yEuler)
    {
        switch (conveyorLetter)
        {
            case 'C':
                conveyorC.localEulerAngles = new Vector3(0, yEuler, 0);
                break;
            case 'E':
                conveyorE.localEulerAngles = new Vector3(0, yEuler, 0);
                break;
            default:
                break;
        }
    }
    public void MoveBox(Transform pos)
    {
        StartCoroutine(UpdateBoxMovement(pos));
    }
    public void MoveConveyorA(Vector3 pos)
    {
        conveyorA.position = pos;
    }
    IEnumerator UpdateBoxMovement(Transform pos)
    {
        yield return new WaitForSeconds(0.5f);
        boxGO.position = pos.position;
        boxGO.eulerAngles = pos.eulerAngles;
    }
}
