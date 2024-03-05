using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisableUnusedButtons : MonoBehaviour
{
    [SerializeField] GameObject[] buttons; //0 drill / 1 screw

    private PunchButtons3D punchButtons;

    private void Start()
    {
        punchButtons = FindObjectOfType<PunchButtons3D>();
    }

    public void HideUnusedButtons()
    {
        Debug.Log("HIDE");

        bool[] actions = punchButtons.GetActions(); //0 drill / 1 screw

        string nameOfGameObject = gameObject.name;

        Debug.Log(nameOfGameObject);

        buttons[0].SetActive(actions[0]);
        buttons[1].SetActive(actions[1]);
    }
}
